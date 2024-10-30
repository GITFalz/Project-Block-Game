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

    public static void Hide()
    {
        GameObject.Find("PopupErrorPanel").transform.Find("ErrorBase").gameObject.SetActive(false);
    }
}