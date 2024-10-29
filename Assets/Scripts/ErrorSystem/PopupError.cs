using UnityEngine;
using UnityEngine.UI;

public static class PopupError
{
    public static void Popup(string message)
    {
        GameObject text = GameObject.Find("PopupErrorText");
        text.GetComponent<Text>().text = message;
        GameObject.Find("PopupErrorPanel").SetActive(true);
    }

    public static void Hide()
    {
        GameObject.Find("PopupErrorPanel").SetActive(false);
    }
}