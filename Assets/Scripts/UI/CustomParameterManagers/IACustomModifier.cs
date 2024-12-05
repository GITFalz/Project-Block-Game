using UnityEngine;

public abstract class IACustomModifier : CustomUI
{ 
    public abstract string GetModifierName();
    public abstract Transform GetTransform();
}