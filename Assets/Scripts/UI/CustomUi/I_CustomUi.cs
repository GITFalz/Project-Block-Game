using UnityEngine;

public interface I_CustomUi
{
    void Init(CustomUICollectionManager collectionManager);
    float Align(Vector3 position);
    string ToCWorld();
}