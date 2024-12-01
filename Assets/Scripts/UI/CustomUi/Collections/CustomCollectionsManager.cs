using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollectionsManager : MonoBehaviour
{
    [Header("Collection Parameters")] 
    public string collectionName;
    public GameObject collectionPrefab;
    
    public List<GameObject> collectionObjects = new List<GameObject>();
    public List<I_CustomUi> collectionList = new List<I_CustomUi>();

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
        
        _height = _rectTransform.rect.height;
        _position = _rectTransform.position;

        foreach (Transform c in _content)
        {
            I_CustomUi ui = c.GetComponent<I_CustomUi>();
            
            collectionObjects.Add(c.gameObject);
                
            if (ui is CustomGroupManager group)
                collectionList.Add(group);
            else if (ui is CustomDoubleIntManager doubleInt)
                collectionList.Add(doubleInt);
            
        }
        
        AlignCollections();
    }
    
    public void AlignCollections()
    {
        Vector3 position = new Vector3(0, _height, 0);
        _content.GetComponent<RectTransform>().position = _position - position;
        
        for (int i = 0; i < collectionObjects.Count; i++)
        {
            if (collectionObjects[i].activeSelf == false)
                continue;
            
            collectionList[i].Align(position);
        }
    }
}
