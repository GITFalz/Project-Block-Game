using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action action;
    
    public void OnPointerDown(PointerEventData eventData)
    { 
        ButtonHoldManager.currentAction = action;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonHoldManager.currentAction = null;
    }
}