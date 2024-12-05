using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomGroupManager : CustomUIGroup
{
    [Header("Panel")]
    public Button button;
    public TMP_Text text;
    
    private bool _open = true;

    public override void Init(CustomUICollectionManager collectionManager)
    {
        base.Init(collectionManager);
        
        if (button == null)
        {
            Console.Log("Group Button is null");
            return;
        }
        
        if (text == null)
        {
            Console.Log("Group Text is null");
            return;
        }
        
        button.onClick.AddListener(OnClick);
        text.text = name.type;
    }
    
    public void OnClick()
    {
        _open = !_open;
        foreach (var o in contentObjects)
        {
            o.SetActive(_open);
        }
        
        CollectionManager.AlignCollections();
    }
    
    public override string ToCWorld()
    {
        string text = $"{name.text}\n{{\n";
        
        text += base.ToCWorld();

        return text + "}\n";
    }
}