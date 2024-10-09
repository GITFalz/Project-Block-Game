using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwitch<T>
{
    private Func<bool> function;
    private List<T> states = new List<T>();

    private bool b = true;
    private int size;
    private int index = 0;

    public T currentState;

    public StateSwitch(Func<bool> function, params T[] states)
    {
        this.function = function;

        size = states.Length;

        if (size > 0)
        {
            currentState = states[0];

            for (int i = 0; i < size; i++)
            {
                this.states.Add(states[i]);
            }
        }
    }

    public bool CanSwitch()
    {
        if (b && function())
        {
            b = false;
            index++; index %= size;
            currentState = states[index];
            return true;
        }
        else if (!b && !function())
        {
            b = true;
        }
        return false;
    }

    public void Switch()
    {
        index++; index %= size;
        currentState = states[index];
    }
}
