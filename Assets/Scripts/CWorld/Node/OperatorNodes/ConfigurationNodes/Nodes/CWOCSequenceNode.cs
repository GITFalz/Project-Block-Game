public class CWOCSequenceNode : CWAConfigurationNode
{
    public int top_min;
    public int top_max;

    public Block block;
    
    public override Block GetBlock(int airDistance, int height)
    {
        if (airDistance >= top_min && airDistance <= top_max)
            return block;
        return null;
    }

    public override bool Match(int height)
    {
        throw new System.NotImplementedException();
    }
}