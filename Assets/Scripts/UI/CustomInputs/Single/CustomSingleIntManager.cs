using TMPro;
using UnityEngine;

public class CustomSingleIntManager : CustomSingleAbstractManager
{
    [Header("Base Settings")] 
    public int valueA;
    
    [Header("Misc")]
    public float vA;

    protected override void InitValues()
    {
        _fieldA.text = $"{valueA}";
    }
    
    protected override void SetHoldA(float x)
    {
        vA += x;
        
        valueA = (int) vA;
    }
    
    public override string ToCWorld()
    {
        if (!isChecked) return "";
        
        int a = int.Parse(_fieldA.text);

        return $"{name.text} {a}\n";
    }
}
