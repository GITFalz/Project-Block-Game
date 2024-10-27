using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public Dictionary<string, CWAInitializerNode> initializers;
    public Dictionary<string, CWAExecuteNode> executes;

    private bool test = true;

    public void Init()
    {
        initializers = new Dictionary<string, CWAInitializerNode>();
        executes = new Dictionary<string, CWAExecuteNode>();
    }

    public float GetTextureNoise(int x, int y, int z)
    {
        foreach (var i in initializers.Values)
        {
            i.Init(x, y, z);
        }

        float height = 0;

        foreach (CWAExecuteNode e in executes.Values)
        {
            float noise = e.GetNoise();
            if (noise > height)
                height = noise;
        }

        return height;
    }

    public float GetSampleNoise(int x, int y, int z, string sampleName)
    {
        if (initializers.TryGetValue(sampleName, out CWAInitializerNode i))
        {
            return i.GetNoise();
        }
        return 0;
    }
    
    public float GetSampleNoise(int x, int y, int z, CWAInitializerNode i)
    {
        Init(x, y, z);
        return i.GetNoise();
    }

    public Block[] GenerateBiome(Vector3Int position, Block[] blocks, string biomeName)
    {
        if (executes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlocks(position, blocks, this);
        }

        return blocks;
    }
    
    public uint GetBlockMapPillar(int x, int y, int z, string sampleName)
    {
        if (initializers.TryGetValue(sampleName, out CWAInitializerNode i))
        {
            return i.GetPillar(x, y, z);
        }
        return 0;
    }

    public void Init(int x, int y, int z)
    {
        foreach (var i in initializers.Values)
        {
            i.Init(x, y, z);
        }
    }
    
    public Block GetBlock(int x, int y, int z)
    {
        Block block = null;
        foreach (CWAExecuteNode execute in executes.Values)
        {
            //Block b = execute.GetBlock(x, y, z);
            //if (b != null) block = b;
        }

        return block;
    }
}

public abstract class CNode
{
    
}

public abstract class CExecute : CNode
{
    public abstract float GetNoise();
    public abstract Block GetBlock(int x, int y, int z);
}

public class CBiomeNode : CExecute
{
    public List<CBlockHeightSequence> blocks;
    
    public CSampleNode sample;

    public float t_min;
    public float t_max;

    public float minHeight;
    public float maxHeight;

    public CBiomeNode()
    {
        t_min = 0;
        t_max = 1;
        
        sample = new CSampleNode();
    }
    
    public override float GetNoise()
    {
        return sample.GetNoise();
    }

    public override Block GetBlock(int x, int y, int z)
    {
        return GetBlock(x, y, z, sample.GetNoise());
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
    public abstract float GetNoise();
}


public class CSampleNode : CInit
{
    public List<CSampleNode> add;
    public List<CSampleNode> mul;
    public List<CSampleNode> sub;
    
    public COverrideNode overRide;
    public CNoiseNode noise;
    public float noiseValue;

    public CSampleNode()
    {
        add = new List<CSampleNode>();
        mul = new List<CSampleNode>();
        sub = new List<CSampleNode>();
                
        overRide = new COverrideNode();
        noise = new CNoiseNode();
        noiseValue = 0;
    }

    public override void Init(int x, int z)
    {
        noiseValue = noise.GetNoiseValue(x, z);
    }

    public override float GetNoise()
    {
        float height = noiseValue;

        if (overRide != null) 
        {
            foreach (CSampleNode node in add)
                height += node.noiseValue;
            foreach (CSampleNode node in mul)
                height *= node.noiseValue;
            foreach (CSampleNode node in sub)
                height -= node.noiseValue;

            height = overRide.Apply(height);
        }
        
        return height;
    }
}

public class COverrideNode : CSettings
{
    public float t_min;
    public float t_max;

    public float c_min;
    public float c_max;

    public float l_min;
    public float l_max;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;
    public bool lerp;
    
    public COverrideNode()
    {
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        l_min = 0;
        l_max = 1;

        t_smooth = false;
        t_slide = false;
        invert = false;
        lerp = false;
    }

    public float Apply(float height)
    {
        if (t_smooth)
            height = Mathp.PLerp(t_min, t_max, height);
        if (t_slide)
            height = Mathp.SLerp(t_min, t_max, height);
        if (invert)
            height = 1 - height;
        if (lerp)
            height = Mathf.Lerp(l_min, l_max, height);
        
        height = Mathf.Clamp(height, c_min, c_max);
        
        return height;
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

    public float l_min;
    public float l_max;

    public float amplitude;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;

    public bool lerp;

    public CNoiseNode()
    {
        sizeX = 0;
        sizeY = 0;
        
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        l_min = 0;
        l_max = 1;

        amplitude = 1;

        t_smooth = false;
        t_slide = false;
        invert = false;
    }
    
    public float GetNoiseValue(int x, int z)
    {
        float height = Mathf.PerlinNoise((float)((float)x / sizeX + 0.001f), (float)((float)z / sizeY + 0.001f));
        
        if (t_smooth)
            height = Mathp.PLerp(t_min, t_max, height);
        if (t_slide)
            height = Mathp.SLerp(t_min, t_max, height);
        if (invert)
            height = 1 - height;
        if (lerp)
            height = Mathf.Lerp(l_min, l_max, height);
        
        height = Mathf.Clamp(height, c_min, c_max);
        
        return height * amplitude;
    }
}