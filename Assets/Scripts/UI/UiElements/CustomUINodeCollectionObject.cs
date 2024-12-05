using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomUINodeCollectionObject : CustomUIGroup
{
    [Header("Panel")]
    public TMP_Text text;
    public TMP_InputField inputField;

    public override void Init(CustomUICollectionManager collectionManager)
    {
        base.Init(collectionManager);
        
        if (text == null)
        {
            Console.Log("Collection Text is null");
            return;
        }
        
        if (inputField == null)
        {
            Console.Log("Collection InputField is null");
            return;
        }
            
        text.text = collectionManager.collectionName.type;
        inputField.text = $"{CollectionManager.collectionName.text}{contentObjects.Count}";
    }
    
    public override string ToCWorld()
    {
        string nodeName = doName ? $" ( name = {inputField.text} )" : "";
        string text = $"{name.text}{nodeName}\n{{\n";

        text += base.ToCWorld();
        
        return text + "}\n";
    }
}