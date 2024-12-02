using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomCollectionsManager : MonoBehaviour
{
    [Header("Collection Parameters")] 
    public string collectionName;
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
        
        transform.Find("Panel").Find("Text").GetComponent<TMP_Text>().text = collectionName;
        
        _height = _rectTransform.rect.height;
        _position = _rectTransform.position;

        foreach (Transform c in _content)
        {
            collectionObjects.Add(c.gameObject);
        }
        
        AlignCollections();
    }
    
    public void AlignCollections()
    {
        Vector3 position = _position - new Vector3(0, _height, 0);
        _content.GetComponent<RectTransform>().position = position;
        
        foreach (var c in collectionObjects)
        {
            Vector3 newPosition = transform.position;
            newPosition.y -= _height;
            I_CustomUi cI = c.GetComponent<I_CustomUi>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            cI.Align(newPosition);
        }
    }
}
