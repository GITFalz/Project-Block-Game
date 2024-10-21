using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CWorldSampleNode : CWorldAbstractNode
{
    public static CWorldSampleNode instance;

    public WMWriter writer;
    public CSampleNode sampleNode;
    public CNoiseNode noise;
    public COverrideNode overide;

    public CWorldSampleNode() { if (instance == null) instance = this; }

    public void SetSample(CSampleNode sample)
    {
        sampleNode = sample;
        noise = sample.noise;
        overide = sample.overRide;
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
        { "}", (w) => w.Increment(0, 1) },
    };

    public Dictionary<string, Func<WMWriter, int>> noises = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "size", (w) => w.On_SampleNoiseSize() },
        { "threshold", (w) => w.On_AssingNext2Floats(ref instance.noise.t_min, ref instance.noise.t_max) },
        { "clamp", (w) => w.On_AssingNext2Floats(ref instance.noise.c_min, ref instance.noise.c_max) },
        { "lerp", (w) => {
                if (w.On_AssingNext2Floats(ref instance.noise.l_min, ref instance.noise.l_max) == -1)
                    return w.Error("Error at Sample > noise > lerp");
                
                instance.noise.lerp = true;
                return 0; } },
        { "amplitude", (w) => w.On_AssingNextFloat(ref instance.noise.amplitude) },
        { "slide", (w) => w.On_SetTrue(ref instance.noise.t_slide) },
        { "smooth", (w) => w.On_SetTrue(ref instance.noise.t_smooth) },
        { "invert", (w) => w.On_SetTrue(ref instance.noise.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };

    public Dictionary<string, Func<WMWriter, int>> overrides = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "add", (w) => w.On_SampleListAdd(instance.sampleNode.add) },
        { "mul", (w) => w.On_SampleListAdd(instance.sampleNode.mul) },
        { "sub", (w) => w.On_SampleListAdd(instance.sampleNode.sub) },
        { "threshold", (w) => w.On_AssingNext2Floats(ref instance.overide.t_min, ref instance.overide.t_max) },
        { "clamp", (w) => w.On_AssingNext2Floats(ref instance.overide.c_min, ref instance.overide.c_max) },
        { "lerp", (w) => {
            if (w.On_AssingNext2Floats(ref instance.overide.l_min, ref instance.overide.l_max) == -1)
                return w.Error("Error at Sample > noise > lerp");
                
            instance.overide.lerp = true;
            return 0; } },
        { "slide", (w) => w.On_SetTrue(ref instance.overide.t_slide) },
        { "smooth", (w) => w.On_SetTrue(ref instance.overide.t_smooth) },
        { "invert", (w) => w.On_SetTrue(ref instance.overide.invert) },
        { "}", (w) => w.Increment(1, 1) }
    };
}
