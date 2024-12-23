using UnityEngine;

public class CustomDoubleIntManager : CustomDoubleAbstractManager
{
    [Header("Base Settings")]
    public int valueA;
    public int valueB;

    [Header("Misc")] 
    public float vA;
    public float vB;
    
    protected override void InitValues()
    {
        _fieldA.text = $"{valueA}";
        _fieldB.text = $"{valueB}";
    }
    
    protected override void SetHoldBoth(float x)
    {
        vA += x;
        vB += x;
        
        valueA = (int) vA;
        valueB = (int) vB;
    }

    protected override void SetHoldA(float x)
    {
        vA += x;
        
        valueA = (int) vA;
    }
    
    protected override void SetHoldB(float x)
    {
        vB += x;
        
        valueB = (int) vB;
    }
    
    public override string ToCWorld()
    {
        if (!isChecked) return "";
        
        string stringA = _fieldA.text.Replace(',', '.');
        string stringB = _fieldB.text.Replace(',', '.');
        
        int a = int.Parse(stringA);
        int b = int.Parse(stringB);
        
        string nameStr = doName ? name.text : "";

        return $"{nameStr} {a}, {b}\n";
    }
}