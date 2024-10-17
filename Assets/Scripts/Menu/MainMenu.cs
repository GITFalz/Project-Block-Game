using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject commands;
    public GameObject cworld;
    public void NewWorld(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SwitchToCommands(GameObject currentMenu)
    {
        currentMenu.SetActive(false);
        commands.SetActive(true);
    }
    
    public void SwitchToCWorld(GameObject currentMenu)
    {
        currentMenu.SetActive(false);
        cworld.SetActive(true);
    }
}