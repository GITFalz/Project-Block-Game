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
    public GameCommandSystem commandSystem;
    
    public StateMachine stateMachine;
    public GameObject points;

    private char[] prefixes = { '!' };

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
        if (args.Length >= 1 && args[0].StartsWith('!'))
        {
            Console.Log("Accessing game commands...");
            args[0] = args[0].Trim('!');
            commandSystem.ExecuteCommand(args);
        }
        else
        {
            await CommandTest(0, SystemCommands.baseCommands);
            inputField.text = "";
            args = null;
        }
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

    public async Task<string> Do_Load()
    {
        string result = await CommandTest(1, SystemCommands.loadCommands);
        if (result.Equals(""))
            return "";
        
        string[] paths = args[1].Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        
        string mainPath = FileManager.EditorFolderPath;
        
        foreach (string path in paths)
        {
            mainPath = Path.Combine(mainPath, path);
        }

        mainPath += mainPath.EndsWith(".cworld") ? "" : ".cworld";

        if (!File.Exists(mainPath))
            return Console.Log("The file doesn't exist");

        await writer.Load(mainPath);
        return "";
    }
    
    public async Task<string> Do_Clear()
    {
        Console.Clear();
        return await Task.Run(() => "Clearing console...");
    }

    public Task<string> Do_LoadClear()
    {
        ChunkGenerationNodes.Clear();
        Console.Log("Cleared nodes!");
        return Task.FromResult("");
    }
    
    public async Task<string> Do_Cinematic()
    {
        return await CommandTest(1, SystemCommands.cinematicCommands);
    }
    
    public Task<string> Do_CinematicEnable()
    {
        stateMachine.doCinematic = true;
        return Task.FromResult("");
    }
    
    public Task<string> Do_CinematicDisable()
    {
        stateMachine.doCinematic = false;
        return Task.FromResult("");
    }
    
    public async Task<string> Do_CinematicPosition()
    {
        if (stateMachine == null)
            return "";

        Vector3 position = stateMachine.player.GetComponent<Rigidbody>().position;
        GameObject instance = Instantiate(points, position, Quaternion.identity);
        stateMachine.points.Add(instance.transform);
        stateMachine.times.Add(5);

        return await Task.FromResult("");
    }
}

public static class SystemCommands
{
    public static Dictionary<string, Func<ConsoleManager, Task<string>>> baseCommands = new Dictionary<string, Func<ConsoleManager, Task<string>>>
    {
        { "load", (c) => c.Do_Load() },
        { "clear", (c) => c.Do_Clear() },
        { "cinematic", (c) => c.Do_Cinematic() },
    };
    
    public static Dictionary<string, Func<ConsoleManager, Task<string>>> loadCommands = new Dictionary<string, Func<ConsoleManager, Task<string>>>
    {
        { "clear", (c) => c.Do_LoadClear() },
    };
    
    public static Dictionary<string, Func<ConsoleManager, Task<string>>> cinematicCommands = new Dictionary<string, Func<ConsoleManager, Task<string>>>
    {
        { "enable", (c) => c.Do_CinematicEnable() },
        { "disable", (c) => c.Do_CinematicDisable() },
        { "position", (c) => c.Do_CinematicPosition() },
    };
}