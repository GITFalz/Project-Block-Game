using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomStaticUICollectionManager : MonoBehaviour
{
    [Header("Collection Parameters")] 
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
        content = transform.Find("Content");
        _rectTransform = transform.Find("Panel").GetComponent<RectTransform>();
        
        if (content == null || _rectTransform == null)
            return;
        
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