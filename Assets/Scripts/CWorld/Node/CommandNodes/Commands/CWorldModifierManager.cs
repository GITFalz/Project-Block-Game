using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldModifierManager
{
    public static string name;
    public static CWorldMapNode MapNode;
    
    public static void SetMap(CWorldMapNode map)
    {
        MapNode = map;
    }
    
    public static Dictionary<string, Func<Task<int>>> labels = new Dictionary<string, Func<Task<int>>>()
    {
        { "(", () => Increment(1, 0) },
        { "name", () => CWorldNodesManager.On_Name(ref name) },
        { ")", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "sample", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetModifierSample(value))
                    return await Error("Can't find the sample specified in the modifier");
                CWorldCommandManager.Increment();
                return 0;
            }
        },
        {
            "range", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out Vector2Int ints) == -1)
                    return await Error("no suitable ints found");
                await ChunkGenerationNodes.SetModifierRange(ints);
                return 0;
            }
        },
        {
            "ignore", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Error("no suitable floats found");
                await ChunkGenerationNodes.SetModifierIgnore(floats);
                return 0;
            }
        },
        {
            "invert", async () =>
            {
                await ChunkGenerationNodes.SetModifierInvert(true);
                return 0;
            }
        },
        { 
            "gen", async () =>
            {
                await ChunkGenerationNodes.AddModifierGen();
                return await CWorldNodesManager.On_Settings(biomeRanges);
            } 
        },
        { "}", () => Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> biomeRanges = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "sample", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetModifierGenSample(value))
                    return await Error("Can't find the sample specified in the modifier gen");
                Increment();
                return 0;
            }
        },
        {
            "range", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out Vector2Int ints) == -1)
                    return await Error("no suitable ints found");
                await ChunkGenerationNodes.SetModifierGenRange(ints);
                return 0;
            }
        },
        {
            "offset", async () =>
            {
                if (await CWorldCommandManager.GetNextInt(out int value) == -1)
                    return await Error("No good int found");
                await ChunkGenerationNodes.SetModifierGenOffset(value);
                return 0;
            }
        },
        {
            "flip", async () =>
            {
                await ChunkGenerationNodes.SetModifierGenFlip(true);
                Increment();
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