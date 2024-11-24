using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class VESettings : MonoBehaviour
{
    [Header("CWorld Text")] 
    public string text;
    
    [Header("Node Parameters")]
    public List<CustomUIBase> Parameters = new List<CustomUIBase>();
    
    private TMP_Text TextComponent;

    private void Awake()
    {
        if (TextComponent == null)
            TextComponent = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();

        Init();
    }
    
    public void Init()
    {

    }

    public string SettingToText()
    {
        string sb = "\n" + text + " {";
        foreach (var parameter in Parameters)
        {
            if (parameter != null)
                sb += parameter.ParameterToText();
        }

        return sb + "\n}";
    }

    public void Show()
    {
        Debug.Log(SettingToText());
    }
}