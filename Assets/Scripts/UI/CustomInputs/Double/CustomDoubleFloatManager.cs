using System.Globalization;
using UnityEngine;

public class CustomDoubleFloatManager : CustomDoubleAbstractManager
{
    [Header("Base Settings")]
    public float valueA;
    public float valueB;

    protected override void InitValues()
    {
        _fieldA.text = $"{valueA}";
        _fieldB.text = $"{valueB}";
    }

    protected override void SetHoldBoth(float x)
    {
        valueA += x;
        valueB += x;
    }

    protected override void SetHoldA(float x)
    {
        valueA += x;
    }
    
    protected override void SetHoldB(float x)
    {
        valueB += x;
    }

    public override string ToCWorld()
    {
        if (!isChecked) return "";
        
        string stringA = _fieldA.text.Replace(',', '.');
        string stringB = _fieldB.text.Replace(',', '.');
        
        float a = float.Parse(stringA, CultureInfo.InvariantCulture);
        float b = float.Parse(stringB, CultureInfo.InvariantCulture);
        
        string aStr = a.ToString(CultureInfo.InvariantCulture);
        string bStr = b.ToString(CultureInfo.InvariantCulture);
        string nameStr = doName ? name.text : "";
        
        return $"{nameStr} {aStr}, {bStr}\n";
    }
}