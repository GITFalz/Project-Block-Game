using UnityEngine;

public class DoubleButtonHoldManager : MonoBehaviour
{
    public OnButtonHold buttonA;
    public OnButtonHold buttonB;

    public int IsHolding()
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();

        if (b)
            return 2;
        if (a)
            return 1;
        
        return 0;
    }
    
    public Vector2 IsHoldingVector()
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();

        return new Vector2(a ? 1 : 0, b ? 1 : 0);
    }
    
    public bool IsHoldingVector(out Vector2 vector)
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();

        vector = new Vector2(a ? 1 : 0, b ? 1 : 0);
        return a || b;
    }
}