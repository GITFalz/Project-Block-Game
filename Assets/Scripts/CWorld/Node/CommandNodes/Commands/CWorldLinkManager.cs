using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldLinkManager
{
    public static string name;
    public static Action<Vector2Int> LinkPointRange;
    
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
            "A", async () => await CWorldNodesManager.On_Settings(A)
        },
        { 
            "B", async () => await CWorldNodesManager.On_Settings(B)
        },
        {
            "base", async () => await CWorldNodesManager.On_Settings(Base)
        },
        { "}", () => Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> A = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => CWorldCommandManager.Increment(1, 0) },
        {
            "xyz", async () =>
            {
                
                if (await CWorldCommandManager.GetNextNInts(3, out var ints) == -1)
                    return await Error("Error setting A xyz");

                Vector3Int position = new Vector3Int(ints[0], ints[1], ints[2]);
                ChunkGenerationNodes.SetLinkAPosition(position);
                return 0;
            }
            
        },
        { 
            "link", async () => await CWorldNodesManager.On_Settings(Alink)
        },
        { 
            "x", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAxRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { 
            "y", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAyRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { 
            "z", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAzRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> B = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "xyz", async () =>
            {
                    
                if (await CWorldCommandManager.GetNextNInts(3, out var ints) == -1)
                    return await Error("Error setting B xyz");

                Vector3Int position = new Vector3Int(ints[0], ints[1], ints[2]);
                ChunkGenerationNodes.SetLinkBPosition(position);
                return 0;
            } 
        },
        { 
            "x", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkBxRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { 
            "y", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkByRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { 
            "z", async () =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkBzRange;
                return await CWorldNodesManager.On_Settings(Point);
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> Base = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "x", async () => await CWorldNodesManager.On_Settings(Point)
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> Point = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "range", async () =>
            {
                CWorldCommandManager.GetNextValue(out string value);

                Vector2Int ints;

                if (value.Equals("internal"))
                {
                    if (await CWorldCommandManager.GetNext2Ints(out ints) == -1)
                        return Console.LogError("Error setting Base x, values must be intergers");
                }
                else
                {
                    Increment(-1);
                }
                
                if (await CWorldCommandManager.GetNext2Ints(out ints) == -1)
                    return Console.LogError("Error setting Base x, values must be intergers");

                LinkPointRange(ints);

                return 0;
            }
        },
        { "}", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> Alink = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { 
            "set", async () =>
            {
                CWorldCommandManager.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetLinkLink(value))
                {
                    return Console.LogError("Link not found");
                }
                
                Increment();
                return 0;
            }
        },
        { 
            "threshold", async () =>
            {
                await CWorldCommandManager.GetNextFloat(out var value);
                await ChunkGenerationNodes.SetLinkThreshold(value);
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