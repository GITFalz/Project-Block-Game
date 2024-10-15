using System.Collections.Generic;
using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public List<CInit> initializers = new List<CInit>();
    public List<CExecute> executes = new List<CExecute>();
    
    public Block GetBlock(int x, int y, int z)
    {
        foreach (CExecute execute in executes)
        {
            Block block = execute.GetBlock(x, y, z);
            if (block != null) return block;
        }

        return null;
    }
}

public abstract class CNode
{
    
}

public abstract class CExecute : CNode
{
    public abstract Block GetBlock(int x, int y, int z);
}

public class CBiomeNode : CExecute
{
    public List<CBlockHeightSequence> blocks;

    public CSampleNode mask;
    public CSampleNode sample;
    public CNoiseNode noise;

    public float t_min;
    public float t_max;

    public float minHeight;
    public float maxHeight;

    public CBiomeNode()
    {
        t_min = 0;
        t_max = 1;

        mask = new CSampleNode();
        sample = new CSampleNode();
        noise = new CNoiseNode();
    }

    public override Block GetBlock(int x, int y, int z)
    {
        return GetBlock(x, y, z, noise.GetNoiseValue(x, z));
    }

    public Block GetBlock(int x, int y, int z, float n)
    {
        //int height = Mathf.FloorToInt(Mathf.Lerp(minHeight, maxHeight, mask?.GetNoise(x, z, n) ?? n));
        
        foreach (var block in blocks)
        {
            //Block b = block.IsBlock(x, y, z, height);
            //if (b != null) return b;
        }
        return null;
    }
}


public abstract class CInit : CNode
{
    public abstract void Init(int x, int z);
}


public class CSampleNode : CInit
{
    public List<CSampleNode> add;
    public CSampleNode mask;
    public COverrideNode oride;
    public CNoiseNode noise;
    public float noiseValue;

    public CSampleNode()
    {
        mask = null;
        add = new List<CSampleNode>();
        
        noise = new CNoiseNode();
        noiseValue = 0;
    }

    public override void Init(int x, int z)
    {
        noiseValue = noise.GetNoiseValue(x, z);
    }

    public float GetNoise(int x, int z)
    {
        float height = noise.GetNoiseValue(x, z);

        if (add.Count > 0)
        {
            foreach (CSampleNode node in add)
            {
                height += node.GetNoise(x, z);
            }
        }
        
        return height;
    }
}

public class COverrideNode
{
    public float t_min;
    public float t_max;

    public float c_min;
    public float c_max;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;
    
    public COverrideNode()
    {
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        t_smooth = false;
        t_slide = false;
        invert = false;
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

public abstract class CSettings : CNode
{
    
}

public class CNoiseNode : CSettings
{
    public float sizeX;
    public float sizeY;

    public float t_min;
    public float t_max;

    public float c_min;
    public float c_max;

    public float amplitude;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;

    public CNoiseNode()
    {
        sizeX = 0;
        sizeY = 0;
        
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        amplitude = 1;

        t_smooth = false;
        t_slide = false;
        invert = false;
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
        
        return height * amplitude;
    }
}