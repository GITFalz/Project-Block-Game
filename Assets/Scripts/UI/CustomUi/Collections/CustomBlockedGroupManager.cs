using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomBlockedGroupManager : MonoBehaviour, I_GroupedUi
{
    [Header("Collection Parameters")] 
    public TypeOrText collectionName;
    
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

    public bool DoHorizontalSpacing()
    {
        return false;
    }
}