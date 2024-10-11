using System.Collections.Generic;
using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public List<C
    public List<CNode> nodes = new List<CNode>();
    
    public Block GetBlock(int x, int y, int z)
    {
        foreach (CNode node in nodes)
        {
            Block block = node.GetBlock(x, y, z);
            if (block != null) return block;
        }

        return null;
    }
}

public abstract class CNode
{
    public abstract Block GetBlock(int x, int y, int z);
}

public class CBiomeNode : CNode
{
    public List<CBlockHeightSequence> blocks;

    public CSampleNode mask;
    public CSampleNode sample;
    public CNoiseSettings noise;

    public float t_min;
    public float t_max;

    public float minHeight;
    public float maxHeight;

    public CBiomeNode()
    {
        t_min = 0;
        t_max = 1;
    }

    public override Block GetBlock(int x, int y, int z)
    {
        return GetBlock(x, y, z, noise.GetNoiseValue(x, z));
    }

    public Block GetBlock(int x, int y, int z, float n)
    {
        int height = Mathf.FloorToInt(Mathf.Lerp(minHeight, maxHeight, mask?.GetNoise(x, z, n) ?? n));
        
        foreach (var block in blocks)
        {
            Block b = block.IsBlock(x, y, z, height);
            if (b != null) return b;
        }
        return null;
    }
}


public abstract class CInit
{
    public abstract void Init(int x, int z);
}


public class CSampleNode : CInit
{
    public CNoiseSettings noise;
    public float noiseValue;

    public override void Init(int x, int z)
    {
        noiseValue = noise.GetNoiseValue(x, z);
    }
}

public struct CBlockHeightSequence
{
    public int minHeight;
    public int maxHeight;
    
    public int minDepth;
    public int maxDepth;
    
    public Block block;

    public Block IsBlock(int x, int y, int z, int height)
    {
        if (y < minHeight || y > maxHeight) return null;

        int minD = minDepth == -1 ? height : height - minDepth;
        int maxD = maxDepth == -1 ? minHeight : height - maxDepth;
        
        return y > minD || y < maxD ? null : block;
    }
}

public abstract class CSettings
{
    
}

public class CNoiseSettings : CSettings
{
    public float sizeX;
    public float sizeY;

    public float t_min;
    public float t_max;

    public float c_min;
    public float c_max;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;

    public CNoiseSettings()
    {
        sizeX = 20;
        sizeY = 20;
        
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        t_smooth = false;
        t_slide = false;
    }
    
    public float GetNoiseValue(int x, int z)
    {
        float height = Mathf.Clamp(Mathf.PerlinNoise((float)((float)x / sizeX + 0.001f), (float)((float)z / sizeY + 0.001f)), c_min, c_max);
        
        if (t_smooth)
            height = Mathp.PLerp(t_min, t_max, height);
        if (t_slide)
            height = Mathp.SLerp(t_min, t_max, height);
        if (invert)
            height = 1 - height;

        return height;
    }
}