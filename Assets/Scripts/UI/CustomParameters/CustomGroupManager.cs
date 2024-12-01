using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomGroupManager : MonoBehaviour, I_CustomUi
{
    [Header("Parameters")] 
    public string name;
    
    [Header("Grouped Parameters")]
    public List<GameObject> parameters = new List<GameObject>();
    
    private bool _open = true;
    private Button _button;
    private TMP_Text _text;
    
    private Transform _content;
    private CustomUiUpdater _updater;
    
    private float _height;
    private Vector3 _position;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _button = transform.Find("Panel").Find("Button").GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        
        _text = transform.Find("Panel").Find("Under").Find("Text").GetComponent<TMP_Text>();
        _text.text = name;
        
        _content = transform.Find("Content");
        _rectTransform = transform.Find("Panel").Find("Under").GetComponent<RectTransform>();
        
        if (_content == null || _rectTransform == null) 
            return;
        
        _height = _rectTransform.rect.height;
        _position = _rectTransform.position;
        
        foreach (Transform p in _content)
        {
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

        _updater.go = this.gameObject;
        _updater.count = 3;
    }

    public float Align(Vector3 pos)
    {
        Vector3 position = _rectTransform.position - pos;
        _content.GetComponent<RectTransform>().position = position;
        
        foreach (var parameter in parameters)
        {
            if (parameter.activeSelf == false)
                continue;
            
            I_CustomUi ui = parameter.GetComponent<I_CustomUi>();
            if (ui != null)
                position.y += ui.Align(position);
        }

        return position.y;
    }
}