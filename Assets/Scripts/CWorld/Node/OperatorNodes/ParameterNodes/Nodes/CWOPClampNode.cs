using UnityEngine;

public class CWOPClampNode : CWAParameterNode
{
    private float min;
    private float max;

    public CWOPClampNode(float min, float max)
    {
        this.min = min; this.max = max;
    }

    public override float GetValue(float value)
    {
        return Mathf.Clamp(value, min, max);
    }
}