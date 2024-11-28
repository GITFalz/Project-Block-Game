using UnityEngine;

public class TripleButtonHoldManager : MonoBehaviour
{
    public OnButtonHold buttonA;
    public OnButtonHold buttonB;
    public OnButtonHold buttonC;

    public int IsHoldingInt()
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();
        bool c = buttonC.IsHolding();

        if (c)
            return 3;
        if (b)
            return 2;
        if (a)
            return 1;
        
        return 0;
    }
    
    public bool IsHoldingInt(out int i)
    {
        i = -1;
        
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();
        bool c = buttonC.IsHolding();

        if (c)
        {
            i = 2;
            return true;
        }
        if (b)
        {
            i = 1;
            return true;
        }
        if (a)
        {
            i = 0;
            return true;
        }

        return false;
    }
    
    public Vector3 IsHoldingVector()
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();
        bool c = buttonC.IsHolding();

        return new Vector3(a ? 1 : 0, b ? 1 : 0, c ? 1 : 0);
    }
    
    public bool IsHoldingVector(out Vector3 vector)
    {
        bool a = buttonA.IsHolding();
        bool b = buttonB.IsHolding();
        bool c = buttonC.IsHolding();

        vector = new Vector3(a ? 1 : 0, b ? 1 : 0, c ? 1 : 0);
        return a || b || c;
    }
}