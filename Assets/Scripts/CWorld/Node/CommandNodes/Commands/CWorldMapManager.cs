using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldMapManager
{
    
    public static Dictionary<string, Func<Task<int>>> settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { "biomeRange", () => CWorldNodesManager.On_Settings(biomeRanges)},
        { "}", () => Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> biomeRanges = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "set", async () =>
            {
                CWorldCommandManager.GetNextValue(out string value);
                if (!await ChunkGenerationNodes.SetMapBiomeRange(value))
                {
                    Console.Log("Couldn't find biome probably");
                    return -1;
                }

                Increment();
                
                return await CWorldCommandManager.CommandsTest(setRanges) == -1 ? await Error("Problem with the settings") : 0;
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> setRanges = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "sample", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (await CWorldCommandManager.GetNext2Floats(out var floats) == -1)
                    return await Error("Not valid range u_u");
                

                if (!await ChunkGenerationNodes.SetMapSampleRange(value, floats))
                    return await Error("Sample may not exists when setting biome range");

                return 0;
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }

    private static async Task<int> Increment(int i, int result)
    {
        return await CWorldCommandManager.Increment(i, result);
    }
    
    private static async Task<int> Error(string message)
    {
        return await Console.LogErrorAsync(message);
    }
}