using UnityEngine;

public abstract class CustomUI : MonoBehaviour
{
    [Header("Parameters")] 
    public TypeOrText name;
    
    [Header("Print")]
    public bool doName;
    
    [Header("Base Properties")]
    public float height = 30;
    
    public abstract void Init(CustomUICollectionManager collectionManager);
    public abstract float Align(Vector3 position);
    public abstract string ToCWorld();
}