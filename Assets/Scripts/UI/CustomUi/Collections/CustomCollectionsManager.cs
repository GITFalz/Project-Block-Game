using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomCollectionsManager : MonoBehaviour
{
    [Header("Collection Parameters")] 
    public TypeOrText collectionName;
    public GameObject collectionPrefab;
    
    public List<GameObject> collectionObjects = new List<GameObject>();

    private Transform _content;
    
    private RectTransform _rectTransform;
    private float _height;
    private Vector3 _position;

    private void Awake()
    {
        _content = transform.Find("Content");
        _rectTransform = transform.Find("Panel").GetComponent<RectTransform>();
        
        if (_content == null || _rectTransform == null)
            return;
        
        
        transform.Find("Panel").Find("Text").GetComponent<TMP_Text>().text = collectionName.type;
        transform.Find("Panel").Find("Button").GetComponent<Button>().onClick.AddListener(AddCollection);
        transform.Find("Panel").Find("Show").GetComponent<Button>().onClick.AddListener(Show);
        
        _height = _rectTransform.rect.height;
        _position = _rectTransform.position;

        foreach (Transform c in _content)
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
        GameObject collection = Instantiate(collectionPrefab, _content);
        I_CustomUi cI = collection.GetComponent<I_CustomUi>();
        
        if (cI == null || cI.Equals(null))
            return;
        
        cI.Init(this);
        collectionObjects.Add(collection);
        AlignCollections();
    }

    public void Show()
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
    }
    
    public void AlignCollections()
    {
        Vector3 position = _position - new Vector3(0, _height, 0);
        _content.GetComponent<RectTransform>().position = position;
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
}

[System.Serializable]
public class TypeOrText
{
    public string type;
    public string text;
}