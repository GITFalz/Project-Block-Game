using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldSampleManager
{
    public static string name;

    public static Vector2 noiseSize = new Vector2(2, 2);
    public static Vector2 noiseOffset = new Vector2(0, 0);

    public static float noiseAmplitude = 1;
    public static bool noiseInvert = false;
    
    public static List<SampleData> noiseSampleData = new List<SampleData>();
    
    public static bool overrideInvert = false;
    
    public static List<SampleData> overrideSampleData = new List<SampleData>();
    public static List<SampleOverrideData> sampleOverrideData = new List<SampleOverrideData>();

    public static bool flip = false;
    public static int min_height = 0;
    public static int max_height = 400;

    public static void Reset()
    {
        name = "";
        
        noiseSize = new Vector2(2, 2);
        noiseOffset = new Vector2(0, 0);
        
        noiseAmplitude = 1;
        noiseInvert = false;
        
        noiseSampleData.Clear();
        
        overrideInvert = false;
        
        overrideSampleData.Clear();
        sampleOverrideData.Clear();
        
        flip = false;
        min_height = 0;
        max_height = 400;
    }
    
    public static Dictionary<string, Func<Task<int>>> labels = new Dictionary<string, Func<Task<int>>>()
    {
        { "(", async () => await Increment(1, 0) },
        { "name", () => CWorldNodesManager.On_Name(ref name) },
        { ")", () => Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        { "override", async () => await CWorldNodesManager.On_Settings(overrides) },
        { "noise", async () => await CWorldNodesManager.On_Settings(noises) },
        { "display", CWorldNodesManager.On_Display },
        { "}", () => Increment(0, 1) },
    };

    public static Dictionary<string, Func<Task<int>>> noises = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        {
            "size", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error setting noise size");
                
                noiseSize = floats;
                return 0;
            }
        },
        {
            "offset", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error setting noise offset");
                
                noiseOffset = floats;
                return 0;
            }
        },
        
        { 
            "clamp", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating clamp");

                noiseSampleData.Add(new SampleData { type = "clamp", floats = floats });
                return 0;
            } 
        },
        { 
            "lerp", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating lerp");
                
                noiseSampleData.Add(new SampleData { type = "lerp", floats = floats });
                return 0;
            } 
        },
        { 
            "slide", async () =>
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating slide");

                noiseSampleData.Add(new SampleData { type = "slide", floats = floats });
                return 0;
            }
        },
        { 
            "smooth", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                noiseSampleData.Add(new SampleData { type = "smooth", floats = floats });
                return 0;
            } 
        },
        { 
            "ignore", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                noiseSampleData.Add(new SampleData { type = "ignore", floats = floats });
                return 0;
            } 
        },

        {
            "amplitude", async () =>
            {
                if (await CWorldCommandManager.GetNextFloat(out float value) == -1)
                    return await Console.LogErrorAsync("Error creating lerp");

                noiseAmplitude = value;
                return 0;
            }
        },
        {
            "invert", async () =>
            {
                await Increment(1, 0);
                noiseInvert = true;
                return 0;
            }
        },
        { "}", () => Increment(1, 1) }
    };

    public static Dictionary<string, Func<Task<int>>> overrides = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", () => Increment(1, 0) },
        
        { 
            "add", async () =>
            {
                while (true)
                {
                    Increment();
                    sampleOverrideData.Add(new SampleOverrideData { type = OverrideType.Add, name = CWorldCommandManager.Arg });

                    Increment();
                    if (CWorldCommandManager.Arg.Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "mul", async () => 
            {
                while (true)
                {
                    Increment();
                    sampleOverrideData.Add(new SampleOverrideData { type = OverrideType.Mul, name = CWorldCommandManager.Arg });
                    
                    Increment();
                    if (CWorldCommandManager.Arg.Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "sub", async () => 
            {
                while (true)
                {
                    Increment();
                    sampleOverrideData.Add(new SampleOverrideData { type = OverrideType.Sub, name = CWorldCommandManager.Arg });

                    Increment();
                    if (CWorldCommandManager.Arg.Equals(",")) continue;
                    return 0;
                }
            } 
        },
        
        { 
            "clamp", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                overrideSampleData.Add(new SampleData { type = "clamp", floats = floats });
                return 0;
            } 
        },
        { 
            "lerp", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                overrideSampleData.Add(new SampleData { type = "lerp", floats = floats });
                return 0;
            } 
        },
        { 
            "slide", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                overrideSampleData.Add(new SampleData { type = "slide", floats = floats });
                return 0;
            } 
        },
        { 
            "smooth", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                overrideSampleData.Add(new SampleData { type = "smooth", floats = floats });
                return 0;
            } 
        },
        { 
            "ignore", async () => 
            {
                if (await CWorldCommandManager.GetNext2Floats(out Vector2 floats) == -1)
                    return await Console.LogErrorAsync("Error creating smooth");
                
                overrideSampleData.Add(new SampleData { type = "ignore", floats = floats });
                return 0;
            } 
        },

        {
            "invert", async () =>
            {
                await Increment(1, 0);
                overrideInvert = true;
                return 0;
            } 
        },
        
        { "}", () => Increment(1, 1) }
    };
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }
    
    private static Task<int> Increment(int i, int result)
    {
        return CWorldCommandManager.Increment(i, result);
    }
}

public struct SampleData
{
    public string type;
    public Vector2 floats;
}

public struct SampleOverrideData
{
    public OverrideType type;
    public string name;
}

public enum OverrideType
{
    Add,
    Mul,
    Sub
}