
using TMPro;
using UnityEngine;

public class CustomDoubleIntManager : MonoBehaviour, I_CustomUi
{
    [Header("Properties")] public string name;
    
    private TMP_Text _text;
    private TMP_InputField _fieldA;
    private TMP_InputField _fieldB;
    
    private RectTransform _rectTransform;
    private float _height;

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _text.text = name;
        
        _fieldA = transform.Find("Input1").GetComponent<TMP_InputField>();
        _fieldB = transform.Find("Input2").GetComponent<TMP_InputField>();
    }

    public float Align(Vector3 position)
    {
        _rectTransform.position = position;
        return _rectTransform.rect.height;
    }
}