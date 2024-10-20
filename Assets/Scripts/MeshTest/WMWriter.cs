using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WMWriter : MonoBehaviour
{
    public static WMWriter instance;

    public string msg = "";
    
    
    public TMP_Text inputField;
    public TMP_Text log;
    public TextureGeneration textureGeneration;
    public CWorldHandler handler;
    
    
    private string[] lines;
    private int index;
    private bool showChunkGen = false;
    private char[] charactersToReplace = new char[] { '(', ')', '=', '{', '}', ',', ':', '/'};

    private CNode currentNode;
    private CWorldSampleNode worldSampleNode;

    private string currentName = "";
    private string currentType = "";

    private string displayName = "";

    private void Start()
    {
        worldSampleNode = new CWorldSampleNode();
        worldSampleNode.writer = this;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCode()
    {
        handler.executes.Clear();
        handler.initializers.Clear();
        
        index = 0;
        currentName = "";
        currentType = "";
        displayName = "";
        
        string input = inputField.text;
        input = Regex.Replace(input, @"\u200B", "").Trim();

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (i < input.Length - 1 && input[i] == '/' && input[i + 1] == '/')
            {
                result.Append(" // ");
                i++;
            }
            else if (Array.Exists(charactersToReplace, element => element == input[i]))
            {
                result.Append($" {input[i]} ");
            }
            else
            {
                result.Append(input[i]);
            }
        }

        input = result.ToString().Trim();
        
        lines = input.Split(new[] { '\n','\t', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            Debug.Log(line.Trim());
        }
        Debug.Log("---------------------------------");

        while (index <= lines.Length)
        {
            if (index == lines.Length)
                break;
        
            Debug.Log("hello");
            int message = CommandTest(index, types);
            
            if (message == -1)
            {
                Debug.Log($"There is an error at string index {index}");
                break;
            }

            index++;
        }

        if (!displayName.Equals(""))
        {
            textureGeneration.UpdateTexture(displayName);
        }
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
        msg = message;
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
        worldSampleNode.SetSample(new CSampleNode());
        
        if (CommandsTest(worldSampleNode.labels) == -1) return Error("Problem in the label found");
        if (!handler.initializers.TryAdd(currentName, worldSampleNode.sampleNode))
            return Error("name is used twice");
        currentNode = handler.initializers[currentName];
        if (CommandsTest(worldSampleNode.settings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }
    
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
    
    public int On_SampleNoiseSize()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the size");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.sizeX = floats.x;
        sampleNode.noise.sizeY = floats.y;

        return 0;
    }

    public int On_Settings(Dictionary<string, Func<WMWriter, int>> commands)
    {
        index++;
        return CommandsTest(commands) == -1 ? Error("Problem with the settings") : 0;
    }

    public int On_SampleListAdd(List<CSampleNode> list)
    {
        while (true)
        {
            index++;
            if (handler.initializers.TryGetValue(lines[index], out var init))
            {
                if (init is CSampleNode sampleNode) list.Add(sampleNode);
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
    
    
    
    
    public int On_Biome()
    {
        index++;
        if (CommandsTest(biomeLabel) == -1) return Error("Problem in the label found");
        if (!handler.executes.TryAdd(currentName, new CBiomeNode()))
            return Error("name is used twice");
        currentNode = handler.executes[currentName];
        if (CommandsTest(biomeSettings) == -1) return Error("Problem in the sample settings found");
        return 0;
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
        
        if (handler.initializers.TryGetValue(lines[index], out CInit init))
        {
            if (currentNode is CBiomeNode biomeNode && init is CSampleNode sampleNode2)
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