using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VEParameterModifier : CustomUIBase
{
    [Header("CWorld Text")]
    public string text;
    
    [Header("Content")]
    public GameObject option2;
    public GameObject option2Range;
    public Transform content;
    
    public List<NameToText> signs = new List<NameToText>();
    public List<NameToText> types = new List<NameToText>();
    
    private TMP_Text _textComponent;
    private List<AType> _types = new List<AType>();
        
    public override void Init()
    {
        if (_textComponent == null)
            _textComponent = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();
    }

    public override string ParameterToText()
    {
        string result = "\n" + text + " {";
        foreach (var modifier in _types)
        {
            result += modifier.ParameterToText();
        }
        result += "\n}";
        return result;
    }

    public void AddModifier()
    {
        GameObject modifier = Instantiate(option2, content);
        modifier.transform.SetAsLastSibling();
        
        VEParameterType typeScript = modifier.GetComponent<VEParameterType>();

        typeScript.SetScript(this);
        typeScript.SetIndex(_types.Count);
        typeScript.Init();
        typeScript.SetDropdown(signs, types);
        
        _types.Add(typeScript);

        go = transform.gameObject;
        count = 3;
    }
    
    public void AddOption2RangeModifier()
    {
        GameObject modifier = Instantiate(option2Range, content);
        modifier.transform.SetAsLastSibling();
        
        VEParameterOption2Range typeScript = modifier.GetComponent<VEParameterOption2Range>();

        typeScript.SetScript(this);
        typeScript.SetIndex(_types.Count);
        typeScript.Init();
        typeScript.SetDropdown(signs, types);
        
        _types.Add(typeScript);

        go = transform.gameObject;
        count = 3;
    }
    
    public void Remove(int index)
    {
        go = transform.gameObject;
        count = 3;
        
        if (_types.Count > 0)
        {
            Destroy(_types[index].gameObject);
            _types.RemoveAt(index);
        }
        
        foreach (var type in _types)
        {
            if (type.GetIndex() > index)
            {
                type.Decrement();
            }
        }
    }
}

public abstract class AType : CustomUIBase
{
    public abstract int GetIndex();
    public abstract void Decrement();
}

[System.Serializable]
public struct NameToText
{
    public string name;
    public string text;
}