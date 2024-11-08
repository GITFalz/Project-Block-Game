using TMPro;
using UnityEngine;

public class ErrorHandler : MonoBehaviour
{
    public Transform ErrorBase;
    public TMP_Text ErrorBaseText;
    public Transform ErrorLog;
    public TMP_InputField ErrorLogField;
    
    public void Hide()
    {
        ErrorBase.gameObject.SetActive(false);
        ErrorLog.gameObject.SetActive(false);
    }

    public void SwitchLog()
    {
        ErrorLog.gameObject.SetActive(!ErrorBase.gameObject.activeSelf);
    }

    public void Error(string message, string log)
    {
        ErrorBase.gameObject.SetActive(true);

        ErrorBaseText.text = message;
        ErrorLogField.text = log;
    }
}