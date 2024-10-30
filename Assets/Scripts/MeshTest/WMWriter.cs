using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WMWriter : MonoBehaviour
{
    public static WMWriter instance;

    public GameObject buttonPrefab;
    public GameObject deleteButtonPrefab;
    public Transform contentPanel;

    public TMP_InputField inputField;
    //public TMP_Text inputField;
    public TMP_Text log;
    public TextureGeneration textureGeneration;
    public CWorldHandler handler;

    public BlockManager blockManager;

    public FileManager fileManager;
    [FormerlySerializedAs("blockManager")] public BlockManagerSO blockManagerSo;
    public CWorldMenu menu;

    public WriterManager writerManager;
    
    /**
    private string[] lines;
    private int index;
    private char[] charactersToReplace = { '(', ')', '=', '{', '}', ',', ':', '/'};

    private string saveFile = "";

    private CWAOperatorNode currentNode;
    private CWorldSampleManager _worldSampleManager;
    private CWorldBiomeManager _worldBiomeManager;

    private string currentName = "";
    private string currentBiomeName = "";
    private string currentType = "";

    private string displayName = "";

    private bool _import;
    */

    private void Start()
    {
        writerManager = new WriterManager(this, false);
        
        handler.Init();
        fileManager.Init();
        menu.Init();
        BlockManager.Init();

        LoadOnEnter();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    
    
    public void ExecuteCode(string content)
    {
        CWorldHandler.biomeNodes.Clear();
        CWorldHandler.sampleNodes.Clear();
        CWorldHandler.MapNode = null;
        
        writerManager.lines = writerManager.InitLines(content);

        Main();
    }

    public void ExecuteCode()
    {
        ExecuteCode(inputField.text);
    }

    public void Main()
    {
        textureGeneration.SetMove(true);

        DisplayLines(writerManager.lines);
        
        while (writerManager.index <= writerManager.lines.Length)
        {
            if (writerManager.index == writerManager.lines.Length)
                break;

            int message = CommandTest(writerManager.index, types);
            
            if (message == -1)
            {
                Debug.Log($"There is an error at string index {writerManager.index}");
                break;
            }

            writerManager.index++;
        }

        menu.Save(writerManager.savePath, writerManager.fileContent);

        if (!writerManager.displayName.Equals(""))
        {
            Debug.Log(writerManager.displayName);
            textureGeneration.UpdateTexture(writerManager.displayName);
        }
        
        blockManager.UpdateInspector();
    }


    public void LoadOnEnter()
    {
        if (!fileManager.executeOnEnterPath.Equals(""))
        {
            Debug.Log("Loading files...");
            string[] files = GetCWorldFilesInFolder();
            Debug.Log(files.Length + " files found");
            
            foreach (var filePath in files)
            {
                Load(filePath);
            }
        }
    }

    public int Load(string path)
    {
        Debug.Log(path);

        WriterManager oldWriter = writerManager;
        writerManager = new WriterManager(this, true);

        string content;
        
        try { content = File.ReadAllText(path); }
        catch (FileNotFoundException) { return Error("File not found"); }

        writerManager.savePath = path;

        try { writerManager.InitLines(content); }
        catch (Exception e) { return Error(e + ": A problem occured when initializing lines"); }

        try { Main(); }
        catch (Exception e) { return Error(e + ": A problem occured when running the main loop"); }
        
        Debug.Log(oldWriter.index + " " + writerManager.index);
        
        writerManager = oldWriter;

        return 1;
    }

    public string[] GetCWorldFilesInFolder()
    {
        List<string> currentPaths = new List<string>();
        List<string> checkedPaths = new List<string>();
        List<string> toBeChecked = new List<string>();
        List<string> files = new List<string>();
        
        string[] filePaths = Directory.GetFiles(fileManager.executeOnEnterPath, "*.cworld");
        files.AddRange(filePaths);
        
        currentPaths = FileManager.GetFolderPaths(fileManager.executeOnEnterPath);

        {
            Debug.Log(currentPaths.ToString());
        }

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
    


    public void SaveFile()
    {
        writerManager.lines = writerManager.InitLines(inputField.text);
            
        bool quitNext = false;
        foreach (string value in writerManager.lines)
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


    public int On_Save()
    {
        writerManager.index++;
        writerManager.saveFile = writerManager.lines[writerManager.index];
        return 1;
    }

    public int On_Use()
    {
        writerManager.index++;
        string[] path = writerManager.lines[writerManager.index].Split(new[] { '/', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        string mainPath = fileManager.worldPacksFolderPath;
        foreach (string p in path)
        {
            mainPath = Path.Combine(mainPath, p);
        }

        return Load(mainPath + ".cworld");
    }
    
    
    
    public int CommandTest(int index, Dictionary<string, Func<WMWriter, int>> commands)
    {
        string command = writerManager.lines[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<WMWriter, int> func))
        {
            return func(this);
        }
        return -1;
    }

    public int IsComment(int index)
    {
        if (writerManager.lines[index].Trim().StartsWith("//"))
        {
            index++;
            while (!writerManager.lines[index].Trim().EndsWith("//"))
            {
                index++;
                if (index == writerManager.lines.Length)
                {
                    return index;
                }
            }
        }
        return index;
    }

    public int Error(string message)
    {
        PopupError.Popup(message);
        Debug.Log(message + " at string index : " + writerManager.index);
        return -1;
    }

    private bool MaxIndex(int i)
    {
        return writerManager.index + i < writerManager.lines.Length;
    }

    public int DisplayLines(string[] content)
    {
        string concat = "";
        foreach (var s in content)
        {
            concat += " " + s;
        }

        Debug.Log(concat);
        return 0;
    }
    

    public int CommandsTest(Dictionary<string, Func<WMWriter, int>> commands)
    {
        bool done = false;
        while (!done)
        {
            int message = CommandTest(writerManager.index, commands);
            if(message == -1) return Error("Problem in the label found");
            if(message == 1) done = true;
        }
        return 0;
    }
    
    
    

    public int On_Sample()
    {
        writerManager.index++;
        writerManager.worldSampleManager.SetSample(new CWorldSampleNode(writerManager.currentName));
        
        if (CommandsTest(writerManager.worldSampleManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.sampleNodes.TryAdd(writerManager.currentName, writerManager.worldSampleManager.sampleNode))
        {
            if (!writerManager.import)
                return Error("name is used twice");
            if (writerManager.import)
                return SkipNode();
        }
        
        writerManager.currentNode = CWorldHandler.sampleNodes[writerManager.currentName];
        if (CommandsTest(writerManager.worldSampleManager.settings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_Biome()
    {
        writerManager.index++;
        writerManager.worldBiomeManager.SetBiome(new CWorldBiomeNode());
        
        if (CommandsTest(writerManager.worldBiomeManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.biomeNodes.TryAdd(writerManager.currentBiomeName, writerManager.worldBiomeManager.biomeNode))
        {
            if (!writerManager.import)
                return Error("name is used twice");
            if (writerManager.import)
                return SkipNode();
        }

        writerManager.currentNode = CWorldHandler.biomeNodes[writerManager.currentBiomeName];
        if (CommandsTest(writerManager.worldBiomeManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }
    
    public int On_Block()
    {
        writerManager.Inc();
        
        writerManager.worldBlockManager.SetBlock(new CWorldBlock());
        
        if (CommandsTest(writerManager.worldBlockManager.labels) == -1) return Error("Problem in the label found");

        writerManager.worldBlockManager.BlockNode.blockName = writerManager.currentBlockName;
        
        if (CommandsTest(writerManager.worldBlockManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }
    
    public int On_Map()
    {
        writerManager.Inc();
        
        writerManager.worldMapManager.SetMap(new CWorldMapNode());

        if (writerManager.NextLine(1).Equals("Force") || CWorldHandler.MapNode == null)
        {
            CWorldHandler.MapNode = writerManager.worldMapManager.MapNode;
        }
        else
        {
            return Error("Watch out!, a map node already exists in the system. To replace the node write 'Map Force'");
        }
        
        writerManager.Inc();
        
        if (CommandsTest(writerManager.worldMapManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }

    public int SkipNode()
    {
        try
        {
            while (true)
            {
                if (writerManager.lines[writerManager.index].Equals("}") && writerManager.index + 1 == writerManager.lines.Length ||
                    writerManager.lines[writerManager.index].Equals("}") && types.ContainsKey(writerManager.lines[writerManager.index + 1]))
                    return 0;

                writerManager.index++;
            }
        }
        catch (Exception e)
        {
            return Error(e.Message);
        }
    }
    

    #region value getters

    public int GetNextFloat(out float value)
    {
        value = 0;

        try
        {
            writerManager.index++;
            if (!float.TryParse(writerManager.lines[writerManager.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float amplitude)) return Error("No valid float value found");
            writerManager.index++;

            value = amplitude;
            return 0;
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
    
    public int GetNextInt(out int value)
    {
        value = 0;

        try
        {
            writerManager.index++;
            if (!int.TryParse(writerManager.lines[writerManager.index], out int result)) return Error("No valid int value found");
            writerManager.index++;

            value = result;
            return 0;
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
    
    public int GetNext2Floats(out Vector2 floats)
    {
        floats = Vector2.zero;

        try
        {
            writerManager.index++;
            if (!float.TryParse(writerManager.lines[writerManager.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) return Error("No valid min value found");
            writerManager.index++;
            if (!writerManager.lines[writerManager.index].Equals(","))return Error("',' is missing");
            writerManager.index++;
            if (!float.TryParse(writerManager.lines[writerManager.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) return Error("No valid max value found");
            writerManager.index++;

            floats.x = x;
            floats.y = y;
            return 0;
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
    
    public int GetNext2Ints(out Vector2Int ints)
    {
        ints = Vector2Int.zero;

        try
        {
            writerManager.index++;
            if (!int.TryParse(writerManager.lines[writerManager.index], out int x)) return Error("No valid int found");
            writerManager.index++;
            if (!writerManager.lines[writerManager.index].Equals(","))return Error("',' is missing");
            writerManager.index++;
            if (!int.TryParse(writerManager.lines[writerManager.index], out int y)) return Error("No valid int found");
            writerManager.index++;

            ints.x = x;
            ints.y = y;
            return 0;
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

    public int GetNextValue(out string value)
    {
        value = "";

        writerManager.index++;
        value = writerManager.lines[writerManager.index];
        writerManager.index++;

        return 0;
    }
    
    public int GetNext(out string value)
    {
        value = "";

        writerManager.index++;
        value = writerManager.lines[writerManager.index];

        return 0;
    }
    
    public int GetNext2Values(out string[] values)
    {
        values = new[] {"", ""};

        writerManager.index++;
        values[0] = writerManager.lines[writerManager.index];
        writerManager.index++;
        if (!writerManager.lines[writerManager.index].Equals(","))return Error("',' is missing");
        writerManager.index++;
        values[1] = writerManager.lines[writerManager.index];
        writerManager.index++;

        return 0;
    }

    #endregion

    public int On_SampleName()
    {
        Debug.Log("Name test");
        writerManager.index++;
        
        if (!writerManager.lines[writerManager.index].Equals("="))
            return Error("No '=' found");
        writerManager.index++;

        if (writerManager.lines[writerManager.index].Equals(")"))
            return Error("'name' expected but ) found");
        writerManager.currentName = writerManager.lines[writerManager.index];
        writerManager.index++;
        return 2;
    }
    
    public int On_BiomeName()
    {
        Debug.Log("Name test");
        writerManager.index++;
        
        if (!writerManager.lines[writerManager.index].Equals("="))
            return Error("No '=' found");
        writerManager.index++;

        if (writerManager.lines[writerManager.index].Equals(")"))
            return Error("'name' expected but ) found");
        writerManager.currentBiomeName = writerManager.lines[writerManager.index];
        writerManager.index++;
        return 2;
    }
    public int On_BlockName()
    {
        Debug.Log("Name test");
        writerManager.index++;
        
        if (!writerManager.lines[writerManager.index].Equals("="))
            return Error("No '=' found");
        writerManager.index++;

        if (writerManager.lines[writerManager.index].Equals(")"))
            return Error("'name' expected but ) found");
        writerManager.currentBlockName = writerManager.lines[writerManager.index];
        writerManager.index++;
        return 2;
    }

    public int On_BlockSetTextures()
    {
        int i = 0;
        while (true)
        {
            if (GetNextInt(out int t) == -1)
                return Error("A problem occured when trying to get the texture indexes, check if they are indeed integers");

            if (!BlockManager.SetUv(writerManager.worldBlockManager.BlockNode.index, i, t))
                return Error("A problem occured when trying set the uv texture index, too many indices could be the problem");
            
            i++;
            if (writerManager.lines[writerManager.index].Equals(",")) continue;
            return 0;
        }
    }

    public int On_SetBiomeRange()
    {
        writerManager.Inc();
        if (!CWorldHandler.biomeNodes.TryGetValue(writerManager.CurrentLine(), out var biome))
            return Error("Can't find biome -_-");
        
        CWorldHandler.MapNode.biomePool.Add(new BiomePool(biome));
        writerManager.Inc();
        return CommandsTest(writerManager.worldMapManager.setRanges) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SetSampleRange()
    {
        writerManager.Inc();

        if (!CWorldHandler.sampleNodes.TryGetValue(writerManager.CurrentLine(), out var sample))
            return Error("Sample may not exists when setting biome range");
        
        if (CWorldHandler.MapNode == null)
            return Error("How?? Map == null??");

        CWorldHandler.MapNode.biomePool[^1].samples.Add(sample.name, new BiomePoolSample(sample));

        if (GetNext2Floats(out var floats) == -1)
            return Error("Not valid range u_u");

        CWorldHandler.MapNode.biomePool[^1].samples[sample.name].min = floats.x;
        CWorldHandler.MapNode.biomePool[^1].samples[sample.name].max = floats.y;

        return 0;
    }
    
    public int On_SampleNoiseSize()
    {
        writerManager.index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the size");
        
        if (writerManager.currentNode is not CWorldSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noiseNode.sizeX = floats.x;
        sampleNode.noiseNode.sizeY = floats.y;

        return 0;
    }

    public int On_Settings(Dictionary<string, Func<WMWriter, int>> commands)
    {
        writerManager.index++;
        return CommandsTest(commands) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SampleListAdd(List<CWorldSampleNode> list)
    {
        while (true)
        {
            writerManager.index++;
            if (CWorldHandler.sampleNodes.TryGetValue(writerManager.lines[writerManager.index], out var init))
            {
                list.Add(init);
            }

            writerManager.index++;
            if (writerManager.lines[writerManager.index].Equals(",")) continue;
            return 0;
        }
    }

    public int On_SetSample()
    {
        writerManager.Inc();
        if (!CWorldHandler.sampleNodes.TryGetValue(writerManager.CurrentLine(), out var init))
            return Error("Can't find the sample specified in the biome");

        writerManager.worldBiomeManager.biomeNode.sample = init;
        writerManager.Inc();
        return 0;
    }
    
    
    public int On_AssignNext2Floats(ref float param1, ref float param2)
    {
        writerManager.index++;
        if (GetNext2Floats(out Vector2 floats) == -1) 
            return Error("A problem was found while trying to get the next 2 floats, check if they are the correct type 0.0");
        param1 = floats.x; param2 = floats.y; return 0;
    }
    
    public int On_AssignNextFloat(ref float param1)
    {
        writerManager.index++;
        if (GetNextFloat(out float value) == -1) 
            return Error("A problem was found while writing the threshold");
        param1 = value; return 0;
    }

    public int On_SetTrue(ref bool value)
    {
        writerManager.index++; value = true; return 0;
    }
    

    public int On_Name()
    {
        Debug.Log("Name test");
        writerManager.index++;
        
        if (writerManager.lines[writerManager.index].Equals(")"))
            return Error("'name = sample_name' expected but ')' found");
        
        if (!writerManager.lines[writerManager.index].Equals("="))
            return Error("No '=' found");
        writerManager.index++;

        if (writerManager.lines[writerManager.index].Equals(")"))
            return Error("'name' expected but ) found");
        writerManager.currentName = writerManager.lines[writerManager.index];
        return 2;
    }

    public int Increment(int i, int result)
    {
        Debug.Log("Increment by : " + i);
        writerManager.index += i;
        return result;
    }

    public int On_Display()
    {
        writerManager.index++;
        writerManager.displayName = writerManager.currentName;

        return 0;
    }
    
    public Dictionary<string, Func<WMWriter, int>> types = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "Save" , (w) => w.On_Save() },
        { "Use" , (w) => w.On_Use() },
        { "Sample", (w) => w.On_Sample() },
        { "Biome", (w) => w.On_Biome() },
        { "Block", (w) => w.On_Block() },
        { "Map", (w) => w.On_Map() },
    };
}