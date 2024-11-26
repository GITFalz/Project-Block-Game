using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonHoldInputField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TMP_InputField inputField;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        CinematicKeyFrameSettingsManager.swipping = true;
        CinematicKeyFrameSettingsManager.swippingInputField = inputField;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CinematicKeyFrameSettingsManager.swipping = false;
    }
}