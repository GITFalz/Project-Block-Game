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
        
        float a = float.Parse(_fieldA.text, CultureInfo.InvariantCulture);
        string aStr = a.ToString(CultureInfo.InvariantCulture);
        return $"{name.text} {aStr}\n";
    }
}