using System.Collections.Generic;

public class CWOSSpecificsNode : CWASettingsNode
{
    public List<CWAParameterNode> parameters;

    public CWOSSpecificsNode()
    {
        parameters = new List<CWAParameterNode>();
    }
    
    public override float GetFloatValue(float value)
    {
        foreach (var parameter in parameters)
            value = parameter.GetValue(value);
        
        return value;
    }
}