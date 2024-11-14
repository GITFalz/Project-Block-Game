using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldLinkManager
{
    public static string currentLink;
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref currentLink) },
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
        { "}", (w) => w.Increment(1, 1) },
    };
}