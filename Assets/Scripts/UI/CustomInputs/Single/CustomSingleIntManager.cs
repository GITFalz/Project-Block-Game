using TMPro;
using UnityEngine;

public class CustomSingleIntManager : CustomSingleAbstractManager
{
    public override string ToCWorld()
    {
        int a = int.Parse(_fieldA.text);

        return $"{name.text} {a}\n";
    }
}
