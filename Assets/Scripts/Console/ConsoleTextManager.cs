using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleTextManager : MonoBehaviour
{
    public TMP_InputField inputField;
    
    public static TMP_InputField consoleText;
    public static List<string> lines;
    public static int lineCount;

    private void Start()
    {
        lines = new List<string>();
        lineCount = 0;
    }

    private void Awake()
    {
        consoleText = inputField;
    }

    public static void UpdateText()
    {
        string text = "";
        foreach (var line in lines)
        {
            text += line + "\n";
        }
        consoleText.text = text;
    }
}

public static class Console
{
    public static void Log(string message)
    {
        if (ConsoleTextManager.lines == null)
            ConsoleTextManager.lines = new List<string>();
            
        if (ConsoleTextManager.lineCount == 100)
            ConsoleTextManager.lines.RemoveAt(0);
        else
            ConsoleTextManager.lineCount++;
        
        ConsoleTextManager.lines.Add(message);

        ConsoleTextManager.UpdateText();
    }
}
