using System;
using UnityEngine;

public class ButtonHoldManager : MonoBehaviour
{
    public static Action currentAction;
    
    private void Update()
    {
        currentAction?.Invoke();
    }
}