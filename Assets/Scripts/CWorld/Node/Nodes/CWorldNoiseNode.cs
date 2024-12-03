using System.Collections.Generic;
using UnityEngine;

public class CWorldNoiseNode
{
    public float sizeX;
    public float sizeY;

    public float offsetX;
    public float offsetY;

    public float amplitude;
    public bool invert;
    
    public List<CWAParameterNode> parameters;

    public CWorldNoiseNode()
    {
        sizeX = 100;
        sizeY = 100;

        offsetX = 0.001f;
        offsetY = 0.001f;

        amplitude = 1;
        invert = false;
        
        parameters = new List<CWAParameterNode>();
    }

    public float GetNoiseValue(float x, float z)
    {
        float height = Mathf.PerlinNoise((float)((float)x / sizeX + offsetX), (float)((float)z / sizeY + offsetY));

        foreach (CWAParameterNode parameter in parameters)
            height = parameter.GetValue(height);
        
        if (invert)
            height = 1 - height;
        
        if (float.IsNaN(height))
            return 0;
        
        return height * amplitude;
    }
    
    public float GetNoiseValue(int x, int z)
    {
        return GetNoiseValue((float)x, (float)z);
    }
}