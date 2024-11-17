using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldSampleManager
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
        { "override", (w) => w.On_Settings(overrides) },
        { "noise", (w) => w.On_Settings(noises) },
        { "display", (w) => w.On_Display() },
        { "}", (w) => w.Increment(0, 1) },
    };

    public static Dictionary<string, Func<WMWriter, Task<int>>> noises = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "size", async (w) =>
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error setting noise size");
                
                await ChunkGenerationNodes.SetSampleNoiseSize(floats);
                return 0;
            }
        },
        {
            "offset", async (w) =>
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error setting noise offset");
                
                await ChunkGenerationNodes.SetSampleNoiseOffset(floats);
                return 0;
            }
        },
        
        { 
            "clamp", async (w) =>
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating clamp");

                return !await ChunkGenerationNodes.AddSampleNoiseParameter("clamp", floats) ? await w.Error("Couldn't add clamp parameter") : 0;
            } 
        },
        { 
            "lerp", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating lerp");

                return !await ChunkGenerationNodes.AddSampleNoiseParameter("lerp", floats) ? await w.Error("Couldn't add lerp parameter") : 0;
            } 
        },
        { 
            "slide", async (w) =>
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating slide");
                
                return !await ChunkGenerationNodes.AddSampleNoiseParameter("slide", floats) ? await w.Error("Couldn't add slide parameter") : 0;
            } 
        },
        { 
            "smooth", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleNoiseParameter("smooth", floats) ? await w.Error("Couldn't add smooth parameter") : 0;
            } 
        },
        { 
            "ignore", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleNoiseParameter("ignore", floats) ? await w.Error("Couldn't add ignore parameter") : 0;
            } 
        },

        {
            "amplitude", async (w) =>
            {
                if (await w.GetNextFloat(out float value) == -1)
                    return await w.Error("Error creating lerp");

                await ChunkGenerationNodes.SetSampleNoiseAmplitude(value);
                return 0;
            }
        },
        {
            "invert", async (w) =>
            {
                await w.Increment(1, 0);
                await ChunkGenerationNodes.SetSampleNoiseInvert();
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) }
    };

    public static Dictionary<string, Func<WMWriter, Task<int>>> overrides = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        
        { 
            "add", async (w) =>
            {
                while (true)
                {
                    w.Increment();
                    if (!await ChunkGenerationNodes.AddSampleOverrideAdd(w.Arg))
                        return await w.Error("Couldn't find sample");

                    w.Increment();
                    if (w.writerManager.args[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "mul", async (w) => 
            {
                while (true)
                {
                    w.Increment();
                    if (!await ChunkGenerationNodes.AddSampleOverrideMultiply(w.Arg))
                        return await w.Error("Couldn't find sample");

                    w.Increment();
                    if (w.writerManager.args[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "sub", async (w) => 
            {
                while (true)
                {
                    w.Increment();
                    if (!await ChunkGenerationNodes.AddSampleOverrideSubtract(w.Arg))
                        return await w.Error("Couldn't find sample");

                    w.Increment();
                    if (w.writerManager.args[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        
        { 
            "clamp", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleOverrideParameter("clamp", floats) ? await w.Error("Couldn't add clamp parameter") : 0;
            } 
        },
            { 
                "lerp", async (w) => 
                {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleOverrideParameter("lerp", floats) ? await w.Error("Couldn't add lerp parameter") : 0;
            } 
        },
        { 
            "slide", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleOverrideParameter("slide", floats) ? await w.Error("Couldn't add slide parameter") : 0;
            } 
        },
        { 
            "smooth", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleOverrideParameter("smooth", floats) ? await w.Error("Couldn't add smooth parameter") : 0;
            } 
        },
        { 
            "ignore", async (w) => 
            {
                if (await w.GetNext2Floats(out Vector2 floats) == -1)
                    return await w.Error("Error creating smooth");
                
                return !await ChunkGenerationNodes.AddSampleOverrideParameter("ignore", floats) ? await w.Error("Couldn't add ignore parameter") : 0;
            } 
        },

        {
            "invert", async (w) =>
            {
                await w.Increment(1, 0);
                await ChunkGenerationNodes.SetSampleOverrideInvert();
                return 0;
            } 
        },
        
        { "}", (w) => w.Increment(1, 1) }
    };
}
