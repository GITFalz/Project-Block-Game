using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class VENode : MonoBehaviour
{
    [Header("CWorld Text")] 
    public string text;
    
    [Header("Node Parameters")]
    public List<VESettings> Settings = new List<VESettings>();
    
    [Header("Content")]
    public VisualEditorManager manager;
    
    private TMP_Text TextComponent;
    private TMP_InputField Name;

    private void Awake()
    {
        if (TextComponent == null)
            TextComponent = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();
        
        if (Name == null)
            Name = transform.Find("Panel").Find("Name").GetComponent<TMP_InputField>();

        Init();
    }
    
    public void Init()
    {

    }

    public string NodeToText()
    {
        string sb = "\n" + text + " ( name = " + Name.text + " ) \n{";
        foreach (var settings in Settings)
        {
            if (settings != null)
                sb += settings.SettingToText();
        }

        return sb + "\n}";
    }

    public void Show()
    {
        Debug.Log(NodeToText());
    }
    
    public void Generate()
    {
        manager.GenerateFoliage(NodeToText());
    }
}