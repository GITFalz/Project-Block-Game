using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldBiomeManager
{
    public static string name;
    public static CWOCSequenceNode sequenceNode;
    
    public static Dictionary<string, Func<Task<int>>> labels = new Dictionary<string, Func<Task<int>>>()
    {
        { "(", async () => await Increment(1, 0) },
        { "name", async () => await CWorldNodesManager.On_Name(ref name) },
        { ")", async () => await Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        { "sample", async () => await CWorldNodesManager.On_Settings(samples) },
        { "modifier", async () => await CWorldNodesManager.On_Settings(modifiers) },
        { "foliage", async () => await CWorldNodesManager.On_Settings(Foliage) },
        { "sequence", async () => await CWorldNodesManager.On_Settings(sequences) },
        { "}", async () => await Increment(0, 1) },
    };

    public static Dictionary<string, Func<Task<int>>> sequences = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        { 
            "id", async () =>
            {
                if (await CWorldCommandManager.GetNextInt(out int result) == -1)
                    return await Console.LogErrorAsync("id needs to be an integer");

                sequenceNode = new CWOCSequenceNode();
                sequenceNode.block = new Block((short)result, 0);
                return 0;
            } 
        },
        { 
            "fixed", async () =>
            {
                if (await CWorldCommandManager.GetNextInt(out int value) == -1)
                    return await Console.LogErrorAsync("height must be an integer");

                sequenceNode.top_min = value;
                sequenceNode.top_max = value;
                return 0;
            } 
        },
        { 
            "set", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out Vector2Int ints) == -1)
                {
                    if (await CWorldCommandManager.GetNext2Values(out string[] values) == -1)
                        return await Console.LogErrorAsync("missing ','");

                    int result;

                    if (values[0].Equals("max") && int.TryParse(values[1], out result))
                    {
                        if (result >= 1)
                        {
                            sequenceNode.top_min = 1;
                            sequenceNode.top_max = result;
                            return 0;
                        }
                        
                        return await Console.LogErrorAsync("the second value needs to be superior or equal to max (max = 1)");
                    }
                    
                    if (int.TryParse(values[0], out var result1) && int.TryParse(values[1], out var result2))
                    {
                        if (result2 >= result1)
                        {
                            sequenceNode.top_min = result1;
                            sequenceNode.top_max = result2;
                            return 0;
                        }
                        
                        return await Console.LogErrorAsync("the first value needs to be smaller or equal to the second");
                    }
                    
                    if (int.TryParse(values[0], out result) && values[1].Equals("min"))
                    {
                        sequenceNode.top_min = result;
                        sequenceNode.top_max = 9999;
                        return 0;
                    }
                    
                    return await Console.LogErrorAsync("uhm... error");
                }
                
                sequenceNode.top_min = ints.x;
                sequenceNode.top_max = ints.y;
                return 0;
            } 
        },
        { "}", async () =>
        {
            ChunkGenerationNodes.SetBiomeSequence(sequenceNode);
            return await Increment(1, 1);
        } }
    };

    public static Dictionary<string, Func<Task<int>>> samples = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        {
            "set", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeSample(value))
                    return await Console.LogErrorAsync("Can't find the sample specified in the biome");
                Increment();
                return 0;
            }
        },
        { 
            "range", async () =>
            {
                if (await CWorldCommandManager.GetNext2Ints(out Vector2Int ints) == -1)
                    return await Console.LogErrorAsync("no suitable ints found");
                await ChunkGenerationNodes.SetBiomeRange(ints);
                return 0;
            } 
        },
        { "}", async () => await Increment(1, 1) }
    };
    
    public static Dictionary<string, Func<Task<int>>> modifiers = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        {
            "use", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeModifier(value))
                    return await Console.LogErrorAsync("Can't find the modifier specified in the biome");
                Increment();
                return 0;
            }
        },
        { "}", async () => await Increment(1, 1) }
    };
    
    public static Dictionary<string, Func<Task<int>>> Foliage = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        {
            "use", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeFoliage(value))
                    return await Console.LogErrorAsync("Can't find the tree specified in the biome"); 
                Increment();
                return 0;
            }
        },
        {
            "sample", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetBiomeTreeSample(value))
                    return await Console.LogErrorAsync("Can't find the sample specified in the biome");
                Increment();
                return 0;
            }
        },
        {
            "range", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out var values) == -1)
                    return await Console.LogErrorAsync("no suitable floats found");
                await ChunkGenerationNodes.SetBiomeTreeSampleRange(values);
                return 0;
            }
        },
        { "}", async () => await Increment(1, 1) }
    };
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }
    
    private static async Task<int> Increment(int i, int result)
    {
        return await CWorldCommandManager.Increment(i, result);
    }
}
