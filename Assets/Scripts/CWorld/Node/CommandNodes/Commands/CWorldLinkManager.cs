using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldLinkManager
{
    public static string name;
    public static Action<Vector2Int> LinkPointRange;
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref name) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "A", async (w) => await w.On_Settings(A)
        },
        { 
            "B", async (w) => await w.On_Settings(B)
        },
        {
            "base", async (w) => await w.On_Settings(Base)
        },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> A = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "xyz", async (w) =>
            {
                
                if (await w.GetNextNInts(3, out var ints) == -1)
                    return await w.Error("Error setting A xyz");

                Vector3Int position = new Vector3Int(ints[0], ints[1], ints[2]);
                ChunkGenerationNodes.SetLinkAPosition(position);
                return 0;
            }
            
        },
        { 
            "link", async (w) => await w.On_Settings(Alink)
        },
        { 
            "x", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAxRange;
                return await w.On_Settings(Point);
            }
        },
        { 
            "y", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAyRange;
                return await w.On_Settings(Point);
            }
        },
        { 
            "z", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkAzRange;
                return await w.On_Settings(Point);
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> B = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "xyz", async (w) =>
            {
                    
                if (await w.GetNextNInts(3, out var ints) == -1)
                    return await w.Error("Error setting B xyz");

                Vector3Int position = new Vector3Int(ints[0], ints[1], ints[2]);
                ChunkGenerationNodes.SetLinkBPosition(position);
                return 0;
            } 
        },
        { 
            "x", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkBxRange;
                return await w.On_Settings(Point);
            }
        },
        { 
            "y", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkByRange;
                return await w.On_Settings(Point);
            }
        },
        { 
            "z", async (w) =>
            {
                LinkPointRange = ChunkGenerationNodes.SetLinkBzRange;
                return await w.On_Settings(Point);
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> Base = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "x", async (w) => await w.On_Settings(Point)
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> Point = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "range", async (w) =>
            {
                w.GetNextValue(out string value);

                Vector2Int ints;

                if (value.Equals("internal"))
                {
                    if (await w.GetNext2Ints(out ints) == -1)
                        return Console.LogError("Error setting Base x, values must be intergers");
                }
                else
                {
                    w.Increment(-1);
                }
                
                if (await w.GetNext2Ints(out ints) == -1)
                    return Console.LogError("Error setting Base x, values must be intergers");

                LinkPointRange(ints);

                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> Alink = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { 
            "set", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetLinkLink(value))
                {
                    return Console.LogError("Link not found");
                }
                
                w.Increment();
                return 0;
            }
        },
        { 
            "threshold", async (w) =>
            {
                await w.GetNextFloat(out var value);
                await ChunkGenerationNodes.SetLinkThreshold(value);
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) },
    };
}