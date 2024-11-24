using System;
using TMPro;
using UnityEngine;


[System.Serializable]
public class VEParameter1 : MonoBehaviour, ICustomUI
{
    [Header("CWorld Text")]
    public string text;
    
    private TMP_Text _textComponent;
    private TMP_InputField _inputField;
    
    public void Init()
    {
        if (_textComponent == null)
            _textComponent = transform.Find("Text").GetComponent<TMP_Text>();
            
        if (_inputField == null)
            _inputField = transform.Find("Input").GetComponent<TMP_InputField>();
    }

    public string ParameterToText()
    {
        return _inputField != null && _textComponent != null ? $"\n{text} {_inputField.text}" : "\n";
    }
}