public class CWOPSmoothNode : CWAParameterNode
{
    private float min;
    private float max;

    public CWOPSmoothNode(float min, float max)
    {
        this.min = min; this.max = max;
    }
    
    public override float GetValue(float value)
    {
        return MathUtils.PLerp(min, max, value);
    }
}