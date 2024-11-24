using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VEParameter3 : CustomUIBase
{
    [Header("CWorld Text")]
    public string text;
    
    private TMP_Text _textComponent;
    private TMP_InputField _inputField1;
    private TMP_InputField _inputField2;
    private TMP_InputField _inputField3;
        
    public override void Init()
    {
        if (_textComponent == null)
            _textComponent = transform.Find("Text").GetComponent<TMP_Text>();
            
        if (_inputField1 == null)
            _inputField1 = transform.Find("Input1").GetComponent<TMP_InputField>();
        
        if (_inputField2 == null)
            _inputField2 = transform.Find("Input2").GetComponent<TMP_InputField>();
        
        if (_inputField3 == null)
            _inputField3 = transform.Find("Input3").GetComponent<TMP_InputField>();
    }

    public override string ParameterToText()
    {
        if (_inputField1 != null && _inputField2 != null)
        {
            return $"\n{text} {_inputField1.text}, {_inputField2.text}, {_inputField3.text}";
        }

        return "\n";
    }
}