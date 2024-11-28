using UnityEngine;

public class SingleButtonHoldManager : MonoBehaviour
{
    public OnButtonHold buttonA;
    
    public bool IsHolding()
    {
        return buttonA.IsHolding();
    }
}