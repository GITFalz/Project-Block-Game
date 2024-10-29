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

    public FileManager fileManager;
    public BlockManager blockManager;
    public CWorldMenu menu;
    
    
    private string[] lines;
    private int index;
    private bool showChunkGen = false;
    private char[] charactersToReplace = { '(', ')', '=', '{', '}', ',', ':', '/'};

    private string saveFile = "";

    private string[] _worldFiles;

    private CWAOperatorNode currentNode;
    private CWorldSampleManager _worldSampleManager;
    private CWorldBiomeManager _worldBiomeManager;

    private string currentName = "";
    private string currentBiomeName = "";
    private string currentType = "";

    private string displayName = "";

    private HashSet<string> fileNames;

    private void Start()
    {
        handler.Init();
        
        _worldSampleManager = new CWorldSampleManager();
        _worldSampleManager.writer = this;

        _worldBiomeManager = new CWorldBiomeManager();
        _worldBiomeManager.writer = this;

        fileNames = new HashSet<string>();
        
        fileManager.Init();
        _worldFiles = Directory.GetFiles(fileManager.worldPacksFolderPath, "*.cworld");

        menu.Init();
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
        
        lines = InitLines(content);

        Main();
    }

    public void ExecuteCode()
    {
        ExecuteCode(inputField.text);
    }
    
    

    public string[] InitLines(string content)
    {
        index = 0;
        currentName = "";
        currentBiomeName = "";
        currentType = "";
        displayName = "";
        saveFile = "";
        
        content = Regex.Replace(content, @"\u200B", "").Trim();

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < content.Length; i++)
        {
            if (i < content.Length - 1 && content[i] == '/' && content[i + 1] == '/') {
                result.Append(" // ");
                i++;
            }
            else if (Array.Exists(charactersToReplace, element => element == content[i]))
                result.Append($" {content[i]} ");
            else
                result.Append(content[i]);
        }

        content = result.ToString().Trim();
        
        return content.Split(new[] { '\n','\t', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public void Main()
    {
        textureGeneration.SetMove(true);
        
        while (index <= lines.Length)
        {
            if (index == lines.Length)
                break;

            int message = CommandTest(index, types);
            
            if (message == -1)
            {
                Debug.Log($"There is an error at string index {index}");
                break;
            }

            index++;
        }
        
        Debug.Log("hello");

        menu.Save(saveFile, inputField.text);

        if (!displayName.Equals(""))
        {
            Debug.Log(displayName);
            textureGeneration.UpdateTexture(displayName);
        }
    }
    


    public void SaveFile()
    {
        lines = InitLines(inputField.text);
            
        bool quitNext = false;
        foreach (string value in lines)
        {
            if (quitNext)
            {
                menu.Save(value, inputField.text);
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


    public int On_Save()
    {
        index++;
        saveFile = lines[index];
        return 1;
    }
    
    
    
    public int CommandTest(int index, Dictionary<string, Func<WMWriter, int>> commands)
    {
        string command = lines[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<WMWriter, int> func))
        {
            return func(this);
        }
        return -1;
    }

    public int IsComment(int index)
    {
        if (lines[index].Trim().StartsWith("//"))
        {
            index++;
            while (!lines[index].Trim().EndsWith("//"))
            {
                index++;
                if (index == lines.Length)
                {
                    return index;
                }
            }
        }
        return index;
    }

    public int Error(string message)
    {
        Debug.Log(message + " at string index : " + index);
        return -1;
    }

    private bool MaxIndex(int i)
    {
        return index + i < lines.Length;
    }
    

    public int CommandsTest(Dictionary<string, Func<WMWriter, int>> commands)
    {
        bool done = false;
        while (!done)
        {
            int message = CommandTest(index, commands);
            if(message == -1) return Error("Problem in the label found");
            if(message == 1) done = true;
        }
        return 0;
    }
    
    
    

    public int On_Sample()
    {
        index++;
        _worldSampleManager.SetSample(new CWorldSampleNode(currentName));
        
        if (CommandsTest(_worldSampleManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.sampleNodes.TryAdd(currentName, _worldSampleManager.sampleNode))
            return Error("name is used twice");

        currentNode = CWorldHandler.sampleNodes[currentName];
        if (CommandsTest(_worldSampleManager.settings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_Biome()
    {
        index++;
        _worldBiomeManager.SetBiome(new CWorldBiomeNode());
        
        if (CommandsTest(_worldBiomeManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.biomeNodes.TryAdd(currentBiomeName, _worldBiomeManager.biomeNode))
            return Error("name is used twice");

        currentNode = CWorldHandler.biomeNodes[currentBiomeName];
        if (CommandsTest(_worldBiomeManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }
    

    #region value getters

    public int GetNextFloat(out float value)
    {
        value = 0;

        try
        {
            index++;
            if (!float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float amplitude)) return Error("No valid amplitude value found");
            index++;

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
            index++;
            if (!int.TryParse(lines[index], out int result)) return Error("No valid amplitude value found");
            index++;

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
            index++;
            if (!float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) return Error("No valid min value found");
            index++;
            if (!lines[index].Equals(","))return Error("',' is missing");
            index++;
            if (!float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) return Error("No valid max value found");
            index++;

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
            index++;
            if (!int.TryParse(lines[index], out int x)) return Error("No valid int found");
            index++;
            if (!lines[index].Equals(","))return Error("',' is missing");
            index++;
            if (!int.TryParse(lines[index], out int y)) return Error("No valid int found");
            index++;

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

        index++;
        value = lines[index];
        index++;

        return 0;
    }
    
    public int GetNext(out string value)
    {
        value = "";

        index++;
        value = lines[index];

        return 0;
    }
    
    public int GetNext2Values(out string[] values)
    {
        values = new[] {"", ""};

        index++;
        values[0] = lines[index];
        index++;
        if (!lines[index].Equals(","))return Error("',' is missing");
        index++;
        values[1] = lines[index];
        index++;

        return 0;
    }

    #endregion

    public int On_SampleName()
    {
        Debug.Log("Name test");
        index++;
        
        if (!lines[index].Equals("="))
            return Error("No '=' found");
        index++;

        if (lines[index].Equals(")"))
            return Error("'name' expected but ) found");
        currentName = lines[index];
        index++;
        return 2;
    }
    
    public int On_BiomeName()
    {
        Debug.Log("Name test");
        index++;
        
        if (!lines[index].Equals("="))
            return Error("No '=' found");
        index++;

        if (lines[index].Equals(")"))
            return Error("'name' expected but ) found");
        currentBiomeName = lines[index];
        index++;
        return 2;
    }
    
    public int On_SampleNoiseSize()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the size");
        
        if (currentNode is not CWorldSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noiseNode.sizeX = floats.x;
        sampleNode.noiseNode.sizeY = floats.y;

        return 0;
    }

    public int On_Settings(Dictionary<string, Func<WMWriter, int>> commands)
    {
        index++;
        return CommandsTest(commands) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SampleListAdd(List<CWorldSampleNode> list)
    {
        while (true)
        {
            index++;
            if (CWorldHandler.sampleNodes.TryGetValue(lines[index], out var init))
            {
                list.Add(init);
            }

            index++;
            if (lines[index].Equals(",")) continue;
            return 0;
        }
    }
    
    
    public int On_AssignNext2Floats(ref float param1, ref float param2)
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1) 
            return Error("A problem was found while trying to get the next 2 floats, check if they are the correct type 0.0");
        param1 = floats.x; param2 = floats.y; return 0;
    }
    
    public int On_AssignNextFloat(ref float param1)
    {
        index++;
        if (GetNextFloat(out float value) == -1) 
            return Error("A problem was found while writing the threshold");
        param1 = value; return 0;
    }

    public int On_SetTrue(ref bool value)
    {
        index++; value = true; return 0;
    }
    
    public int On_BiomeOverride()
    {
        index++;
        if (CommandsTest(biomeOverrideOptions) == -1) return Error("Problem in the biome settings found");
        return 0;
    }

    public int On_BiomeOverrideSample()
    {
        index++;
        if (!lines[index].Equals(":"))
            return Error("':' expected");
        
        index++;
        
        if (CWorldHandler.sampleNodes.TryGetValue(lines[index], out CWorldSampleNode init))
        {
            if (currentNode is CWorldBiomeNode biomeNode)
                biomeNode.sample = init;
            else
                return Error("Something went wrong");
        }
        
        index++;

        return 0;
    }
    

    public int On_Name()
    {
        Debug.Log("Name test");
        index++;
        
        if (lines[index].Equals(")"))
            return Error("'name = sample_name' expected but ')' found");
        
        if (!lines[index].Equals("="))
            return Error("No '=' found");
        index++;

        if (lines[index].Equals(")"))
            return Error("'name' expected but ) found");
        currentName = lines[index];
        return 2;
    }

    public int Increment(int i, int result)
    {
        Debug.Log("Increment by : " + i);
        index += i;
        return result;
    }

    public int On_Display()
    {
        index++;
        displayName = currentName;

        return 0;
    }
    
    public Dictionary<string, Func<WMWriter, int>> types = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "Save" , (w) => w.On_Save() },
        { "Sample", (w) => w.On_Sample() },
        { "Biome", (w) => w.On_Biome() },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeLabel = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_SampleName() },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeSettings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "override", (w) => w.On_BiomeOverride() },
        { "display", (w) => w.On_Display() },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeOverrideOptions = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "sample", (w) => w.On_BiomeOverrideSample() },
        { "}", (w) => w.Increment(1, 1) }
    };
}