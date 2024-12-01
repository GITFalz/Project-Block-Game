using System;
using UnityEngine;
using UnityEngine.UI;

public class TripleButtonHoldManager : MonoBehaviour
{
    public OnButtonHold buttonA;
    public OnButtonHold buttonB;
    public OnButtonHold buttonC;

    private Button _bA;
    private Button _bB;
    private Button _bC;
    
    private bool _a;
    private bool _b;
    private bool _c;

    private void Start()
    {
        _bA = buttonA.GetComponent<Button>();
        _bB = buttonB.GetComponent<Button>();
        _bC = buttonC.GetComponent<Button>();
        
        _bA.onClick.AddListener(() => { _a = true; });
        _bB.onClick.AddListener(() => { _b = true; });
        _bC.onClick.AddListener(() => { _c = true; });
    }

    public bool IsClickingInt(out int i)
    {
        if (_a)
        {
            i = 0;
            _a = false;
            return true;
        }
        if (_b)
        {
            i = 1;
            _b = false;
            return true;
        }
        if (_c)
        {
            i = 2;
            _c = false;
            return true;
        }
        
        i = -1;
        return false;
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