using System.Collections.Generic;
using UnityEngine;

public class CWOSOverrideNode
{
    public float amplitude;
    public bool invert;

    public List<CWOISampleNode> add;
    public List<CWOISampleNode> multiply;
    public List<CWOISampleNode> subtract;
    
    public List<CWAParameterNode> parameters;
    
    public CWOSOverrideNode()
    {
        amplitude = 1;
        invert = false;
        
        add = new List<CWOISampleNode>();
        multiply = new List<CWOISampleNode>();
        subtract = new List<CWOISampleNode>();
        
        parameters = new List<CWAParameterNode>();
    }

    public float Apply(float height)
    {
        foreach (CWOISampleNode sample in add)
            height += sample.GetNoise();
        
        foreach (CWOISampleNode sample in multiply)
            height *= sample.GetNoise();
        
        foreach (CWOISampleNode sample in subtract)
            height -= sample.GetNoise();
        
        foreach (CWAParameterNode parameter in parameters)
            height = parameter.GetValue(height);
        
        if (invert)
            height = 1 - height;
        
        return height * amplitude;
    }
}