using System;
using System.Collections.Generic;
using UnityEngine;

public class CWorldSampleNode : CWorldAbstractNode
{
    public static CWorldSampleNode instance;

    public WMWriter writer;
    public CWOISampleNode sampleNode;
    public CWOSNoiseNode noiseNode;
    public CWOSOverrideNode overrideNode;

    public CWorldSampleNode() { if (instance == null) instance = this; }

    public void SetSample(CWOISampleNode sample)
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
        { "override", (w) => w.On_Settings(instance.overrides) },
        { "noise", (w) => w.On_Settings(instance.noises) },
        { "display", (w) => w.On_Display() },
        { "biome", (w) => w.On_Settings(instance.biomes) },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomes = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "flip", (w) => w.On_SetTrue(ref instance.sampleNode.flip) },
        { "range", (w) =>
        {
            if (w.GetNext2Ints(out Vector2Int ints) == -1)
                return w.Error("no suitable ints found");

            instance.sampleNode.min_height = ints.x;
            instance.sampleNode.max_height = ints.y;
            return 0;
        } },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> noises = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "size", (w) => w.On_SampleNoiseSize() },
        { "clamp", (w) =>
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating clamp");

            instance.noiseNode.parameters.Add(new CWOPClampNode(floats.x, floats.y));
            return 0;
        } },
        { "lerp", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating lerp");

            instance.noiseNode.parameters.Add(new CWOPLerpNode(floats.x, floats.y));
            return 0;
        } },
        { "amplitude", (w) => w.On_AssingNextFloat(ref instance.noiseNode.amplitude) },
        { "slide", (w) =>
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating slide");
            
            instance.noiseNode.parameters.Add(new CWOPSlideNode(floats.x, floats.y));
            return 0;
        } },
        { "smooth", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.noiseNode.parameters.Add(new CWOPSmoothNode(floats.x, floats.y));
            return 0;
        } },
        { "ignore", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.noiseNode.parameters.Add(new CWOPIgnoreNode(floats.x, floats.y));
            return 0;
        } },
        { "invert", (w) => w.On_SetTrue(ref instance.noiseNode.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> overrides = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "add", (w) => w.On_SampleListAdd(instance.overrideNode.add) },
        { "mul", (w) => w.On_SampleListAdd(instance.overrideNode.multiply) },
        { "sub", (w) => w.On_SampleListAdd(instance.overrideNode.subtract) },
        { "clamp", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.overrideNode.parameters.Add(new CWOPClampNode(floats.x, floats.y));
            return 0;
        } },
        { "lerp", (w) => {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.overrideNode.parameters.Add(new CWOPLerpNode(floats.x, floats.y));
            return 0;
        } },
        { "slide", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.overrideNode.parameters.Add(new CWOPSlideNode(floats.x, floats.y));
            return 0;
        } },
        { "smooth", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.overrideNode.parameters.Add(new CWOPSmoothNode(floats.x, floats.y));
            return 0;
        } },
        { "ignore", (w) => 
        {
            if (w.GetNext2Floats(out Vector2 floats) == -1)
                return w.Error("Error creating smooth");
            
            instance.overrideNode.parameters.Add(new CWOPIgnoreNode(floats.x, floats.y));
            return 0;
        } },
        { "invert", (w) => w.On_SetTrue(ref instance.overrideNode.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };
}
