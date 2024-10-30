using System;
using System.Collections.Generic;
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
    
    public Dictionary<string, Func<WMWriter, int>> labels = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_BiomeName() },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> settings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "sample", (w) => w.On_Settings(w.writerManager.worldBiomeManager.samples) },
        { "sequence", (w) => w.On_Settings(w.writerManager.worldBiomeManager.sequences) },
        { "}", (w) => w.Increment(0, 1) },
    };

    public Dictionary<string, Func<WMWriter, int>> sequences = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "id", (w) =>
        {
            w.GetNextValue(out string value);
            if (!int.TryParse(value, out int result))
                return w.Error("id needs to be an integer");

            w.writerManager.worldBiomeManager._sequenceNode = new CWOCSequenceNode();
            w.writerManager.worldBiomeManager._sequenceNode.block = new Block((short)result, 0);
            return 0;
        } },
        { "fixed", (w) =>
        {
            if (w.GetNextInt(out int value) == -1)
                return w.Error("height must be an integer");

            w.writerManager.worldBiomeManager._sequenceNode.top_min = value;
            w.writerManager.worldBiomeManager._sequenceNode.top_max = value;
            return 0;
        } },
        { "set", (w) =>
        {
            if (w.GetNext2Ints(out Vector2Int ints) == -1)
            {
                if (w.GetNext2Values(out string[] values) == -1)
                    return w.Error("missing ','");

                int result;

                if (values[0].Equals("max") && int.TryParse(values[1], out result))
                {
                    if (result >= 1)
                    {
                        w.writerManager.worldBiomeManager._sequenceNode.top_min = 1;
                        w.writerManager.worldBiomeManager._sequenceNode.top_max = result;
                        return 0;
                    }
                    
                    return w.Error("the second value needs to be superior or equal to max (max = 1)");
                }
                
                if (int.TryParse(values[0], out var result1) && int.TryParse(values[1], out var result2))
                {
                    if (result2 >= result1)
                    {
                        w.writerManager.worldBiomeManager._sequenceNode.top_min = result1;
                        w.writerManager.worldBiomeManager._sequenceNode.top_max = result2;
                        return 0;
                    }
                    
                    return w.Error("the first value needs to be smaller or equal to the second");
                }
                
                if (int.TryParse(values[0], out result) && values[1].Equals("min"))
                {
                    w.writerManager.worldBiomeManager._sequenceNode.top_min = result;
                    w.writerManager.worldBiomeManager._sequenceNode.top_max = 9999;
                    return 0;
                }
                
                return w.Error("uhm... error");
            }
            
            w.writerManager.worldBiomeManager._sequenceNode.top_min = ints.x;
            w.writerManager.worldBiomeManager._sequenceNode.top_max = ints.y;
            return 0;
        } },
        { "}", (w) =>
        {
            w.writerManager.worldBiomeManager.biomeNode.SequenceNodes.Add(w.writerManager.worldBiomeManager._sequenceNode);
            return w.Increment(1, 1);
        } }
    };

    public Dictionary<string, Func<WMWriter, int>> samples = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "set", (w) => w.On_SetSample() },
        { "range", (w) =>
        {
            if (w.GetNext2Ints(out Vector2Int ints) == -1)
                return w.Error("no suitable ints found");

            w.writerManager.worldBiomeManager.biomeNode.min_height = ints.x;
            w.writerManager.worldBiomeManager.biomeNode.max_height = ints.y;
            return 0;
        } },
        { "}", (w) => w.Increment(1, 1) }
    };
}