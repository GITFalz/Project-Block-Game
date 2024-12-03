using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomUICollectionManager: MonoBehaviour
{
    [Header("Collection Parameters")] 
    public TypeOrText collectionName;
    public GameObject collectionPrefab;
    public bool doHorizontalSpacing;

    [Header("Content")]
    public Transform content;
    
    [Header("Misc")]
    public List<GameObject> collectionObjects = new List<GameObject>();
    
    private RectTransform _rectTransform;
    private float _height;
    private Vector3 _position;

    private void Awake()
    {
        Transform panel = transform.Find("Panel");

        if (panel == null)
        {
            Debug.LogError("Collection Manager: Panel not found");
            return;
        }
        
        content = transform.Find("Content");
        _rectTransform = panel.GetComponent<RectTransform>();
        
        if (content == null || _rectTransform == null)
            return;

        Transform text = panel.Find("Text");
        Transform button = panel.Find("Button");
        Transform show = panel.Find("Show");
            
        if (text != null)
        {
            TMP_Text textText = text.GetComponent<TMP_Text>();
            if (textText != null)
                textText.text = collectionName.type;
        }
            
        if (button != null)
        {
            Button buttonButton = button.GetComponent<Button>();
            if (buttonButton != null)
                buttonButton.onClick.AddListener(AddCollection);
        }
            
        if (show != null)
        {
            Button showButton = show.GetComponent<Button>();
            if (showButton != null)
                showButton.onClick.AddListener(() => Show());
        }
        
        _height = _rectTransform.rect.height;
        _position = _rectTransform.position;

        foreach (Transform c in content)
        {
            I_CustomUi cI = c.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            cI.Init(this);
            collectionObjects.Add(c.gameObject);
        }
        
        AlignCollections();
    }
    
    public void AddCollection()
    {
        GameObject collection = Instantiate(collectionPrefab, content);
        I_CustomUi cI = collection.GetComponent<I_CustomUi>();
        
        if (cI == null || cI.Equals(null))
            return;
        
        cI.Init(this);
        collectionObjects.Add(collection);
        AlignCollections();
    }

    public string Show()
    {
        string text = "";
        
        foreach (var c in collectionObjects)
        {
            if (c.activeSelf == false)
                continue;
            
            I_CustomUi cI = c.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            text += cI.ToCWorld();
        }
        
        Debug.Log(text);

        return text;
    }
    
    public void AlignCollections()
    {
        Vector3 position = _position - new Vector3(0, _height, 0);
        content.GetComponent<RectTransform>().position = position;
        Vector3 newPosition = transform.position - new Vector3(0, _height, 0);
        
        foreach (var c in collectionObjects)
        {
            if (c.activeSelf == false)
                continue;
            
            I_CustomUi cI = c.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            newPosition.y -= cI.Align(newPosition);
        }
    }
    
    public bool DoHorizontalSpacing()
    {
        return doHorizontalSpacing;
    } 
}

[Serializable]
public class TypeOrText
{
    public string type;
    public string text;
}