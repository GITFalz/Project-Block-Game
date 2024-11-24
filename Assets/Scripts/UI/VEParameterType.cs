using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class VEParameterType : AType
{
    private TMP_Dropdown _sign;
    private TMP_Dropdown _modifier;
    
    private VEParameterModifier _modifierScript;

    private int index = 0;
    
    public override void Init()
    {
        transform.Find("Text").GetComponent<TMP_Text>().text = "Type";
        _sign = transform.Find("Sign").GetComponent<TMP_Dropdown>();
        _modifier = transform.Find("Modifier").GetComponent<TMP_Dropdown>();
    }
    
    public void SetDropdown(List<NameToText> signs, List<NameToText> types)
    {
        _sign.options.Clear();
        foreach (var sign in signs)
        {
            _sign.options.Add(new TMP_Dropdown.OptionData(sign.text));
        }
        
        _modifier.options.Clear();
        foreach (var type in types)
        {
            _modifier.options.Add(new TMP_Dropdown.OptionData(type.text));
        }
    }
    
    public void SetScript(VEParameterModifier modifier)
    {
        _modifierScript = modifier;
    }
    
    public void SetIndex(int i)
    {
        index = i;
    }
    
    public override int GetIndex()
    {
        return index;
    }

    public override  void Decrement()
    {
        index--;
    }
    
    public override string ParameterToText()
    {
        return $"\n{_sign.options[_sign.value].text} {_modifier.options[_modifier.value].text}";
    }
    
    public void RemoveModifier()
    {
        if (_modifierScript == null)
            return;
        
        _modifierScript.Remove(index);
    }
}