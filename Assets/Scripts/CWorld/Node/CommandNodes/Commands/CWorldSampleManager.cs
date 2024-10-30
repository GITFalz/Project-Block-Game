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
        { "biome", (w) => w.On_Settings(w.writerManager.worldSampleManager.biomes) },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomes = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "flip", (w) => w.On_SetTrue(ref w.writerManager.worldSampleManager.sampleNode.flip) },
        { "range", (w) =>
        {
            if (w.GetNext2Ints(out Vector2Int ints) == -1)
                return w.Error("no suitable ints found");

            w.writerManager.worldSampleManager.sampleNode.min_height = ints.x;
            w.writerManager.worldSampleManager.sampleNode.max_height = ints.y;
            return 0;
        } },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> noises = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "size", (w) => w.On_SampleNoiseSize() },
        { "offset", (w) => w.On_AssignNext2Floats(ref w.writerManager.worldSampleManager.noiseNode.offsetX, ref w.writerManager.worldSampleManager.noiseNode.offsetY) },
        { "clamp", (w) =>
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating clamp");

            w.writerManager.worldSampleManager.noiseNode.parameters.Add(new CWOPClampNode(floats.x, floats.y));
            return 0;
        } },
        { "lerp", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating lerp");

            w.writerManager.worldSampleManager.noiseNode.parameters.Add(new CWOPLerpNode(floats.x, floats.y));
            return 0;
        } },
        { "amplitude", (w) => w.On_AssignNextFloat(ref w.writerManager.worldSampleManager.noiseNode.amplitude) },
        { "slide", (w) =>
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating slide");
            
            w.writerManager.worldSampleManager.noiseNode.parameters.Add(new CWOPSlideNode(floats.x, floats.y));
            return 0;
        } },
        { "smooth", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.noiseNode.parameters.Add(new CWOPSmoothNode(floats.x, floats.y));
            return 0;
        } },
        { "ignore", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.noiseNode.parameters.Add(new CWOPIgnoreNode(floats.x, floats.y));
            return 0;
        } },
        { "invert", (w) => w.On_SetTrue(ref w.writerManager.worldSampleManager.noiseNode.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> overrides = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "add", (w) => w.On_SampleListAdd(w.writerManager.worldSampleManager.overrideNode.add) },
        { "mul", (w) => w.On_SampleListAdd(w.writerManager.worldSampleManager.overrideNode.multiply) },
        { "sub", (w) => w.On_SampleListAdd(w.writerManager.worldSampleManager.overrideNode.subtract) },
        { "clamp", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.overrideNode.parameters.Add(new CWOPClampNode(floats.x, floats.y));
            return 0;
        } },
        { "lerp", (w) => {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.overrideNode.parameters.Add(new CWOPLerpNode(floats.x, floats.y));
            return 0;
        } },
        { "slide", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.overrideNode.parameters.Add(new CWOPSlideNode(floats.x, floats.y));
            return 0;
        } },
        { "smooth", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.overrideNode.parameters.Add(new CWOPSmoothNode(floats.x, floats.y));
            return 0;
        } },
        { "ignore", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            w.writerManager.worldSampleManager.overrideNode.parameters.Add(new CWOPIgnoreNode(floats.x, floats.y));
            return 0;
        } },
        { "invert", (w) => w.On_SetTrue(ref w.writerManager.worldSampleManager.overrideNode.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };
}
