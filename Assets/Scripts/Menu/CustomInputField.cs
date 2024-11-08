using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputField : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField inputField;
    public float scrollSensitivity = 10f;

    private void Start()
    {
        inputField.onSelect.AddListener(delegate { inputField.caretPosition = inputField.text.Length; });

        inputField.scrollSensitivity = scrollSensitivity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inputField.caretPosition = TMP_TextUtilities.GetCursorIndexFromPosition(inputField.textComponent, eventData.pressPosition, Camera.main);
    }
}