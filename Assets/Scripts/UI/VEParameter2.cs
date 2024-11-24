using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VEParameter2 : CustomUIBase
{
    [Header("CWorld Text")]
    public string text;
    
    private TMP_Text TextComponent;
    private TMP_InputField InputField1;
    private TMP_InputField InputField2;
        
    public override void Init()
    {
        if (TextComponent == null)
            TextComponent = transform.Find("Text").GetComponent<TMP_Text>();
            
        if (InputField1 == null)
            InputField1 = transform.Find("Input1").GetComponent<TMP_InputField>();
        
        if (InputField2 == null)
            InputField2 = transform.Find("Input2").GetComponent<TMP_InputField>();
    }

    public override string ParameterToText()
    {
        if (InputField1 != null && InputField2 != null)
        {
            return $"\n{text} {InputField1.text}, {InputField2.text}";
        }

        return "\n";
    }
}