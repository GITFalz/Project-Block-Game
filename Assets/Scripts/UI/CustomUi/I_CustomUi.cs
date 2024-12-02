using UnityEngine;

public interface I_CustomUi
{
    void Init(I_GroupedUi collectionManager);
    float Align(Vector3 position);
    string ToCWorld();
}