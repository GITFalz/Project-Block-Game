using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomNodeGroupManager : MonoBehaviour, I_CustomUi
{
    [Header("Parameters")] 
    public TypeOrText name;
    
    [Header("Grouped Parameters")]
    public List<GameObject> parameters = new List<GameObject>();
    
    [Header("Misc")]
    public I_GroupedUi collectionsManager;
    
    private bool _open = true;
    public Button _button;
    public TMP_InputField _inputField;
    public TMP_Text _text;
    
    private Transform _content;
    private CustomUiUpdater _updater;
    
    private float _height;
    private RectTransform _rectTransform;

    public void Init(I_GroupedUi collectionManager)
    {
        collectionsManager = collectionManager;
        
        _button = transform.Find("Panel").Find("Button").GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        
        _text = transform.Find("Panel").Find("Under").Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
        
        _inputField = transform.Find("Panel").Find("Under").Find("Input").GetComponent<TMP_InputField>();
        
        _content = transform.Find("Content");
        _rectTransform = transform.Find("Panel").Find("Under").GetComponent<RectTransform>();
        
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

        _updater = GameObject.Find("Managers").GetComponent<CustomUiUpdater>();
    }
    
    public void OnClick()
    {
        _open = !_open;
        foreach (var parameter in parameters)
        {
            parameter.SetActive(_open);
        }
        
        collectionsManager.AlignCollections();

        _updater.go = this.gameObject;
        _updater.count = 3;
    }

    public float Align(Vector3 pos)
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
        string text = $"{name.text} ( name = {_inputField.text} )\n{{\n";
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