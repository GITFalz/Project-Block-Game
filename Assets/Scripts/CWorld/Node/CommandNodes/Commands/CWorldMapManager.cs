using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CWorldMapManager
{
    public static CWorldMapManager instance;

    public CWorldMapManager()
    {
        if (instance == null) instance = this;
    }
    
    public Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "biomeRange", (w) => w.On_Settings(w.writerManager.worldMapManager.biomeRanges)},
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> biomeRanges = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "set", async (w) =>
            {
                w.GetNextValue(out string value);
                if (!await ChunkGenerationNodes.SetMapBiomeRange(value))
                    return await w.Error("Couldn't find biome probably");
                
                return await w.CommandsTest(w.writerManager.worldMapManager.setRanges) == -1 ? await w.Error("Problem with the settings") : 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> setRanges = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", async (w) =>
            {
                w.GetNext(out var value);
                if (await w.GetNext2Floats(out var floats) == -1)
                    return await w.Error("Not valid range u_u");
                

                if (!await ChunkGenerationNodes.SetMapSampleRange(value, floats))
                    return await w.Error("Sample may not exists when setting biome range");

                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
}