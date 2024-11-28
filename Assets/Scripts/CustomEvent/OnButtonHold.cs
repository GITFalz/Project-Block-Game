using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool holding = false;
    
    public bool IsHolding()
    {
        return holding;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;
    }
}