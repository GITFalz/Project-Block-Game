using UnityEngine;

public interface I_CustomModifier : I_CustomUi
{
    void Init(CustomUICollectionManager modifierManager);
    string GetModifierName();
    Transform GetTransform();
}