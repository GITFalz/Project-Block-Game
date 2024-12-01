using UnityEngine;

public class CWOPLerpNode : CWAParameterNode
{
    private float min;
    private float max;

    public CWOPLerpNode(float min, float max)
    {
        this.min = min; this.max = max;
    }
    
    public override float GetValue(float value)
    {
        return Mathf.Lerp(min, max, value);
    }
}