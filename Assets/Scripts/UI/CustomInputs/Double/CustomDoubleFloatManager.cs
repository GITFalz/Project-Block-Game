using System.Globalization;
using UnityEngine;

public class CustomDoubleFloatManager : CustomDoubleAbstractManager
{
    public override string ToCWorld()
    {
        float a = float.Parse(_fieldA.text, CultureInfo.InvariantCulture);
        float b = float.Parse(_fieldB.text, CultureInfo.InvariantCulture);
        
        string aStr = a.ToString(CultureInfo.InvariantCulture);
        string bStr = b.ToString(CultureInfo.InvariantCulture);
        string nameStr = doName ? name.text : "";
        
        return $"{nameStr} {aStr}, {bStr}\n";
    }
}