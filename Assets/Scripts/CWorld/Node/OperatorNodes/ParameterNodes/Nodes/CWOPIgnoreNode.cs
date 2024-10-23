public class CWOPIgnoreNode : CWAParameterNode
{
    private float min;
    private float max;

    public CWOPIgnoreNode(float min, float max)
    {
        this.min = min; this.max = max;
    }

    public override float GetValue(float value)
    {
        return value >= min && value <= max ? -1 : value;
    }
}