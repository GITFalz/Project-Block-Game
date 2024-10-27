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
    
    
    private string[] lines;
    private int index;
    private bool showChunkGen = false;
    private char[] charactersToReplace = { '(', ')', '=', '{', '}', ',', ':', '/'};

    private string saveFile = "";

    private string[] _worldFiles;

    private CWAOperatorNode currentNode;
    private CWorldSampleNode worldSampleNode;
    private CWorldBiomeNode _worldBiomeNode;

    private string currentName = "";
    private string currentBiomeName = "";
    private string currentType = "";

    private string displayName = "";

    private HashSet<string> fileNames;

    private void Start()
    {
        handler.Init();
        
        worldSampleNode = new CWorldSampleNode();
        worldSampleNode.writer = this;

        _worldBiomeNode = new CWorldBiomeNode();
        _worldBiomeNode.writer = this;

        fileNames = new HashSet<string>();
        
        _worldFiles = Directory.GetFiles(fileManager.worldPacksFolderPath, "*.cworld");
        
        GenerateButtons();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    
    
    public void ExecuteCode(string content)
    {
        lines = InitLines(content);

        Main();
    }

    public void ExecuteCode()
    {
        handler.executes.Clear();
        handler.initializers.Clear();

        lines = InitLines(inputField.text);

        foreach (string line in lines)
        {
            Debug.Log(line.Trim());
        }
        Debug.Log("---------------------------------");

        Main();
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

        Save();

        if (!displayName.Equals(""))
        {
            Debug.Log(displayName);
            textureGeneration.UpdateTexture(displayName);
        }
    }


    public void Save()
    {
        if (!saveFile.Equals(""))
        {
            string filePath = Path.Combine(fileManager.worldPacksFolderPath, saveFile + ".cworld");
            File.WriteAllText(filePath, inputField.text);
        }
        
        GenerateButtons();
    }


    public void SaveFile()
    {
        saveFile = "";
        lines = InitLines(inputField.text);
            
        bool quitNext = false;
        foreach (string value in lines)
        {
            if (quitNext)
            {
                saveFile = value;
                break;
            }
            
            if (value.Equals("Save"))
            {
                quitNext = true;
            }
        }

        Save();
    }


    public void GenerateButtons()
    {
        _worldFiles = Directory.GetFiles(fileManager.worldPacksFolderPath, "*.cworld");
        
        foreach (string filePath in _worldFiles)
        {
            GenerateButton(filePath);
        }
    }

    public void GenerateButton(string filePath)
    {
        string buttonName = Path.GetFileNameWithoutExtension(filePath);

        if (!fileNames.Contains(buttonName))
        {
            GameObject buttonContainer = new GameObject("Container");
            buttonContainer.transform.SetParent(contentPanel, false);
            var layoutGroup = buttonContainer.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.spacing = 2;
            
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer.transform);
            GameObject newDeleteButton = Instantiate(deleteButtonPrefab, buttonContainer.transform);
            
            newButton.GetComponentInChildren<TMP_Text>().text = buttonName;

            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => DisplayContent(filePath));
            
            Button deletebutton = newDeleteButton.GetComponent<Button>();
            deletebutton.onClick.AddListener(() => DeleteFile(filePath, buttonContainer));

            fileNames.Add(buttonName);
        }
    }


    public string DisplayContent(string filePath)
    {
        string content = File.ReadAllText(filePath);
        Debug.Log(content);
        inputField.text = content;
        
        return "";
    }

    public void DeleteFile(string filePath, GameObject container)
    {
        File.Delete(filePath);
        Destroy(container);
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
        worldSampleNode.SetSample(new CWOISampleNode());
        
        if (CommandsTest(worldSampleNode.labels) == -1) return Error("Problem in the label found");
        if (!handler.initializers.TryAdd(currentName, worldSampleNode.sampleNode))
            return Error("name is used twice");
        currentNode = handler.initializers[currentName];
        if (CommandsTest(worldSampleNode.settings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_Biome()
    {
        index++;
        _worldBiomeNode.SetBiome(new CWOEBiomeNode());
        
        if (CommandsTest(_worldBiomeNode.labels) == -1) return Error("Problem in the label found");
        if (!handler.executes.TryAdd(currentBiomeName, _worldBiomeNode.biomeNode))
            return Error("name is used twice");
        currentNode = handler.executes[currentBiomeName];
        if (CommandsTest(_worldBiomeNode.settings) == -1) return Error("Problem in the biome settings found");
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
        
        if (currentNode is not CWOISampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noiseNode.sizeX = floats.x;
        sampleNode.noiseNode.sizeY = floats.y;

        return 0;
    }

    public int On_Settings(Dictionary<string, Func<WMWriter, int>> commands)
    {
        index++;
        return CommandsTest(commands) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SampleListAdd(List<CWOISampleNode> list)
    {
        while (true)
        {
            index++;
            if (handler.initializers.TryGetValue(lines[index], out var init))
            {
                if (init is CWOISampleNode sampleNode) list.Add(sampleNode);
                else return Error("Sample node not found (handler > get init)");
            }

            index++;
            if (lines[index].Equals(",")) continue;
            return 0;
        }
    }
    
    
    public int On_AssingNext2Floats(ref float param1, ref float param2)
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1) 
            return Error("A problem was found while writing the threshold");
        param1 = floats.x; param2 = floats.y; return 0;
    }
    
    public int On_AssingNext2Floats(out Vector2 floats)
    {
        index++;
        if (GetNext2Floats(out floats) == -1) 
            return Error("A problem was found while writing the threshold");
        return 0;
    }
    
    public int On_AssingNextFloat(ref float param1)
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
        
        if (handler.initializers.TryGetValue(lines[index], out CWAInitializerNode init))
        {
            if (currentNode is CWOEBiomeNode biomeNode && init is CWOISampleNode sampleNode2)
                biomeNode.sample = sampleNode2;
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