
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class CWorldTreeManager
{
    public static string name;
    
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
            "sample", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetTreeSample(value))
                {
                    Console.Log("Sample not found, trying to set as modifier");
                    if (!await ChunkGenerationNodes.SetTreeModifier(value))
                    {
                        Console.Log("Modifier not found, setting as basic");
                        await ChunkGenerationNodes.SetTreeBasic();
                    }
                }
                
                w.Increment(); //thanks! ^w^
                return 0;
            }
        },
        {
            "range", async (w) =>
            {
                if (await w.GetNext2Ints(out var value) == -1)
                {
                    Console.Log("Error: range needs to be two integers");
                    return -1;
                }
                
                await ChunkGenerationNodes.SetTreeRange(value);
                return 0;
            }
        },
        {
            "link", async (w) =>
            {
                w.GetNextValue(out var value);
                if (!await ChunkGenerationNodes.SetTreeLink(value))
                {
                    return Console.LogError("Link not found");
                }
                
                w.Increment();
                return 0;
            }
        },
        //{ "link", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> link = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        //{ "sample", async (w) => },
        //{ "link", async (w) => },
        //{ "range", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
}