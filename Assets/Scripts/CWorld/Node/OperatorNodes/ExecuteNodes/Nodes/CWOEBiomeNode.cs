using System.Collections.Generic;

public class CWOEBiomeNode : CWAExecuteNode
{
    public List<CBlockHeightSequence> blocks;
    
    public CWOISampleNode sample;

    public float t_min;
    public float t_max;

    public float minHeight;
    public float maxHeight;

    public CWOEBiomeNode()
    {
        
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