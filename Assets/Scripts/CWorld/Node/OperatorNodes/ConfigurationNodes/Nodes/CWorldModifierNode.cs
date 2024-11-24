using System.Collections.Generic;
using UnityEngine;

public class CWorldModifierNode
{
    public string name;
    
    public CWorldSampleNode sample;
    public IntRangeNode range;
    public FloatRangeNode ignore;
    public List<CWorldModifierGenNode> gen;
    public bool invert;

    public CWorldModifierNode(string name)
    {
        this.name = name;
        range = new IntRangeNode();
        gen = new List<CWorldModifierGenNode>();
        ignore = null;
        invert = false;
    }

    public int GetMaxHeight()
    {
        return (int)Mathf.Clamp(Mathf.Lerp(range.min, range.max, sample.noiseValue), range.min, range.max);
    }
}

public class IntRangeNode
{
    public int min;
    public int max;

    public IntRangeNode(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
    
    public IntRangeNode(Vector2Int range)
    {
        min = range.x;
        max = range.y;
    }

    public IntRangeNode()
    {
        min = 0;
        max = WorldInfo.worldMaxTerrainHeight;
    }
    
    public static IntRangeNode operator +(IntRangeNode a, int offset)
    {
        return new IntRangeNode(a.min + offset, a.max + offset);
    }
}

public class FloatRangeNode
{
    public float min;
    public float max;

    public FloatRangeNode(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    
    public FloatRangeNode(Vector2 range)
    {
        min = range.x;
        max = range.y;
    }

    public FloatRangeNode()
    {
        min = 0;
        max = 1;
    }
}

public class CWorldModifierGenNode
{
    public CWorldSampleNode sample;
    public IntRangeNode range;
    public int offset = 0;
    public bool flip = false;

    public int GetHeight(CWorldModifierNode parent)
    {
        if (parent.ignore != null && parent.sample.noiseValue >= parent.ignore.min && parent.sample.noiseValue <= parent.ignore.max)
            return 0;
        if (sample.noiseValue < -0.5f)
            return -1;
        
        return (int)Mathf.Clamp(Mathf.Lerp(range.min, range.max,  sample.noiseValue), range.min, range.max);
    }
}