using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject commands;
    public TextureGeneration textureGeneration;

    public void SwitchScene(string scene)
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

    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void CloseAll()
    {
        commands?.SetActive(false);
        textureGeneration?.SetMove(false);
    }
}