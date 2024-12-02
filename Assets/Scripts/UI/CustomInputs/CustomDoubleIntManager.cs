
using TMPro;
using UnityEngine;

public class CustomDoubleIntManager : MonoBehaviour, I_CustomUi
{
    [Header("Properties")] 
    public TypeOrText name;
    
    private TMP_Text _text;
    private TMP_InputField _fieldA;
    private TMP_InputField _fieldB;
    
    private RectTransform _rectTransform;
    private float _height;

    public void Init(I_GroupedUi collectionManager)
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
        
        _fieldA = transform.Find("Input1").GetComponent<TMP_InputField>();
        _fieldB = transform.Find("Input2").GetComponent<TMP_InputField>();
        
        _rectTransform = transform.GetComponent<RectTransform>();
        _height = _rectTransform.rect.height;
    }

    public float Align(Vector3 position)
    {
        Debug.Log("Double: " + position);
        
        transform.position = position;
        return _height;
    }

    public string ToCWorld()
    {
        int a = int.Parse(_fieldA.text);
        int b = int.Parse(_fieldB.text);

        return $"{name.text} {a}, {b}\n";
    }
}