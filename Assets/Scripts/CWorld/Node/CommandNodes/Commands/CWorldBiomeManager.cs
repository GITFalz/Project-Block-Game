using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CWorldBiomeManager : CWorldAbstractNode
{
    public static CWorldBiomeManager instance;
    
    public WMWriter writer;
    public CWorldBiomeNode biomeNode;
    public CWorldSampleNode sampleNode;

    public CWorldBiomeManager() { if (instance == null) instance = this; }
    
    private int id = -1;
    private CWOCSequenceNode _sequenceNode;

    public void SetBiome(CWorldBiomeNode biome)
    {
        biomeNode = biome;
        sampleNode = biomeNode.sample;
    }
    
    public Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref w.writerManager.currentBiomeName) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "sample", (w) => w.On_Settings(w.writerManager.worldBiomeManager.samples) },
        { "modifier", (w) => w.On_Settings(w.writerManager.worldBiomeManager.modifiers) },
        { "sequence", (w) => w.On_Settings(w.writerManager.worldBiomeManager.sequences) },
        { "}", (w) => w.Increment(0, 1) },
    };

    public Dictionary<string, Func<WMWriter, Task<int>>> sequences = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "id", async (w) =>
        {
            if (await w.GetNextInt(out int result) == -1)
                return await w.Error("id needs to be an integer");

            w.writerManager.worldBiomeManager._sequenceNode = new CWOCSequenceNode();
            w.writerManager.worldBiomeManager._sequenceNode.block = new Block((short)result, 0);
            return 0;
        } },
        { "fixed", async (w) =>
        {
            if (await w.GetNextInt(out int value) == -1)
                return await w.Error("height must be an integer");

            w.writerManager.worldBiomeManager._sequenceNode.top_min = value;
            w.writerManager.worldBiomeManager._sequenceNode.top_max = value;
            return 0;
        } },
        { 
            "set", async (w) =>
            {
                if (await w.GetNext2Ints(out Vector2Int ints) == -1)
                {
                    if (await w.GetNext2Values(out string[] values) == -1)
                        return await w.Error("missing ','");

                    int result;

                    if (values[0].Equals("max") && int.TryParse(values[1], out result))
                    {
                        if (result >= 1)
                        {
                            w.writerManager.worldBiomeManager._sequenceNode.top_min = 1;
                            w.writerManager.worldBiomeManager._sequenceNode.top_max = result;
                            return 0;
                        }
                        
                        return await w.Error("the second value needs to be superior or equal to max (max = 1)");
                    }
                    
                    if (int.TryParse(values[0], out var result1) && int.TryParse(values[1], out var result2))
                    {
                        if (result2 >= result1)
                        {
                            w.writerManager.worldBiomeManager._sequenceNode.top_min = result1;
                            w.writerManager.worldBiomeManager._sequenceNode.top_max = result2;
                            return 0;
                        }
                        
                        return await w.Error("the first value needs to be smaller or equal to the second");
                    }
                    
                    if (int.TryParse(values[0], out result) && values[1].Equals("min"))
                    {
                        w.writerManager.worldBiomeManager._sequenceNode.top_min = result;
                        w.writerManager.worldBiomeManager._sequenceNode.top_max = 9999;
                        return 0;
                    }
                    
                    return await w.Error("uhm... error");
                }
                
                w.writerManager.worldBiomeManager._sequenceNode.top_min = ints.x;
                w.writerManager.worldBiomeManager._sequenceNode.top_max = ints.y;
                return 0;
            } 
        },
        { "}", (w) =>
        {
            ChunkGenerationNodes.SetBiomeSequence(w.writerManager.worldBiomeManager._sequenceNode);
            return w.Increment(1, 1);
        } }
    };

    public Dictionary<string, Func<WMWriter, Task<int>>> samples = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "set", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeSample(value))
                    return await w.Error("Can't find the sample specified in the biome");
                return 0;
            }
        },
        { 
            "range", async (w) =>
            {
                if (await w.GetNext2Ints(out Vector2Int ints) == -1)
                    return await w.Error("no suitable ints found");
                await ChunkGenerationNodes.SetBiomeRange(ints);
                return 0;
            } 
        },
        { "}", (w) => w.Increment(1, 1) }
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> modifiers = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "use", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeModifier(value))
                    return await w.Error("Can't find the modifier specified in the biome");
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) }
    };
}