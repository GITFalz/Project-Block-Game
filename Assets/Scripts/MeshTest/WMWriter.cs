using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WMWriter : MonoBehaviour
{
    public static WMWriter Instance;

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
    private string currentFileConent;

    private int Index => writerManager.index;
    private string[] Args => writerManager.args;
    public string Arg => Index < Args.Length ? Args[Index] : null;

    private void Start()
    {
        ChunkGenerationNodes.Set();
        writerManager = new WriterManager(this, false);
    }

    private void Awake()
    {
        if (Instance!=null) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        
        LoadOnEnter();
    }

    public async void ExecuteCode()
    {
        ChunkGenerationNodes.Clear();
        await Load(menu.currentFilePath);
    }

    public async Task Main()
    {
        Console.Log("Executing code...");
        while (writerManager.index <= writerManager.args.Length)
        {
            if (writerManager.index == writerManager.args.Length)
                break;
            
            int message = await CommandTest(Index, types);
            
            if (message == -1)
            {
                Console.Log(Regex.Replace(writerManager.GetLine(), "\t", "") + " <<");
                throw new Exception("An error occured");
            }

            writerManager.index++;
        }
        
        blockManager.UpdateInspector();
        
        Console.Log("Done with: " + currentPath);
        Console.Log(">--------------------<");
    }


    public async void LoadOnEnter()
    {
        currentPath = FileManager.ExecuteOnEnterFolderPath;
        if (!currentPath.Equals(""))
        {
            Console.Log("Loading files...");
            string[] files = GetCWorldFilesInFolder();
            string f = files.Length <= 1 ? "file was" : "files were";
            Console.Log(files.Length + $" {f} found");

            int i = 0;
            foreach (var filePath in files)
            {
                Console.Log($"File {i}: " + filePath);
                await Load(filePath);
                i++;
            }
        }
    }

    public async Task<int> Load(string path)
    {
        currentPath = path;

        WriterManager oldWriter = writerManager;
        string oldContent = currentFileConent;
        
        writerManager = new WriterManager(this, true);
        
        try {
            currentFileConent = await File.ReadAllTextAsync(currentPath);
        }
        catch (FileNotFoundException) {
            Console.Log($"File not found: {currentPath}");
            return -1;
        }
        
        Console.Log("Loading: " + currentPath + "\n>> Initializing lines...");
        
        try {
            await writerManager.InitLines(currentFileConent);
        }
        catch (NullReferenceException) {
            Console.Log("Falz forgot to init the writerManager that fucking idiot, please tell him");
            return -1;
        }
        
        Console.Log("Done!");

        try {
            await Main();
        }
        catch (Exception e) {
            Console.Log("A problem occured when running the main loop with exception:\n " + e.Message);
            return -1;
        }
        
        if (!simpleLoad)
        {
            textureGeneration?.SetMove(true);
                
            if (!ChunkGenerationNodes.sampleDisplayName.Equals(""))
            {
                Console.Log(">> Drawing texture...");
                textureGeneration?.UpdateTexture(ChunkGenerationNodes.sampleDisplayName);
            }
        }

        currentFileConent = oldContent;
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

    public async void SaveFile()
    {
        currentFileConent = inputField.text;
        writerManager = new WriterManager(this, true);
        writerManager.args = await writerManager.InitLines(currentFileConent);
        await SaveFileAsync();
    }

    public async Task SaveFileAsync()
    {
        if (menu == null)
            return;
        
        Console.Log("Saving file...");

        bool quitNext = false;
        foreach (string value in writerManager.args)
        {
            if (quitNext)
            {
                string path = value + ".cworld";
                FileManager.Save(menu.currentFolderPath, path, currentFileConent);
                break;
            }

            if (value.Equals("Save"))
            {
                quitNext = true;
            }
        }
        Console.Log("File saved!");
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


    public async Task<int> On_Save()
    {
        Increment();
        await SaveFileAsync();
        return 1;
    }

    public async Task<int> On_Use()
    {
        Increment();
        string[] path = writerManager.args[writerManager.index].Split(new[] { '/', '\'', '|', '>' }, StringSplitOptions.RemoveEmptyEntries);
        string mainPath = FileManager.EditorFolderPath;

        string consolePath = "";
        
        foreach (string p in path)
        {
            consolePath += $" {p}";
            mainPath = Path.Combine(mainPath, p);
        }
        
        Console.Log("Using: " + consolePath);
        
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
        Console.Log("The command " + command + " is not recognized");
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
    

    public async Task<int> CommandsTest(Dictionary<string, Func<WMWriter, Task<int>>> commands)
    {
        bool done = false;
        while (!done)
        { 
            int message = await CommandTest(writerManager.index, commands);
            if(message == -1)
            {
                Console.Log("A command wasn't recognized");
                return -1;
            }
            if(message == 1) done = true;
        }
        return 0;
    }
    
    
    

    public async Task<int> On_Sample()
    {
        Increment();
        
        if (await CommandsTest(CWorldSampleManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddSamples(CWorldSampleManager.name))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }

        if (await CommandsTest(CWorldSampleManager.settings) == -1) return await Error("Problem in the sample settings found");
        
        return 0;
    }

    public async Task<int> On_Biome()
    {
        Increment();
        
        if (await CommandsTest(CWorldBiomeManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddBiomes(CWorldBiomeManager.name))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(CWorldBiomeManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    public async Task<int> On_Block()
    {
        Increment();
        
        CWorldBlockManager.SetBlock(new CWorldBlock());
        
        if (await CommandsTest(CWorldBlockManager.labels) == -1) return await Error("Problem in the label found");

        CWorldBlockManager.BlockNode.blockName = CWorldBlockManager.name;
        
        if (await CommandsTest(CWorldBlockManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    public async Task<int> On_Tree()
    {
        Increment();
        
        if (await CommandsTest(CWorldTreeManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddTree(CWorldTreeManager.name))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(CWorldTreeManager.settings) == -1) return await Error("Problem in the tree settings found");
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
        
        if (await CommandsTest(CWorldMapManager.settings) == -1) return await Error("Problem in the map settings found");
        return 0;
    }
    
    public async Task<int> On_Modifier()
    {
        Increment();
        
        if (await CommandsTest(CWorldModifierManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddModifier(CWorldModifierManager.name))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(CWorldModifierManager.settings) == -1) return await Error("Problem in the modifier settings found");
        return 0;
    }
    
    public async Task<int> On_Link()
    {
        Increment();
        
        if (await CommandsTest(CWorldLinkManager.labels) == -1) return await Error("Couldn't assign label to link");
        if (!await ChunkGenerationNodes.AddLink(CWorldLinkManager.name))
        {
            if (!writerManager.import)
                return await Error("name is used twice");
            if (writerManager.import)
                return await SkipNode();
        }
        
        if (await CommandsTest(CWorldLinkManager.settings) == -1) return await Error("Problem in the link settings found");
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

            if (!BlockManager.SetUv(CWorldBlockManager.BlockNode.index, i, t))
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

        if (!BlockManager.SetPriority(CWorldBlockManager.BlockNode.index, t))
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
        ChunkGenerationNodes.sampleDisplayName = CWorldSampleManager.name;
        
        Console.Log("Drawing texture...");
        
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
    
    public Task<int> GetNextNInts(int n, out List<int> ints)
    {
        ints = new List<int>();

        //Check if there are enough arguments
        if (n <= 0)
            return Task.FromResult(0);
        
        int v;
        
        //Get the first int
        Increment();
        if (!int.TryParse(Arg, out v)) return Error("No valid int found");
        ints.Add(v);
        
        for (int i = 0; i < n-1; i++)
        {
            //loop over ", int" n-1 times
            Increment();
            if (!Arg.Equals(",")) return Error("',' is missing");
            Increment();
            if (!int.TryParse(Arg, out v)) return Error("No valid int found");
            ints.Add(v);
        }

        //Increment to the next command
        Increment();
        
        return Task.FromResult(0);
    }

    public void GetValue(out string value)
    {
        value = Arg;
    }
    
    public void GetNextValue(out string value)
    {
        Increment();
        value = Arg;
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
        { "Link", async (w) => await w.On_Link() },
        { "Use", async (w) => await w.On_Use() },
        { "Sample", async (w) => await w.On_Sample() },
        { "Biome", async (w) => await w.On_Biome() },
        { "Block", async (w) => await w.On_Block() },
        { "Modifier", async (w) => await w.On_Modifier() },
        { "Map", async (w) => await w.On_Map() },
        { "Tree", async (w) => await w.On_Tree() },
    };
}