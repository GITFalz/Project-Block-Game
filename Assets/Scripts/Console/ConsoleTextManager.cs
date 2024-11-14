using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleTextManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public List<string> lines;
    public int lineCount;

    public ConcurrentQueue<string> lineQueue = new();

    public void Init()
    {
        lines = new List<string>();
        lineQueue = new ConcurrentQueue<string>();
        lineCount = 0;
    }

    private void Update()
    {
        if (!lineQueue.TryDequeue(out var line))
            return;
        
        if (lineCount == 100)
            lines.RemoveAt(0);
        else
            lineCount++;
    
        lines.Add(line);

        UpdateText();
    }

    private void Awake()
    {
        Console.console = this;
    }

    public void UpdateText()
    {
        string text = "";
        foreach (var line in lines)
        {
            text += line + "\n";
        }
        inputField.text = text;
    }
}

public static class Console
{
    public static ConsoleTextManager console;
    public static string Log(string message)
    {
        console.lineQueue.Enqueue(message);
        return message;
    }

    public static int LineCount()
    {
        return console.lineCount;
    }

    public static bool RemoveLineAt(int index)
    {
        try
        {
            console.lines.RemoveAt(index);
            console.lineCount--;
            console.UpdateText();
            return true;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    public static bool RemoveLast()
    {
        return RemoveLineAt(console.lineCount - 1);
    }
}
