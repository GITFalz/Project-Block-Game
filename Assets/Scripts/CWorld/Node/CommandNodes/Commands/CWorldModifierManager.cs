using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CWorldModifierManager
{
    public static CWorldModifierManager instance;
    public CWorldMapNode MapNode;

    public CWorldModifierManager()
    {
        if (instance == null) instance = this;
    }
    
    public void SetMap(CWorldMapNode map)
    {
        MapNode = map;
    }
    
    public Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref w.writerManager.currentModifierName) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetModifierSample(value))
                    return await w.Error("Can't find the sample specified in the modifier");
                return 0;
            }
        },
        {
            "range", async (w) =>
            {
                if (await w.GetNext2Ints(out Vector2Int ints) == -1)
                    return await w.Error("no suitable ints found");
                await ChunkGenerationNodes.SetModifierRange(ints);
                return 0;
            }
        },
        {
            "ignore", async (w) =>
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("no suitable floats found");
                await ChunkGenerationNodes.SetModifierIgnore(floats);
                return 0;
            }
        },
        {
            "invert", async (w) =>
            {
                await ChunkGenerationNodes.SetModifierInvert(true);
                return 0;
            }
        },
        { 
            "gen", async (w) =>
            {
                await ChunkGenerationNodes.AddModifierGen();
                return await w.On_Settings(w.writerManager.worldModifierManager.biomeRanges);
            } 
        },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> biomeRanges = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetModifierGenSample(value))
                    return await w.Error("Can't find the sample specified in the modifier gen");
                return 0;
            }
        },
        {
            "range", async (w) =>
            {
                if (await w.GetNext2Ints(out Vector2Int ints) == -1)
                    return await w.Error("no suitable ints found");
                await ChunkGenerationNodes.SetModifierGenRange(ints);
                return 0;
            }
        },
        {
            "offset", async (w) =>
            {
                if (await w.GetNextInt(out int value) == -1)
                    return await w.Error("No good int found");
                await ChunkGenerationNodes.SetModifierGenOffset(value);
                return 0;
            }
        },
        {
            "flip", async (w) =>
            {
                await ChunkGenerationNodes.SetModifierGenFlip(true);
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
}