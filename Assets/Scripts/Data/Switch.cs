using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch
{
    private Func<bool> function;
    private bool b = true;

    public Switch(Func<bool> function)
    {
        this.function = function;
    }

    public bool CanSwitch()
    {
        if (b && function())
        {
            b = false;
            return true;
        }
        else if (!b && !function())
        {
            b = true;
        }
        return false;
    }
}
