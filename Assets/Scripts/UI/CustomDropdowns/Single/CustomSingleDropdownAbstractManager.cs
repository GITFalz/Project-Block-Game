using TMPro;
using UnityEngine;

public abstract class CustomSingleDropdownAbstractManager : CustomDropdownAbstractManager
{
    [Header("Properties")]
    public TypeOrText name;
    
    protected TMP_Dropdown Dropdown1;

    public abstract override void Init(CustomUICollectionManager collectionManager);
    
    public override float Align(Vector3 position)
    {
        transform.position = position;
        return _height;
    }

    public abstract override string ToCWorld();
}