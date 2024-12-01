public class CWOPSlideNode : CWAParameterNode
{
    private float min;
    private float max;

    public CWOPSlideNode(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public override float GetValue(float value)
    {
        return MathUtils.SLerp(min, max, value);
    }
}