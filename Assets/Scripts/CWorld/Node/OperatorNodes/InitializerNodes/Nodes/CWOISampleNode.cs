using System.Collections.Generic;

public class CWOISampleNode : CWAInitializerNode
{
    public CWOSOverrideNode overrideNode;
    public CWOSNoiseNode noiseNode;
    public CWOSSpecificsNode SpecificsNode;
    public float noiseValue;

    public bool flip;
    public int min_height;
    public int max_height;

    public CWOISampleNode()
    {
        overrideNode = new CWOSOverrideNode();
        noiseNode = new CWOSNoiseNode();
        SpecificsNode = new CWOSSpecificsNode();
        noiseValue = 0;

        flip = false;
        min_height = 0;
        max_height = 256;
    }

    public override void Init(int x, int y, int z)
    {
        noiseValue = noiseNode.GetNoiseValue(x, z);
    }

    public override float GetNoise()
    {
        float height = noiseValue;
        height = overrideNode.Apply(height);
        return height;
    }

    public override uint GetPillar(int x, int y, int z)
    {
        return 1;
    }
    
}