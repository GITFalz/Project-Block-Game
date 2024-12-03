using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomUINodeCollectionObject : MonoBehaviour, I_CustomUi
{
    [Header("Parameters")] 
    public TypeOrText name;
    
    [Header("Grouped Parameters")]
    public List<GameObject> parameters = new List<GameObject>();
    
    [Header("Print")]
    public bool doName;
    
    [Header("Misc")]
    public CustomUICollectionManager collectionsManager;
    public TMP_Text text;
    public TMP_InputField inputField;
    
    private Transform _content;
    
    private float _height;
    private RectTransform _rectTransform;

    public void Init(CustomUICollectionManager collectionManager)
    {
        collectionsManager = collectionManager;
        
        text = transform.Find("Panel").Find("Text").GetComponent<TMP_Text>();
        text.text = collectionManager.collectionName.type;
        
        inputField = transform.Find("Panel").Find("Input").GetComponent<TMP_InputField>();
        inputField.text = collectionManager.collectionName.text + "" + collectionManager.collectionObjects.Count;
        
        _content = transform.Find("Content");
        _rectTransform = transform.GetComponent<RectTransform>();
        
        if (_content == null || _rectTransform == null) 
            return;
        
        _height = _rectTransform.rect.height;
        
        foreach (Transform p in _content)
        {
            I_CustomUi cI = p.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            cI.Init(collectionManager);
            parameters.Add(p.gameObject);
        }
    }

    public float Align(Vector3 pos)
    {
        float height = _height;
        
        transform.position = pos;
        Vector3 position = pos - new Vector3(0, _height, 0);
        
        if (collectionsManager.DoHorizontalSpacing())
            position.x += 20;
        
        foreach (var parameter in parameters)
        {
            if (parameter.activeSelf == false)
                continue;
            
            I_CustomUi cI = parameter.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            float newHeight = cI.Align(position);
            position.y -= newHeight;
            height += newHeight;
        }

        return height;
    }
    
    public string ToCWorld()
    {
        string nodeName = doName ? $" ( name = {inputField.text} )" : "";
        string text = $"{name.text}{nodeName}\n{{\n";
        foreach (var parameter in parameters)
        {
            if (parameter.activeSelf == false)
                continue;
            
            I_CustomUi cI = parameter.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;

            text += cI.ToCWorld();
        }

        return text + "}\n";
    }
}