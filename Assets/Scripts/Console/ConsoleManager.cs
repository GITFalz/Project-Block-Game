using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public GameObject console;
    public TMP_InputField inputField;

    public WMWriter writer;

    private string[] args;
    
    public void CloseConsole()
    {
        console.SetActive(false);
    }

    public void OpenConsole()
    {
        console.SetActive(true);
    }

    public async void ExecuteCode()
    {
        string input = inputField.text;
        input = input.Replace("\u200B", "").Trim();
        args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        await CommandTest(0, baseCommands);
        inputField.text = "";
        args = null;
    }
    
    private async Task<string> CommandTest(int index, Dictionary<string, Func<ConsoleManager, Task<string>>> commands)
    {
        string command = (string)args[index].Trim();
        
        if (commands.TryGetValue(command, out var function))
        {
            return await function(this);
        }
        
        return $"The command {command} doesn't exist";
    }

    private async Task<string> Do_Load()
    {
        string result = await CommandTest(1, loadCommands);
        if (result.Equals(""))
            return "";
        
        if (!args[1].EndsWith(".cworld"))
            args[1] += ".cworld";
        
        string[] paths = args[1].Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        
        string mainPath = FileManager.EditorFolderPath;
        
        foreach (string path in paths)
        {
            mainPath = Path.Combine(mainPath, path);
        }

        if (!File.Exists(mainPath))
            return Console.Log("The file doesn't exist");

        await writer.Load(mainPath);
        return "";
    }

    public Task<string> Do_LoadClear()
    {
        ChunkGenerationNodes.Clear();
        return Task.FromResult("");
    }
    
    private Dictionary<string, Func<ConsoleManager, Task<string>>> baseCommands = new Dictionary<string, Func<ConsoleManager, Task<string>>>
    {
        { "load", (c) => c.Do_Load() },
    };
    
    private Dictionary<string, Func<ConsoleManager, Task<string>>> loadCommands = new Dictionary<string, Func<ConsoleManager, Task<string>>>
    {
        { "clear", (c) => c.Do_LoadClear() },
    };
}