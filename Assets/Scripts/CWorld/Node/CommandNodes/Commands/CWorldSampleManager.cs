using System;
using System.Collections.Generic;
using UnityEngine;

public class CWorldSampleManager : CWorldAbstractNode
{
    public static CWorldSampleManager instance;

    public WMWriter writer;
    public CWorldSampleNode sampleNode;
    public CWorldNoiseNode noiseNode;
    public CWOSOverrideNode overrideNode;

    public CWorldSampleManager() { if (instance == null) instance = this; }

    public void SetSample(CWorldSampleNode sample)
    {
        sampleNode = sample;
        noiseNode = sample.noiseNode;
        overrideNode = sample.overrideNode;
    }
    
    public Dictionary<string, Func<WMWriter, int>> labels = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_SampleName() },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> settings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "override", (w) => w.On_Settings(w.writerManager.worldSampleManager.overrides) },
        { "noise", (w) => w.On_Settings(w.writerManager.worldSampleManager.noises) },
        { "display", (w) => w.On_Display() },
        { "}", (w) => w.Increment(0, 1) },
    };

    public Dictionary<string, Func<WMWriter, int>> noises = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        {
            "size", (w) =>
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error setting noise size");
                
                ChunkGenerationNodes.SetSampleNoiseSize(floats);
                return 0;
            }
        },
        {
            "offset", (w) =>
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error setting noise offset");
                
                ChunkGenerationNodes.SetSampleNoiseOffset(floats);
                return 0;
            }
        },
        
        { 
            "clamp", (w) =>
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating clamp");

                return !ChunkGenerationNodes.AddSampleNoiseParameter("clamp", floats) ? w.Error("Couldn't add clamp parameter") : 0;
            } 
        },
        { 
            "lerp", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating lerp");

                return !ChunkGenerationNodes.AddSampleNoiseParameter("lerp", floats) ? w.Error("Couldn't add lerp parameter") : 0;
            } 
        },
        { 
            "slide", (w) =>
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating slide");
                
                return !ChunkGenerationNodes.AddSampleNoiseParameter("slide", floats) ? w.Error("Couldn't add slide parameter") : 0;
            } 
        },
        { 
            "smooth", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleNoiseParameter("smooth", floats) ? w.Error("Couldn't add smooth parameter") : 0;
            } 
        },
        { 
            "ignore", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleNoiseParameter("ignore", floats) ? w.Error("Couldn't add ignore parameter") : 0;
            } 
        },

        {
            "amplitude", (w) =>
            {
                if (w.GetNextFloat(out float value) == -1)
                    return w.Error("Error creating lerp");

                ChunkGenerationNodes.SetSampleNoiseAmplitude(value);
                return 0;
            }
        },
        {
            "invert", (w) =>
            {
                w.Increment(1, 0);
                ChunkGenerationNodes.SetSampleNoiseInvert();
                return 0;
            }
        },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> overrides = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        
        { 
            "add", (w) =>
            {
                while (true)
                {
                    w.writerManager.index++;
                    if (!ChunkGenerationNodes.AddSampleOverrideAdd(w.writerManager.lines[w.writerManager.index]))
                        return w.Error("Couldn't find sample");

                    w.writerManager.index++;
                    if (w.writerManager.lines[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "mul", (w) => 
            {
                while (true)
                {
                    w.writerManager.index++;
                    if (!ChunkGenerationNodes.AddSampleOverrideMultiply(w.writerManager.lines[w.writerManager.index]))
                        return w.Error("Couldn't find sample");

                    w.writerManager.index++;
                    if (w.writerManager.lines[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        { 
            "sub", (w) => 
            {
                while (true)
                {
                    w.writerManager.index++;
                    if (!ChunkGenerationNodes.AddSampleOverrideSubtract(w.writerManager.lines[w.writerManager.index]))
                        return w.Error("Couldn't find sample");

                    w.writerManager.index++;
                    if (w.writerManager.lines[w.writerManager.index].Equals(",")) continue;
                    return 0;
                }
            } 
        },
        
        { 
            "clamp", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleOverrideParameter("clamp", floats) ? w.Error("Couldn't add clamp parameter") : 0;
            } 
        },
            { 
                "lerp", (w) => 
                {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleOverrideParameter("lerp", floats) ? w.Error("Couldn't add lerp parameter") : 0;
            } 
        },
        { 
            "slide", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleOverrideParameter("slide", floats) ? w.Error("Couldn't add slide parameter") : 0;
            } 
        },
        { 
            "smooth", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleOverrideParameter("smooth", floats) ? w.Error("Couldn't add smooth parameter") : 0;
            } 
        },
        { 
            "ignore", (w) => 
            {
                if (w.GetNext2Floats(out Vector2 floats) == -1)
                    return w.Error("Error creating smooth");
                
                return !ChunkGenerationNodes.AddSampleOverrideParameter("ignore", floats) ? w.Error("Couldn't add ignore parameter") : 0;
            } 
        },

        {
            "invert", (w) =>
            {
                w.Increment(1, 0);
                ChunkGenerationNodes.SetSampleOverrideInvert();
                return 0;
            } 
        },
        
        { "}", (w) => w.Increment(1, 1) }
    };
}
