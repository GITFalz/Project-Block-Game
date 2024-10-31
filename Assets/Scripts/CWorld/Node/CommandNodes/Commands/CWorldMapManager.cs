using System;
using System.Collections.Generic;
using UnityEngine;

public class CWorldMapManager
{
    public static CWorldMapManager instance;
    public CWorldMapNode MapNode;

    public CWorldMapManager()
    {
        if (instance == null) instance = this;
    }
    
    public void SetMap(CWorldMapNode map)
    {
        MapNode = map;
    }
    
    public Dictionary<string, Func<WMWriter, int>> settings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "biomeRange", (w) => w.On_Settings(w.writerManager.worldMapManager.biomeRanges)},
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "set", (w) =>
            {
                w.GetNextValue(out string value);
                if (!ChunkGenerationNodes.SetMapBiomeRange(value))
                    return w.Error("Couldn't find biome probably");
                
                return w.CommandsTest(w.writerManager.worldMapManager.setRanges) == -1 ? w.Error("Problem with the settings") : 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> setRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", (w) =>
            {
                w.GetNext(out var value);
                if (w.GetNext2Floats(out var floats) == -1)
                    return w.Error("Not valid range u_u");
                

                if (!ChunkGenerationNodes.SetMapSampleRange(value, floats))
                    return w.Error("Sample may not exists when setting biome range");

                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
}