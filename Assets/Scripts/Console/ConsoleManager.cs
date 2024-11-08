using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public GameObject console;
    
    public void CloseConsole()
    {
        console.SetActive(false);
    }

    public void OpenConsole()
    {
        console.SetActive(true);
    }
}