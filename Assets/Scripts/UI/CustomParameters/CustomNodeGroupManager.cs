using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomNodeGroupManager : CustomUI
{
    [Header("Parameters")] 
    public TypeOrText name;
    
    [Header("Grouped Parameters")]
    public List<GameObject> parameters = new List<GameObject>();
    
    [Header("Misc")]
    public CustomUICollectionManager collectionsManager;
    
    private bool _open = true;
    public Button _button;
    public TMP_InputField _inputField;
    public TMP_Text _text;
    
    private Transform _content;
    
    private float _height;
    private RectTransform _rectTransform;

    private int _index;

    public override void Init(CustomUICollectionManager collectionManager)
    {
        collectionsManager = collectionManager;
        
        _button = transform.Find("Panel").Find("Button").GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        
        _text = transform.Find("Panel").Find("Under").Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
        
        _inputField = transform.Find("Panel").Find("Under").Find("Input").GetComponent<TMP_InputField>();
        _inputField.onValueChanged.AddListener((value) => OnChange());
       
        
        _content = transform.Find("Content");
        _rectTransform = transform.Find("Panel").Find("Under").GetComponent<RectTransform>();
        
        if (_content == null || _rectTransform == null) 
            return;
        
        _height = _rectTransform.rect.height;
        
        foreach (Transform p in _content)
        {
            CustomUI cI = p.GetComponent<CustomUI>();
            
            if (cI == null || cI.Equals(null))
                continue;
            
            cI.Init(collectionManager);
            parameters.Add(p.gameObject);
        }
    }

    public void OnChange()
    {
        
    }
    
    public void OnClick()
    {
        _open = !_open;
        foreach (var parameter in parameters)
        {
            parameter.SetActive(_open);
        }
        
        collectionsManager.AlignCollections();
    }

    public override float Align(Vector3 pos)
    {
        Debug.Log("Node Group: " + pos);
        
        float height = _height;
        
        transform.position = pos;
        Vector3 position = pos - new Vector3(0, _height, 0);
        
        if (collectionsManager.DoHorizontalSpacing())
            position.x += 20;
        
        foreach (var parameter in parameters)
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
        string text = $"{name.text} ( name = {_inputField.text} )\n{{\n";
        foreach (var parameter in parameters)
        {
            if (parameter.activeSelf == false)
                continue;
            
            CustomUI cI = parameter.GetComponent<CustomUI>();
            
            if (cI == null || cI.Equals(null))
                continue;

            text += cI.ToCWorld();
        }

        return text + "}\n";
    }
}