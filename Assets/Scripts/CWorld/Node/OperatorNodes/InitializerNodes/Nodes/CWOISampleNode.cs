using System.Collections.Generic;

public class CWOISampleNode : CWAInitializerNode
{
    public CWOSOverrideNode overrideNode;
    public CWOSNoiseNode noiseNode;
    public float noiseValue;

    public CWOISampleNode()
    {
        overrideNode = new CWOSOverrideNode();
        noiseNode = new CWOSNoiseNode();
        noiseValue = 0;
    }

    public override void Init(int x, int z)
    {
        noiseValue = noiseNode.GetNoiseValue(x, z);
    }

    public override float GetNoise()
    {
        float height = noiseValue;
        height = overrideNode.Apply(height);
        return height;
    }
}