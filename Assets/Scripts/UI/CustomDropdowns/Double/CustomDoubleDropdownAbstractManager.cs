using TMPro;
using UnityEngine;

public abstract class CustomDoubleDropdownAbstractManager : CustomDropdownAbstractManager
{
    [Header("Properties")]
    public TypeOrText name;
    
    protected TMP_Dropdown Dropdown1;
    protected TMP_Dropdown Dropdown2;
    public abstract override void Init(CustomUICollectionManager collectionManager);

    public override float Align(Vector3 position)
    {
        transform.position = position;
        return height;
    }

    public abstract override string ToCWorld();
}