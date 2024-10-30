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

    private WriterManager _wm;
    
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
        _wm = new WriterManager(this, false);
        
        handler.Init();
        fileManager.Init();
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
        
        _wm.lines = _wm.InitLines(content);

        Main();
    }

    public void ExecuteCode()
    {
        ExecuteCode(inputField.text);
    }

    public void Main()
    {
        textureGeneration.SetMove(true);

        DisplayLines(_wm.lines);
        
        while (_wm.index <= _wm.lines.Length)
        {
            if (_wm.index == _wm.lines.Length)
                break;

            int message = CommandTest(_wm.index, types);
            
            if (message == -1)
            {
                Debug.Log($"There is an error at string index {_wm.index}");
                break;
            }

            _wm.index++;
        }

        menu.Save(_wm.savePath, _wm.fileContent);

        if (!_wm.displayName.Equals(""))
        {
            Debug.Log(_wm.displayName);
            textureGeneration.UpdateTexture(_wm.displayName);
        }
        
        blockManager.UpdateInspector();
    }
    


    public void SaveFile()
    {
        _wm.lines = _wm.InitLines(inputField.text);
            
        bool quitNext = false;
        foreach (string value in _wm.lines)
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

    public void Clear()
    {
        inputField.text = "";
    }


    public int On_Save()
    {
        _wm.index++;
        _wm.saveFile = _wm.lines[_wm.index];
        return 1;
    }

    public int On_Use()
    {
        _wm.index++;
        string[] path = _wm.lines[_wm.index].Split(new[] { '/', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        string mainPath = fileManager.worldPacksFolderPath;
        foreach (string p in path)
        {
            mainPath = Path.Combine(mainPath, p);
        }
        
        Debug.Log(mainPath + ".cworld");

        WriterManager oldWriter = _wm;
        _wm = new WriterManager(this, true);

        string content;
        
        try { content = File.ReadAllText(mainPath + ".cworld"); }
        catch (FileNotFoundException) { return Error("File not found"); }

        _wm.savePath = mainPath;

        try { _wm.InitLines(content); }
        catch (Exception e) { return Error(e + ": A problem occured when initializing lines"); }

        try { Main(); }
        catch (Exception e) { return Error(e + ": A problem occured when running the main loop"); }
        
        Debug.Log(oldWriter.index + " " + _wm.index);
        
        _wm = oldWriter;

        return 1;
    }
    
    
    
    public int CommandTest(int index, Dictionary<string, Func<WMWriter, int>> commands)
    {
        string command = _wm.lines[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<WMWriter, int> func))
        {
            return func(this);
        }
        return -1;
    }

    public int IsComment(int index)
    {
        if (_wm.lines[index].Trim().StartsWith("//"))
        {
            index++;
            while (!_wm.lines[index].Trim().EndsWith("//"))
            {
                index++;
                if (index == _wm.lines.Length)
                {
                    return index;
                }
            }
        }
        return index;
    }

    public int Error(string message)
    {
        Debug.Log(message + " at string index : " + _wm.index);
        return -1;
    }

    private bool MaxIndex(int i)
    {
        return _wm.index + i < _wm.lines.Length;
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
            int message = CommandTest(_wm.index, commands);
            if(message == -1) return Error("Problem in the label found");
            if(message == 1) done = true;
        }
        return 0;
    }
    
    
    

    public int On_Sample()
    {
        _wm.index++;
        _wm.worldSampleManager.SetSample(new CWorldSampleNode(_wm.currentName));
        
        if (CommandsTest(_wm.worldSampleManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.sampleNodes.TryAdd(_wm.currentName, _wm.worldSampleManager.sampleNode))
        {
            if (!_wm.import)
                return Error("name is used twice");
            if (_wm.import)
                return SkipNode();
        }
        
        _wm.currentNode = CWorldHandler.sampleNodes[_wm.currentName];
        if (CommandsTest(_wm.worldSampleManager.settings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_Biome()
    {
        _wm.index++;
        _wm.worldBiomeManager.SetBiome(new CWorldBiomeNode());
        
        if (CommandsTest(_wm.worldBiomeManager.labels) == -1) return Error("Problem in the label found");
        if (!CWorldHandler.biomeNodes.TryAdd(_wm.currentBiomeName, _wm.worldBiomeManager.biomeNode))
        {
            if (!_wm.import)
                return Error("name is used twice");
            if (_wm.import)
                return SkipNode();
        }

        _wm.currentNode = CWorldHandler.biomeNodes[_wm.currentBiomeName];
        if (CommandsTest(_wm.worldBiomeManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }
    
    public int On_Block()
    {
        _wm.index++;
        
        _wm.worldBlockManager.SetBlock(new CWorldBlock());
        
        if (CommandsTest(_wm.worldBlockManager.labels) == -1) return Error("Problem in the label found");

        _wm.worldBlockManager.BlockNode.blockName = _wm.currentBlockName;
        
        if (CommandsTest(_wm.worldBlockManager.settings) == -1) return Error("Problem in the biome settings found");
        return 0;
    }

    public int SkipNode()
    {
        try
        {
            while (true)
            {
                if (_wm.lines[_wm.index].Equals("}") && _wm.index + 1 == _wm.lines.Length ||
                    _wm.lines[_wm.index].Equals("}") && types.ContainsKey(_wm.lines[_wm.index + 1]))
                    return 0;

                _wm.index++;
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
            _wm.index++;
            if (!float.TryParse(_wm.lines[_wm.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float amplitude)) return Error("No valid float value found");
            _wm.index++;

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
            _wm.index++;
            if (!int.TryParse(_wm.lines[_wm.index], out int result)) return Error("No valid int value found");
            _wm.index++;

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
            _wm.index++;
            if (!float.TryParse(_wm.lines[_wm.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float x)) return Error("No valid min value found");
            _wm.index++;
            if (!_wm.lines[_wm.index].Equals(","))return Error("',' is missing");
            _wm.index++;
            if (!float.TryParse(_wm.lines[_wm.index], NumberStyles.Float, CultureInfo.InvariantCulture, out float y)) return Error("No valid max value found");
            _wm.index++;

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
            _wm.index++;
            if (!int.TryParse(_wm.lines[_wm.index], out int x)) return Error("No valid int found");
            _wm.index++;
            if (!_wm.lines[_wm.index].Equals(","))return Error("',' is missing");
            _wm.index++;
            if (!int.TryParse(_wm.lines[_wm.index], out int y)) return Error("No valid int found");
            _wm.index++;

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

        _wm.index++;
        value = _wm.lines[_wm.index];
        _wm.index++;

        return 0;
    }
    
    public int GetNext(out string value)
    {
        value = "";

        _wm.index++;
        value = _wm.lines[_wm.index];

        return 0;
    }
    
    public int GetNext2Values(out string[] values)
    {
        values = new[] {"", ""};

        _wm.index++;
        values[0] = _wm.lines[_wm.index];
        _wm.index++;
        if (!_wm.lines[_wm.index].Equals(","))return Error("',' is missing");
        _wm.index++;
        values[1] = _wm.lines[_wm.index];
        _wm.index++;

        return 0;
    }

    #endregion

    public int On_SampleName()
    {
        Debug.Log("Name test");
        _wm.index++;
        
        if (!_wm.lines[_wm.index].Equals("="))
            return Error("No '=' found");
        _wm.index++;

        if (_wm.lines[_wm.index].Equals(")"))
            return Error("'name' expected but ) found");
        _wm.currentName = _wm.lines[_wm.index];
        _wm.index++;
        return 2;
    }
    
    public int On_BiomeName()
    {
        Debug.Log("Name test");
        _wm.index++;
        
        if (!_wm.lines[_wm.index].Equals("="))
            return Error("No '=' found");
        _wm.index++;

        if (_wm.lines[_wm.index].Equals(")"))
            return Error("'name' expected but ) found");
        _wm.currentBiomeName = _wm.lines[_wm.index];
        _wm.index++;
        return 2;
    }
    public int On_BlockName()
    {
        Debug.Log("Name test");
        _wm.index++;
        
        if (!_wm.lines[_wm.index].Equals("="))
            return Error("No '=' found");
        _wm.index++;

        if (_wm.lines[_wm.index].Equals(")"))
            return Error("'name' expected but ) found");
        _wm.currentBlockName = _wm.lines[_wm.index];
        _wm.index++;
        return 2;
    }

    public int On_BlockSetTextures()
    {
        int i = 0;
        while (true)
        {
            if (GetNextInt(out int t) == -1)
                return Error("A problem occured when trying to get the texture indexes, check if they are indeed integers");

            if (!BlockManager.SetUv(_wm.worldBlockManager.BlockNode.index, i, t))
                return Error("A problem occured when trying set the uv texture index, too many indices could be the problem");
            
            i++;
            if (_wm.lines[_wm.index].Equals(",")) continue;
            return 0;
        }
    }
    
    public int On_SampleNoiseSize()
    {
        _wm.index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the size");
        
        if (_wm.currentNode is not CWorldSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noiseNode.sizeX = floats.x;
        sampleNode.noiseNode.sizeY = floats.y;

        return 0;
    }

    public int On_Settings(Dictionary<string, Func<WMWriter, int>> commands)
    {
        _wm.index++;
        return CommandsTest(commands) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SampleListAdd(List<CWorldSampleNode> list)
    {
        while (true)
        {
            _wm.index++;
            if (CWorldHandler.sampleNodes.TryGetValue(_wm.lines[_wm.index], out var init))
            {
                list.Add(init);
            }

            _wm.index++;
            if (_wm.lines[_wm.index].Equals(",")) continue;
            return 0;
        }
    }
    
    
    public int On_AssignNext2Floats(ref float param1, ref float param2)
    {
        _wm.index++;
        if (GetNext2Floats(out Vector2 floats) == -1) 
            return Error("A problem was found while trying to get the next 2 floats, check if they are the correct type 0.0");
        param1 = floats.x; param2 = floats.y; return 0;
    }
    
    public int On_AssignNextFloat(ref float param1)
    {
        _wm.index++;
        if (GetNextFloat(out float value) == -1) 
            return Error("A problem was found while writing the threshold");
        param1 = value; return 0;
    }

    public int On_SetTrue(ref bool value)
    {
        _wm.index++; value = true; return 0;
    }
    

    public int On_Name()
    {
        Debug.Log("Name test");
        _wm.index++;
        
        if (_wm.lines[_wm.index].Equals(")"))
            return Error("'name = sample_name' expected but ')' found");
        
        if (!_wm.lines[_wm.index].Equals("="))
            return Error("No '=' found");
        _wm.index++;

        if (_wm.lines[_wm.index].Equals(")"))
            return Error("'name' expected but ) found");
        _wm.currentName = _wm.lines[_wm.index];
        return 2;
    }

    public int Increment(int i, int result)
    {
        Debug.Log("Increment by : " + i);
        _wm.index += i;
        return result;
    }

    public int On_Display()
    {
        _wm.index++;
        _wm.displayName = _wm.currentName;

        return 0;
    }
    
    public Dictionary<string, Func<WMWriter, int>> types = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "Save" , (w) => w.On_Save() },
        { "Use" , (w) => w.On_Use() },
        { "Sample", (w) => w.On_Sample() },
        { "Biome", (w) => w.On_Biome() },
        { "Block", (w) => w.On_Block() },
    };
}