using System.Globalization;
using TMPro;
using UnityEngine;

public class CustomSingleFloatManager : CustomSingleAbstractManager
{
    [Header("Base Settings")]
    public float valueA;
    
    protected override void InitValues()
    {
        _fieldA.text = $"{valueA}";
    }
    
    protected override void SetHoldA(float x)
    {
        valueA += x;
    }
    
    public override string ToCWorld()
    {
        if (!isChecked) return "";
        string stringA = _fieldA.text.Replace(',', '.');
        float a = float.Parse(stringA, CultureInfo.InvariantCulture);
        string aStr = a.ToString(CultureInfo.InvariantCulture);
        return $"{name.text} {aStr}\n";
    }
}