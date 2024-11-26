using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonHoldMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CinematicPositionKeyFrameManager go;
        
    public void OnPointerDown(PointerEventData eventData)
    {
        CinematicTimelineManager.scrollRect.enabled = false;
        CinematicKeyFrameSettingsManager.moving = true;
        CinematicKeyFrameSettingsManager.movingObject = go;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CinematicKeyFrameSettingsManager.moving = false;
        CinematicTimelineManager.scrollRect.enabled = true;
    }
}