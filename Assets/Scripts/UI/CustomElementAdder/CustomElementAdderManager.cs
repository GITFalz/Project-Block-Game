using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomElementAdderManager : CustomUI
{
    [Header("Properties")]
    public TypeOrText name;
    
    [Header("References")]
    public GameObject elementPrefab;
    public TMP_Text text;
    public Button add;
    public Transform content;
    
    private List<GameObject> elements = new List<GameObject>();
    private CustomUICollectionManager _collectionManager;
    
    private RectTransform _rectTransform;
    private float _height;
    
    public override void Init(CustomUICollectionManager collectionManager)
    {
        text.text = name.type;
        add.onClick.AddListener(AddElement);
        
        _collectionManager = collectionManager;
        
        _rectTransform = transform.GetComponent<RectTransform>();
        _height = _rectTransform.rect.height;
    }

    public override float Align(Vector3 pos)
    {
        float height = _height;
        
        transform.position = pos;
        Vector3 position = pos - new Vector3(0, _height, 0);
        
        if (_collectionManager.DoHorizontalSpacing())
            position.x += 20;
        
        foreach (var parameter in elements)
        {
            if (parameter.activeSelf == false)
                continue;
            
            CustomUI cI = parameter.GetComponent<CustomUI>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            float newHeight = cI.Align(position);
            position.y -= newHeight;
            height += newHeight;
        }

        return height;
    }

    public override string ToCWorld()
    {
        string result = "";
        
        foreach (var element in elements)
        {
            CustomUI cI = element.GetComponent<CustomUI>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            result += cI.ToCWorld();
        }
        
        return result;
    }
    
    
    public void AddElement()
    {
        GameObject element = Instantiate(elementPrefab, content);
        elements.Add(element);
        
        CustomUI cI = element.GetComponent<CustomUI>();
        cI.Init(_collectionManager);
        
        _collectionManager.AlignCollections();
    }
}