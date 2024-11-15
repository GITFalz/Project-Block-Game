using System.Collections.Generic;

public class CWorldSampleNode : CWAInitializerNode
{
    public string name;
    
    public CWOSOverrideNode overrideNode;
    public CWorldNoiseNode noiseNode;
    public CWOSSpecificsNode SpecificsNode;
    public float noiseValue;

    public bool flip;
    public int min_height;
    public int max_height;

    public CWorldSampleNode(string sampleName)
    {
        name = sampleName;
        overrideNode = new CWOSOverrideNode();
        noiseNode = new CWorldNoiseNode();
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

    public void ApplyOverride()
    {
        noiseValue = overrideNode.Apply(noiseValue);
    }

    public override float GetNoise()
    {
        return noiseValue;
    }

    public override uint GetPillar(int x, int y, int z)
    {
        return 1;
    }

    public CWorldSampleNode Copy(CWorldDataHandler handler)
    {
        if (handler.sampleNodes.TryGetValue(name, out var s))
            return s;
        
        CWorldSampleNode sampleNode = new CWorldSampleNode(name)
        {
            noiseNode = noiseNode,
            SpecificsNode = SpecificsNode,
            flip = flip,
            min_height = min_height,
            max_height = max_height
        };

        sampleNode.overrideNode = overrideNode.Copy(handler);

        return sampleNode;
    }
}