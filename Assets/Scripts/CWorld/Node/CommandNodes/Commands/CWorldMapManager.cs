using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldMapManager
{
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "biomeRange", (w) => w.On_Settings(biomeRanges)},
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> biomeRanges = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "set", async (w) =>
            {
                w.GetNextValue(out string value);
                if (!await ChunkGenerationNodes.SetMapBiomeRange(value))
                {
                    Console.Log("Couldn't find biome probably");
                    return -1;
                }

                w.Increment();
                
                return await w.CommandsTest(setRanges) == -1 ? await w.Error("Problem with the settings") : 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> setRanges = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "sample", async (w) =>
            {
                w.GetNextValue(out var value);
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