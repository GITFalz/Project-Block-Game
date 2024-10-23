using System.Collections.Generic;
using UnityEngine;

public class CWOSNoiseNode
{
    public float sizeX;
    public float sizeY;

    public float amplitude;
    public bool invert;
    
    public List<CWAParameterNode> parameters;

    public CWOSNoiseNode()
    {
        sizeX = 0;
        sizeY = 0;

        amplitude = 1;
        invert = false;
        
        parameters = new List<CWAParameterNode>();
    }
    
    public float GetNoiseValue(int x, int z)
    {
        float height = Mathf.PerlinNoise((float)((float)x / sizeX + 0.001f), (float)((float)z / sizeY + 0.001f));
        
        if (invert)
            height = 1 - height;

        foreach (CWAParameterNode parameter in parameters)
            height = parameter.GetValue(height);
        
        return height * amplitude;
    }
}