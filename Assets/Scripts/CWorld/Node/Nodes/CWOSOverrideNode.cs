using System.Collections.Generic;
using UnityEngine;

public class CWOSOverrideNode
{
    public float amplitude;
    public bool invert;

    public List<IModifier> modifiers;
    
    public List<CWAParameterNode> parameters;
    
    public CWOSOverrideNode()
    {
        amplitude = 1;
        invert = false;
        
        modifiers = new List<IModifier>();
        
        parameters = new List<CWAParameterNode>();
    }

    public float Apply(float height)
    {
        foreach (IModifier sample in modifiers)
            height = sample.GetModifier(height);
        
        foreach (CWAParameterNode parameter in parameters)
            height = parameter.GetValue(height);
        
        if (invert)
            height = 1 - height;
        
        return height * amplitude;
    }
}

public interface IModifier
{
    float GetModifier(float height);
    CWorldSampleNode GetSample();
}

public class AddModifier : IModifier
{
    public CWorldSampleNode sample;

    public float GetModifier(float height)
    {
        if (sample == null)
            return -1;
        
        return height + sample.noiseValue;
    }
    
    public CWorldSampleNode GetSample()
    {
        return sample;
    }
}

public class MultiplyModifier : IModifier
{
    public CWorldSampleNode sample;

    public float GetModifier(float height)
    {
        if (sample == null)
            return -1;
        
        return height * sample.noiseValue;
    }
    
    public CWorldSampleNode GetSample()
    {
        return sample;
    }
}

public class SubtractModifier : IModifier
{
    public CWorldSampleNode sample;

    public float GetModifier(float height)
    {
        if (sample == null)
            return -1;
        
        return height - sample.noiseValue;
    }
    
    public CWorldSampleNode GetSample()
    {
        return sample;
    }
}