using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SingleFloatParameterManager : MonoBehaviour, I_CustomUi
{
    [Header("Text")]
    public TMP_Text text;
    public string name;
    
    [Header("Value")]
    public TMP_InputField inputField;
    
    private float _value;
    private bool _valueChanged;

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

    public void Init(CustomUICollectionManager collectionManager)
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

    public float Align(Vector3 position)
    {
        transform.position = position;
        return transform.GetComponent<RectTransform>().rect.height;
    }

    public string ToCWorld()
    {
        return "";
    }
}
