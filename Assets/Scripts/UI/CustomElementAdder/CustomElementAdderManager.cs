using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomElementAdderManager : MonoBehaviour, I_CustomUi
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
    
    public void Init(CustomUICollectionManager collectionManager)
    {
        text.text = name.type;
        add.onClick.AddListener(AddElement);
        
        _collectionManager = collectionManager;
        
        _rectTransform = transform.GetComponent<RectTransform>();
        _height = _rectTransform.rect.height;
    }

    public float Align(Vector3 pos)
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
        string result = "";
        
        foreach (var element in elements)
        {
            I_CustomUi cI = element.GetComponent<I_CustomUi>();
            
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
        
        I_CustomUi cI = element.GetComponent<I_CustomUi>();
        cI.Init(_collectionManager);
        
        _collectionManager.AlignCollections();
    }
}