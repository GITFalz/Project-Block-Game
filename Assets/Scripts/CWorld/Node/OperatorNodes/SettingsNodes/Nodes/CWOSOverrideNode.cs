using System.Collections.Generic;
using UnityEngine;

public class CWOSOverrideNode
{
    public float amplitude;
    public bool invert;

    public List<CWorldSampleNode> add;
    public List<CWorldSampleNode> multiply;
    public List<CWorldSampleNode> subtract;
    
    public List<CWAParameterNode> parameters;
    
    public CWOSOverrideNode()
    {
        amplitude = 1;
        invert = false;
        
        add = new List<CWorldSampleNode>();
        multiply = new List<CWorldSampleNode>();
        subtract = new List<CWorldSampleNode>();
        
        parameters = new List<CWAParameterNode>();
    }

    public float Apply(float height)
    {
        foreach (CWorldSampleNode sample in add)
            height += sample.noiseValue;
        
        foreach (CWorldSampleNode sample in multiply)
            height *= sample.noiseValue;
        
        foreach (CWorldSampleNode sample in subtract)
            height -= sample.noiseValue;
        
        foreach (CWAParameterNode parameter in parameters)
            height = parameter.GetValue(height);
        
        if (invert)
            height = 1 - height;
        
        return height * amplitude;
    }
}