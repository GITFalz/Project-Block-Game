using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputField : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField inputField;

    private void Start()
    {
        inputField.onSelect.AddListener(delegate { inputField.caretPosition = inputField.text.Length; });

        inputField.scrollSensitivity = 30f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inputField.caretPosition = TMP_TextUtilities.GetCursorIndexFromPosition(inputField.textComponent, eventData.pressPosition, Camera.main);
    }
}