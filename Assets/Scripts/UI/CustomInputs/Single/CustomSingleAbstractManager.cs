using TMPro;
using UnityEngine;

public abstract class CustomSingleAbstractManager : CustomInputAbstractManager
{
    protected TMP_InputField _fieldA;

    public override void Init(CustomUICollectionManager collectionManager)
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _text.text = name.type;
    
        _fieldA = transform.Find("Input1").GetComponent<TMP_InputField>();
    
        _rectTransform = transform.GetComponent<RectTransform>();
        _height = _rectTransform.rect.height;
    }

    public float Align(Vector3 position)
    {
        Debug.Log("Single: " + position);
    
        transform.position = position;
        return _height;
    }

    public abstract override string ToCWorld();
}