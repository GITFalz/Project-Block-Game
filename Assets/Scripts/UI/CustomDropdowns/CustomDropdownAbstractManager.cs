using UnityEngine;

public abstract class CustomDropdownAbstractManager : CustomUI
{
    [Header("Properties")]
    public CustomInputAbstractManager input;
    public float height = 30;

    public abstract override void Init(CustomUICollectionManager collectionManager);

    public abstract override float Align(Vector3 position);
    
    public abstract override string ToCWorld();
}