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
        
        string stringA = _fieldA.text.Replace(',', '.');
        
        int a = int.Parse(stringA);

        return $"{name.text} {a}\n";
    }
}
