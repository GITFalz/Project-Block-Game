using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WMWriter : MonoBehaviour
{
    public static WMWriter instance;

    public TMP_InputField inputField;
    public TextureGeneration textureGeneration;
    public CWorldHandler handler;
    public BlockManager blockManager;
    public FileManager fileManager;
    public CWorldMenu menu;
    public ConsoleTextManager consoleTextManager;

    public bool simpleLoad = false;

    [HideInInspector]
    public WriterManager writerManager;

    private string currentPath;

    private int Index => writerManager.index;
    private string[] Args => writerManager.args;
    public string Arg => Index < Args.Length ? Args[Index] : null;

    private void Start()
    {
        ChunkGenerationNodes.Set();
        
        writerManager = new WriterManager(this, false);
            
        BlockManager.Init();
        handler.Init();
        fileManager.Init();
        menu?.Init();
        consoleTextManager?.Init();

        LoadOnEnter();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public async void ExecuteCode()
    {
        await Load(menu.currentFilePath);
    }

    public async Task Main()
    {
        while (writerManager.index <= writerManager.args.Length)
        {
            if (writerManager.index == writerManager.args.Length)
                break;

            int message = await CommandTest(writerManager.index, types);
            
            if (message == -1)
            {
                Console.Log(Regex.Replace(writerManager.GetLine(), "\t", "") + " <<");
                break;
            }

            writerManager.index++;
        }
        
        blockManager.UpdateInspector();
    }


    public async void LoadOnEnter()
    {
        if (!FileManager.EditorFolderPath.Equals(""))
        {
            Console.Log("Loading files...");
            string[] files = GetCWorldFilesInFolder();
            string f = files.Length <= 1 ? "file was" : "files were";
            Console.Log(files.Length + $" {f} found");
            
            foreach (var filePath in files)
            {
                await Load(filePath);
            }
        }
    }

    public async Task<int> Load(string path)
    {
        currentPath = path;

        WriterManager oldWriter = writerManager;
        writerManager = new WriterManager(this, true);

        string content;

        try
        {
            content = await File.ReadAllTextAsync(path);
        }
        catch (FileNotFoundException)
        {
            Console.Log($"File not found: {path}");
            return -1;
        }

        writerManager.savePath = path;
        Console.Log("Initializing lines...");
        await writerManager.InitLines(content);
        Console.Log("Done!");

        try
        {
            await Main();
            if (!simpleLoad)
            {
                textureGeneration?.SetMove(true);
                
                menu?.Save(writerManager.savePath, writerManager.fileContent);

                if (!ChunkGenerationNodes.sampleDisplayName.Equals(""))
                {
                    textureGeneration?.UpdateTexture(ChunkGenerationNodes.sampleDisplayName);
                }
            }
        }
        catch (Exception e)
        {
            Console.Log("A problem occured when running the main loop");
            return -1;
        }
        
        writerManager = oldWriter;

        return 1;
    }

    public string[] GetCWorldFilesInFolder()
    {
        List<string> currentPaths = new List<string>();
        List<string> checkedPaths = new List<string>();
        List<string> toBeChecked = new List<string>();
        List<string> files = new List<string>();
        
        string[] filePaths = Directory.GetFiles(FileManager.ExecuteOnEnterFolderPath, "*.cworld");
        files.AddRange(filePaths);
        
        currentPaths = FileManager.GetFolderPaths(FileManager.ExecuteOnEnterFolderPath);

        while (true)
        {
            foreach (var path in currentPaths)
            {
                var newToBeChecked = FileManager.GetFolderPaths(path);
                toBeChecked.AddRange(newToBeChecked);
                checkedPaths.Add(path);
            }
            currentPaths.Clear();
            currentPaths.AddRange(toBeChecked);
            toBeChecked.Clear();

            if (currentPaths.Count == 0)
                break;
        }
        
        foreach (var path in checkedPaths)
        {
            filePaths = Directory.GetFiles(path, "*.cworld");
            files.AddRange(filePaths);
        }

        return files.ToArray();
    }
    
    /**
     * 
     */
    


    public async void SaveFile()
    {
        writerManager.args = await writerManager.InitLines(inputField.text);
            
        bool quitNext = false;
        foreach (string value in writerManager.args)
        {
            if (quitNext)
            {
                menu.Save(value + ".cworld", inputField.text);
                break;
            }
            
            if (value.Equals("Save"))
            {
                quitNext = true;
            }
        }
    }

    public string DisplayContent(string filePath)
    {
        string content = File.ReadAllText(filePath);
        Debug.Log(content);
        inputField.text = content;
        
        return "";
    }

    public void Clear()
    {
        inputField.text = "";
    }


    public Task<int> On_Save()
    {
        Increment();
        writerManager.saveFile = Arg;
        return Task.FromResult(1);
    }

    public async Task<int> On_Use()
    {
        writerManager.index++;
        string[] path = writerManager.args[writerManager.index].Split(new[] { '/', '\'', '|', '>' }, StringSplitOptions.RemoveEmptyEntries);
        string mainPath = FileManager.EditorFolderPath;
        
        foreach (string p in path)
        {
            mainPath = Path.Combine(mainPath, p);
        }
        
        mainPath += ".cworld";

        if (mainPath.Equals(currentPath))
            return await Error($"Infinite loop is occuring in {currentPath}");

        return await Load(mainPath);
    }
    
    
    
    public async Task<int> CommandTest(int index, Dictionary<string, Func<WMWriter, Task<int>>> commands)
    {
        string command = writerManager.args[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<WMWriter, Task<int>> func))
        {
            return await func(this);
        }
        return -1;
    }

    public int IsComment(int index)
    {
        if (writerManager.args[index].Trim().StartsWith("//"))
        {
            index++;
            while (!writerManager.args[index].Trim().EndsWith("//"))
            {
                index++;
                if (index == writerManager.args.Length)
                {
                    return index;
                }
            }
        }
        return index;
    }

    public Task<int> Error(string message, bool displayIndex = true)
    {
        if (displayIndex)
            message += " at string index : " + writerManager.index;
                
        Console.Log(message);
        return Task.FromResult(-1);
    }

    private bool MaxIndex(int i)
    {
        return writerManager.index + i < writerManager.args.Length;
    }
    

    public async Task<int> CommandsTest(Dictionary<string, Func<WMWriter, Task<int>>> commands)
    {
        bool done = false;
        while (!done)
        { 
            int message = await CommandTest(writerManager.index, commands);
            if(message == -1) return await Error("Problem in the label found");
            if(message == 1) done = true;
        }
        return 0;
    }
    
    
    

    public async Task<int> On_Sample()
    {
        Increment();
        
        if (await CommandsTest(writerManager.worldSampleManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddSamples(writerManager.currentName))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }

        if (await CommandsTest(writerManager.worldSampleManager.settings) == -1) return await Error("Problem in the sample settings found");
        return 0;
    }

    public async Task<int> On_Biome()
    {
        Increment();
        
        if (await CommandsTest(writerManager.worldBiomeManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddBiomes(writerManager.currentBiomeName))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(writerManager.worldBiomeManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    public async Task<int> On_Block()
    {
        Increment();
        
        writerManager.worldBlockManager.SetBlock(new CWorldBlock());
        
        if (await CommandsTest(writerManager.worldBlockManager.labels) == -1) return await Error("Problem in the label found");

        writerManager.worldBlockManager.BlockNode.blockName = writerManager.currentBlockName;
        
        if (await CommandsTest(writerManager.worldBlockManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    public async Task<int> On_Map()
    {
        Increment();

        if (!await ChunkGenerationNodes.AddMap(writerManager.NextLine()))
            return await Error("Watch out!, a map node already exists in the system. To replace the node write 'Map Force'");

        if (writerManager.NextLine(1).Equals("Force"))
            Increment();
        
        Increment();
        
        if (await CommandsTest(writerManager.worldMapManager.settings) == -1) return await Error("Problem in the map settings found");
        return 0;
    }
    
    public async Task<int> On_Modifier()
    {
        Increment();
        
        if (await CommandsTest(writerManager.worldModifierManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddModifier(writerManager.currentModifierName))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(writerManager.worldModifierManager.settings) == -1) return await Error("Problem in the modifier settings found");
        return 0;
    }

    public Task<int> SkipNode()
    {
        try
        {
            while (true)
            {
                if (Arg.Equals("}") && writerManager.index + 1 == writerManager.args.Length ||
                    Arg.Equals("}") && types.ContainsKey(writerManager.args[writerManager.index + 1]))
                    return Task.FromResult(0);

                Increment();
            }
        }
        catch (Exception e)
        {
            return Error(e.Message);
        }
    }

    public Task<int> On_Name(ref string name)
    {
        Increment();
        if (!Arg.Equals("=")) return Error("No '=' found");
        Increment();
        if (Arg.Equals(")")) return Error("'name' expected but ) found");
        name = Arg;
        Increment();
        return Task.FromResult(2);
    }

    public async Task<int> On_BlockSetTextures()
    {
        int i = 0;
        while (true)
        {
            if (await GetNextInt(out int t) == -1)
                return await Error("A problem occured when trying to get the texture indexes, check if they are indeed integers");

            if (!BlockManager.SetUv(writerManager.worldBlockManager.BlockNode.index, i, t))
                return await Error("A problem occured when trying set the uv texture index, too many indices could be the problem");

            i++;
            if (Arg.Equals(",")) continue;
            return 0;
        }
    }

    public async Task<int> On_BlockSetPriority()
    {
        if (await GetNextInt(out int t) == -1)
            return await Error("A problem occured when trying to get the priority value, make sure it's an integer");

        if (!BlockManager.SetPriority(writerManager.worldBlockManager.BlockNode.index, t))
            return await Error("Couldn't find block to set priority");

        return 0;
    }

    public async Task<int> On_Settings(Dictionary<string, Func<WMWriter, Task<int>>> commands)
    {
        Increment();
        return await CommandsTest(commands) == -1 ? await Error("Problem with the settings") : 0;
    }

    public Task<int> Increment(int i, int result)
    {
        writerManager.index += i;
        return Task.FromResult(result);
    }
    
    public void Increment(int i = 1)
    {
        writerManager.index += i;
    }

    public Task<int> On_Display()
    {
        writerManager.index++;
        ChunkGenerationNodes.sampleDisplayName = writerManager.currentName;
        
        return Task.FromResult(0);
    }
    
    
    
    
    #region value getters

    public Task<int> GetNextFloat(out float value)
    {
        value = 0;

        try
        {
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float amplitude)) return Error("No valid float value found");
            Increment();

            value = amplitude;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Error("There are missing parameters, the line should be written like: 'option : value1, value2'");
        }
        
        catch (Exception ex)
        {
            return Error($"Error {ex}");
        }
    }
    
    public Task<int> GetNextInt(out int value)
    {
        value = 0;

        try
        {
            Increment();
            if (!int.TryParse(Arg, out int result)) return Error("No valid int found");
            Increment();

            value = result;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Error("There are missing parameters, the line should be written like: 'option : value1, value2'");
        }
        
        catch (Exception ex)
        {
            return Error($"Error {ex}");
        }
    }
    
    public Task<int> GetNext2Floats(out Vector2 floats)
    {
        floats = Vector2.zero;

        try
        {
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) return Error("No valid float found");
            Increment();
            if (!Arg.Equals(",")) return Error("',' is missing");
            Increment();
            if (!float.TryParse(Arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) return Error("No valid float found");
            Increment();

            floats.x = x;
            floats.y = y;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Error("There are missing parameters, the line should be written like: 'option : value1, value2'");
        }
        
        catch (Exception ex)
        {
            return Error($"Error {ex}");
        }
    }
    
    public Task<int> GetNext2Ints(out Vector2Int ints)
    {
        ints = Vector2Int.zero;

        try
        {
            Increment();
            if (!int.TryParse(Arg, out int x)) return Error("No valid int found");
            Increment();
            if (!Arg.Equals(",")) return Error("',' is missing");
            Increment();
            if (!int.TryParse(Arg, out int y)) return Error("No valid int found");
            Increment();

            ints.x = x;
            ints.y = y;
            return Task.FromResult(0);
        }
        catch (IndexOutOfRangeException)
        {
            return Error("There are missing parameters, the line should be written like: 'option : value1, value2'");
        }
        
        catch (Exception ex)
        {
            return Error($"Error {ex}");
        }
    }
    
    public void GetNext(out string value)
    {
        Increment();
        value = Arg;
    }
    
    public void GetNextValue(out string value)
    {
        GetNext(out value);
        Increment();
    }
    
    public Task<int> GetNext2Values(out string[] values)
    {
        values = new[] {"", ""};

        Increment();
        values[0] = Arg;
        Increment();
        if (!Arg.Equals(",")) return Error("',' is missing");
        Increment();
        values[1] = Arg;
        Increment();

        return Task.FromResult(0);
    }

    #endregion
    
    
    
    
    public Dictionary<string, Func<WMWriter, Task<int>>> types = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "Save", async (w) => await w.On_Save() },
        { "Use", async (w) => await w.On_Use() },
        { "Sample", async (w) => await w.On_Sample() },
        { "Biome", async (w) => await w.On_Biome() },
        { "Block", async (w) => await w.On_Block() },
        { "Modifier", async (w) => await w.On_Modifier() },
        { "Map", async (w) => await w.On_Map() },
    };
}