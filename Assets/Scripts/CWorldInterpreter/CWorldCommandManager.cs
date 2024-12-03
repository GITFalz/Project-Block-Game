using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class CWorldCommandManager
{
    public static WriterManager _writerManager = new();
    public static int Index => _writerManager.index;
    public static string[] Args => _writerManager.args;
    public static int ArgsLength => _writerManager.args.Length;
    public static string Arg => Index < Args.Length ? Args[Index] : null;
    public static string CurrentPath;
    public static string CurrentFileConent;
    
    public static async Task Main()
    {
        Console.Log("Executing code...");
        while (Index <= ArgsLength)
        {
            if (Index == ArgsLength)
                break;
            
            int message = await CommandTest(Index, CWorldNodesManager.Types);
            
            if (message == -1)
            {
                Debug.Log("An error occured");
                Console.Log(Regex.Replace(_writerManager.GetLine(), "\t", "") + " <<");
                throw new Exception("An error occured");
            }
            
            Increment();
        }
        
        if (!ChunkGenerationNodes.localLoad)
            BlockManager.Instance.UpdateInspector();
        
        Console.Log("Done with: " + CurrentPath);
        Console.Log(">--------------------<");
    }
    
    public static async Task<int> CommandTest(int index, Dictionary<string, Func<Task<int>>> commands)
    {
        string command = Args[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<Task<int>> func))
        {
            return await func();
        }
        Console.Log("The command " + command + " is not recognized");
        return -1;
    }
    
    public static async Task<int> CommandsTest(Dictionary<string, Func<Task<int>>> commands)
    {
        bool done = false;
        while (!done)
        { 
            int message = await CommandTest(Index, commands);
            if(message == -1)
            {
                Console.Log("A command wasn't recognized");
                return -1;
            }
            if(message == 1) done = true;
        }
        Debug.Log("---Done---");
        return 0;
    }
    
    public static async Task<int> Load(string path)
    {
        CurrentPath = path;

        WriterManager oldWriter = _writerManager;
        string oldContent = CurrentFileConent;
        
        _writerManager = new WriterManager();
        
        try {
            CurrentFileConent = await File.ReadAllTextAsync(CurrentPath);
        }
        catch (FileNotFoundException) {
            Console.Log($"File not found: {CurrentPath}");
            return -1;
        }
        
        Console.Log("Loading: " + CurrentPath + "\n>> Initializing lines...");
        
        if (await LoadContent(CurrentFileConent) == -1)
            return await Console.LogErrorAsync("An error occured when loading the content");

        try
        {
            CWorldEditorManager.Instance.UpdateTexture();
        }
        catch (Exception e)
        {
            Console.Log("Not in editor mode, can't update texture");
        }
        
        CurrentFileConent = oldContent;
        _writerManager = oldWriter;

        return 0;
    }
    
    public static async Task<int> LoadContent(string content)
    {
        try {
            await _writerManager.InitLines(content);
        }
        catch (NullReferenceException)
        {
            return await Console.LogErrorAsync("Falz forgot to init the writerManager that fucking idiot, please tell him");
        }

        Console.Log("Done!");

        try {
            await Main();
        }
        catch (Exception e) {
            return await Console.LogErrorAsync("A problem occured when running the main loop with exception:\n " + e.Message);
        }

        return 1;
    }
    
    
    
    #region value getters

    public static Task<int> GetNextFloat(out float value)
    {
        value = 0;

        try
        {
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float amplitude)) return Task.FromResult(Console.LogError("No valid float value found")) ;
            Increment();

            value = amplitude;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Task.FromResult(Console.LogError("There are missing parameters, the line should be written like: 'option : value1, value2'"));
        }
        
        catch (Exception ex)
        {
            return Task.FromResult(Console.LogError($"Error {ex}"));
        }
    }
    
    public static  Task<int> GetNextInt(out int value)
    {
        value = 0;

        try
        {
            Increment();
            if (!int.TryParse(Arg, out int result)) return Task.FromResult(Console.LogError("No valid int found"));
            Increment();

            value = result;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Task.FromResult(Console.LogError("There are missing parameters, the line should be written like: 'option : value1, value2'"));
        }
        
        catch (Exception ex)
        {
            return Task.FromResult(Console.LogError($"Error {ex}"));
        }
    }
    
    public static Task<int> GetNext2Floats(out Vector2 floats)
    {
        floats = Vector2.zero;

        try
        {
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) return Task.FromResult(Console.LogError("No valid float found"));
            Increment();
            if (!Arg.Equals(",")) return Task.FromResult(Console.LogError("',' is missing"));
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) return Task.FromResult(Console.LogError("No valid float found"));
            Increment();

            floats.x = x;
            floats.y = y;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Task.FromResult(Console.LogError("There are missing parameters, the line should be written like: 'option : value1, value2'"));
        }
        
        catch (Exception ex)
        {
            return Task.FromResult(Console.LogError($"Error {ex}"));
        }
    }
    
    public static Task<int> GetNext2Ints(out Vector2Int ints)
    {
        ints = Vector2Int.zero;

        try
        {
            Increment();
            if (!int.TryParse(Arg, out int x)) return Task.FromResult(Console.LogError("No valid int found"));
            Increment();
            if (!Arg.Equals(",")) return Task.FromResult(Console.LogError("',' is missing"));
            Increment();
            if (!int.TryParse(Arg, out int y)) return Task.FromResult(Console.LogError("No valid int found"));
            Increment();

            ints.x = x;
            ints.y = y;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Task.FromResult(Console.LogError("There are missing parameters, the line should be written like: 'option : value1, value2'"));
        }
        
        catch (Exception ex)
        {
            return Task.FromResult(Console.LogError($"Error {ex}"));
        }
    }
    
    public static Task<int> GetNextNInts(int n, out List<int> ints)
    {
        ints = new List<int>();

        //Check if there are enough arguments
        if (n <= 0)
            return Task.FromResult(0);
        
        int v;
        
        //Get the first int
        Increment();
        if (!int.TryParse(Arg, out v)) return Task.FromResult(Console.LogError("No valid int found"));
        ints.Add(v);
        
        for (int i = 0; i < n-1; i++)
        {
            //loop over ", int" n-1 times
            Increment();
            if (!Arg.Equals(",")) return Task.FromResult(Console.LogError("',' is missing"));
            Increment();
            if (!int.TryParse(Arg, out v)) return Task.FromResult(Console.LogError("No valid int found"));
            ints.Add(v);
        }

        //Increment to the next command
        Increment();
        
        return Task.FromResult(0);
    }

    public static void GetValue(out string value)
    {
        value = Arg;
    }
    
    public static void GetNextValue(out string value)
    {
        Increment();
        value = Arg;
    }
    
    public static Task<int> GetNext2Values(out string[] values)
    {
        values = new[] {"", ""};

        Increment();
        values[0] = Arg;
        Increment();
        if (!Arg.Equals(",")) return Task.FromResult(Console.LogError("',' is missing"));
        Increment();
        values[1] = Arg;
        Increment();

        return Task.FromResult(0);
    }

    #endregion
    
    public static void Increment(int i = 1)
    {
        _writerManager.index += i;
    }
    
    public static Task<int> Increment(int i, int result)
    {
        _writerManager.index += i;
        return Task.FromResult(result);
    }
}