using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class PopupError
{
    public static void Popup(string message)
    {
        GameObject.Find("PopupErrorPanel").transform.Find("ErrorBase").gameObject.SetActive(true);
        GameObject text = GameObject.Find("PopupErrorText");
        text.GetComponent<TMP_Text>().text = message;
    }

    public static void Log(string log)
    {
        GameObject.Find("PopupErrorPanel").transform.Find("ErrorLog").gameObject.SetActive(true);
        GameObject text = GameObject.Find("PopupLogText");
        text.GetComponent<TMP_Text>().text = log;
    }

    public static void Log(string message, string log)
    {
        Popup(message);
    }

    public static void Hide()
    {
        GameObject.Find("PopupErrorPanel").transform.Find("ErrorBase").gameObject.SetActive(false);
    }
}