using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public Block GetBlock(int x, int y, int z)
    {
        Block block = null;

        return block;
    }
}

public abstract class TypeNode
{
    public abstract NodeData Go(int x, int y, int z);
}

public class MaskNode : TypeNode
{
    public override int Get(int x, int y, int z)
    {
        return 0;
    }
    
    public override float Set(float a)
    {
        return 0;
    }
}

public class SampleNode : TypeNode
{
    public NoiseSetting noise;
    
    public override int Get(int x, int y, int z)
    {
        return 0;
    }
    
    public override float Set(float a)
    {
        return 0;
    }

    public float GetNoise(int x, int y, int z) { }
}

public class BiomeNode : TypeNode
{
    public TypeNode sample;
    public TypeNode mask;

    public BiomeNode()
    {
        sample = null;
        mask = null;
    }
    
    public override int Get(int x, int y, int z)
    {
        return 0;
    }
    
    public override float Set(float a)
    {
        return 0;
    }

    public int GetBlock(int x, int y, int z, TypeNode sample)
    {
        (SampleNode)sample.
    }
}

public class TreeNode : TypeNode
{
    public override int Get(int x, int y, int z)
    {
        return 0;
    }
    
    public override float Set(float a)
    {
        return 0;
    }
}

public class BlockNode : TypeNode
{
    public override int Get(int x, int y, int z)
    {
        return 0;
    }
    
    public override float Set(float a)
    {
        return 0;
    }
}

public class NodeData
{
    public float height;
    public Block block;
}

public class NoiseSetting
{
    public int sizeX;
    public int sizeY;

    public int t_min;
    public int t_max;  
    
    public int c_min;
    public int c_max;

    public bool t_slide;
    public bool t_smooth;
    public bool invert;

    public NoiseSetting()
    {
        sizeX = 0;
        sizeY = 0;
        
        t_min = 0;
        t_max = 1;
        
        c_min = 0;
        c_max = 1;
        
        t_slide = false;
        t_smooth = false;
        invert = false;
    }
    
    public float GetNoise(int x, int y, int z)
    {
        float height = Mathf.Clamp(Mathf.PerlinNoise((float)((float)x / sizeX + 0.001f), (float)((float)y / sizeY + 0.001f)), c_min, c_max);
        
        if (t_smooth)
            height = Mathp.PLerp(t_min, t_max, height);
        if (t_slide)
            height = Mathp.SLerp(t_min, t_max, height);
        if (invert)
            height = 1 - height;

        return height;
    }
}