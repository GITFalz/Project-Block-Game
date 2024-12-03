using UnityEngine;

public abstract class CustomDropdownAbstractManager : MonoBehaviour, I_CustomUi
{
    public CustomInputAbstractManager input;
    
    protected CustomUICollectionManager _collectionsManager;
    protected RectTransform _rectTransform;
    protected float _height;
    
    public abstract void Init(CustomUICollectionManager collectionManager);

    public abstract float Align(Vector3 position);
    
    public abstract string ToCWorld();
}