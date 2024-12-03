using System.Globalization;
using TMPro;
using UnityEngine;

public class CustomSingleFloatManager : CustomSingleAbstractManager
{
    public override string ToCWorld()
    {
        float a = float.Parse(_fieldA.text, CultureInfo.InvariantCulture);
        string aStr = a.ToString(CultureInfo.InvariantCulture);
        return $"{name.text} {aStr}\n";
    }
}