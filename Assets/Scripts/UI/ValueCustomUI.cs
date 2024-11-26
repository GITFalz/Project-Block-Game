using System.Globalization;
using TMPro;
using UnityEngine;

public class ValueCustomUI : MonoBehaviour
{
    public TMP_InputField inputField;
    
    public bool valueChanged = false;
    
    public void UpdateUI(string value)
    {
        inputField.text = value;
    }
    
    public void OnSwiping()
    {
        CinematicKeyFrameSettingsManager.swipping = true;
        CinematicKeyFrameSettingsManager.swippingInputField = inputField;
    }
    
    public string GetValue()
    {
        return inputField.text;
    }
    
    public void OnValueChanged()
    {
        valueChanged = true;
    }
}