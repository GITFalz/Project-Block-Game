public class CWorldModifierNode
{
    public string name;
    
    public CWorldSampleNode sample;
    public IntRangeNode range;
    public FloatRangeNode ignore;
    public bool invert;

    public CWorldModifierNode(string name)
    {
        this.name = name;
        range = new IntRangeNode();
        ignore = null;
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

    public IntRangeNode()
    {
        min = 0;
        max = WorldInfo.worldMaxTerrainHeight;
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

    public FloatRangeNode()
    {
        min = 0;
        max = 1;
    }
}