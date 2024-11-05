using System;
using System.Collections.Generic;
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
    
    public Dictionary<string, Func<WMWriter, int>> labels = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref w.writerManager.currentModifierName) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> settings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", (w) =>
            {
                w.GetNextValue(out var value);
                if (!ChunkGenerationNodes.SetModifierSample(value))
                    return w.Error("Can't find the sample specified in the modifier");
                return 0;
            }
        },
        {
            "range", (w) =>
            {
                if (w.GetNext2Ints(out Vector2Int ints) == -1)
                    return w.Error("no suitable ints found");
                ChunkGenerationNodes.SetModifierRange(ints);
                return 0;
            }
        },
        {
            "ignore", (w) =>
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("no suitable floats found");
                ChunkGenerationNodes.SetModifierIgnore(floats);
                return 0;
            }
        },
        {
            "invert", (w) =>
            {
                ChunkGenerationNodes.SetModifierInvert(true);
                return 0;
            }
        },
        { 
            "gen", (w) =>
            {
                ChunkGenerationNodes.AddModifierGen();
                return w.On_Settings(w.writerManager.worldModifierManager.biomeRanges);
            } 
        },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", (w) =>
            {
                w.GetNextValue(out var value);
                if (!ChunkGenerationNodes.SetModifierGenSample(value))
                    return w.Error("Can't find the sample specified in the modifier gen");
                return 0;
            }
        },
        {
            "range", (w) =>
            {
                if (w.GetNext2Ints(out Vector2Int ints) == -1)
                    return w.Error("no suitable ints found");
                ChunkGenerationNodes.SetModifierGenRange(ints);
                return 0;
            }
        },
        {
            "offset", (w) =>
            {
                if (w.GetNextInt(out int value) == -1)
                    return w.Error("No good int found");
                ChunkGenerationNodes.SetModifierGenOffset(value);
                return 0;
            }
        },
        {
            "flip", (w) =>
            {
                ChunkGenerationNodes.SetModifierGenFlip(true);
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
}