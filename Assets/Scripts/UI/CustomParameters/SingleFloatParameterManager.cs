using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SingleFloatParameterManager : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text text;
    public string name;
    
    [Header("Value")]
    public TMP_InputField inputField;
    
    private float _value;
    private bool _valueChanged;

    private void Start()
    {
        text.text = name;
        
        inputField.onValueChanged.AddListener((value) =>
        {
            if (float.TryParse(value, out var result))
            {
                _value = result;
                _valueChanged = true;
            }
        });
    }

    public bool ValueChanged(out float value)
    {
        if (!_valueChanged)
        {
            value = 0;
            return false;
        }
        
        _valueChanged = false;
        value = _value;
        return true;
    }
    
    public void ChangeValue(float value)
    {
        _value = value;
        inputField.text = value.ToString(CultureInfo.InvariantCulture);
    }
}
