using TMPro;
using UnityEngine;

public abstract class CustomInputAbstractManager : MonoBehaviour, I_CustomUi
{
    [Header("Properties")] 
    public TypeOrText name;
    public bool doName = true;
    
    protected TMP_Text _text;
    
    protected RectTransform _rectTransform;
    protected float _height;

    public abstract void Init(CustomUICollectionManager collectionManager);

    public float Align(Vector3 position)
    {
        Debug.Log("Double: " + position);
        
        transform.position = position;
        return _height;
    }

    public abstract string ToCWorld();
}