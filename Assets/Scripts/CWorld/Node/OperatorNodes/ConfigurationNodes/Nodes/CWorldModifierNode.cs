using System.Collections.Generic;
using UnityEngine;

public class CWorldModifierNode
{
    public ModifierRange range;
}

public class ModifierSample
{
    public CWorldSampleNode sampleNode;
    public int min;
    public int max;
}

public abstract class AbstractModifier
{
    public abstract int GetValue();
}

public class ModifierNoise : AbstractModifier
{
    public ModifierSample sample;
    public int sign;
    
    public override int GetValue()
    {
        return (int)Mathf.Lerp(sample.min, sample.max, sample.sampleNode.noiseValue) * sign;
    }
}

public class ModifierOffset : AbstractModifier
{
    public int offset;
    
    public override int GetValue()
    {
        return offset;
    }
}

public class ModifierRange
{
    public List<AbstractModifier> start;
    public List<AbstractModifier> end;

    public Vector2Int GetRange()
    {
        Vector2Int vector = new Vector2Int();

        foreach (var sample in start)
        {
            vector.x += sample.GetValue();
        }
        
        foreach (var sample in end)
        {
            vector.y += sample.GetValue();
        }
        
        return vector;
    }
}