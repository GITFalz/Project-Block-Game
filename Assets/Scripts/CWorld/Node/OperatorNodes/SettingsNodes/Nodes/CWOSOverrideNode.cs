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

    public CWOSOverrideNode Copy(CWorldDataHandler handler)
    {
        CWOSOverrideNode overrideNode = new CWOSOverrideNode
        {
            amplitude = amplitude,
            invert = invert,
            parameters = parameters
        };

        foreach (var sample in add)
        {
            if (handler.sampleNodes.TryGetValue(sample.name, out var s))
            {
                overrideNode.add.Add(s);
            }
            else
            {
                CWorldSampleNode newSample = sample.Copy(handler);
                overrideNode.add.Add(newSample);
                handler.sampleNodes.Add(newSample.name, newSample);
            }
        }
        
        foreach (var sample in multiply)
        {
            if (handler.sampleNodes.TryGetValue(sample.name, out var s))
            {
                overrideNode.multiply.Add(s);
            }
            else
            {
                CWorldSampleNode newSample = sample.Copy(handler);
                overrideNode.multiply.Add(newSample);
                handler.sampleNodes.Add(newSample.name, newSample);
            }
        }
        
        foreach (var sample in subtract)
        {
            if (handler.sampleNodes.TryGetValue(sample.name, out var s))
            {
                overrideNode.subtract.Add(s);
            }
            else
            {
                CWorldSampleNode newSample = sample.Copy(handler);
                overrideNode.subtract.Add(newSample);
                handler.sampleNodes.Add(newSample.name, newSample);
            }
        }

        return overrideNode;
    }
}