using UnityEngine;

public interface I_CustomUi
{
    void Init(CustomCollectionsManager collectionManager);
    float Align(Vector3 position);
    string ToCWorld();
}