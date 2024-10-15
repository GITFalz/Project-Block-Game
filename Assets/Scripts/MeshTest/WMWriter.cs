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

    private Dictionary<string, CSampleNode> samples;

    private string currentName = "";
    private string currentType = "";

    private void Start()
    {
        samples = new Dictionary<string, CSampleNode>();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCode()
    {
        samples.Clear();
        index = 0;
        currentName = "";
        currentType = "";
        
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
    }
    
    public int CommandTest(int index, Dictionary<string, Func<int>> commands)
    {
        string command = lines[index];
        Debug.Log("Command : " + command);
        if (commands.TryGetValue(command, out Func<int> func))
        {
            return func();
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

    private int Error(string message)
    {
        Debug.Log(message + " at string index : " + index);
        msg = message;
        return -1;
    }

    private bool MaxIndex(int i)
    {
        return index + i < lines.Length;
    }
    

    public int CommandsTest(Dictionary<string, Func<int>> commands)
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
        if (CommandsTest(sampleLabel) == -1) return Error("Problem in the label found");
        if (!samples.TryAdd(currentName, new CSampleNode()))
            return Error("name is used twice");
        currentNode = samples[currentName];
        if (CommandsTest(sampleSettings) == -1) return Error("Problem in the sample settings found");
        return 0;
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

    public int On_SampleNoise()
    {
        index++;
        samples[currentName].noise = new CNoiseNode();
        if (CommandsTest(sampleNoiseOptions) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    #region noise settings
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

    public int On_SampleNoiseThreshold()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the threshold");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.t_min = floats.x;
        sampleNode.noise.t_max = floats.y;

        return 0;
    }

    public int On_SampleNoiseClamp()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the clamp");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.c_min = floats.x;
        sampleNode.noise.c_max = floats.y;

        return 0;
    }

    public int On_SampleNoiseSlide()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.noise.t_slide = true;
        return 0;
    }

    public int On_SampleNoiseSmooth()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.noise.t_smooth = true;
        return 0;
    }

    public int On_SampleNoiseAmplitude()
    {
        index++;
        if (GetNextFloat(out float value) == -1)
            return Error("A problem was found while writing the amplitude");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.amplitude = value;

        return 0;
    }

    public int On_SampleNoiseInvert()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.noise.invert = true;
        return 0;
    }
    #endregion
    
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

    public int On_SampleSettings()
    {
        return Error("not implemented");
    }

    public int On_SampleOverride()
    {
        index++;
        samples[currentName].overRide = new COverrideNode();
        if (CommandsTest(sampleOverrideOptions) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_SampleOverrideSample()
    {
        index++;
        return 0;
    }

    public int On_SampleOverrideAdd()
    {
        index++;
        if (!lines[index].Equals(":"))
            return Error("':' expected");
        
        index++;
        if (!samples.ContainsKey(lines[index]))
            return Error("There is no such sample");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.add.Add(samples[lines[index]]);
        
        index++;

        return 0;
    }
    
    public int On_SampleOverrideThreshold()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the threshold");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.overRide.t_min = floats.x;
        sampleNode.overRide.t_max = floats.y;

        return 0;
    }

    public int On_SampleOverrideClamp()
    {
        index++;
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the clamp");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.overRide.c_min = floats.x;
        sampleNode.overRide.c_max = floats.y;

        return 0;
    }

    public int On_SampleOverrideSlide()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.overRide.t_slide = true;
        return 0;
    }

    public int On_SampleOverrideSmooth()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.overRide.t_smooth = true;
        return 0;
    }

    public int On_SampleOverrideInvert()
    {
        index++;
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.overRide.invert = true;
        return 0;
    }
    
    public int On_Biome() { return 0; }

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
        textureGeneration.UpdateTexture(samples[currentName]);

        return 0;
    }
    
    public Dictionary<string, Func<int>> types = new Dictionary<string, Func<int>>()
    {
        { "Sample", () => instance.On_Sample() },
        { "Biome", () => instance.On_Biome() },
    };
    
    public Dictionary<string, Func<int>> sampleLabel = new Dictionary<string, Func<int>>()
    {
        { "(", () => instance.Increment(1, 0) },
        { "name", () => instance.On_SampleName() },
        { ")", () => instance.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<int>> sampleSettings = new Dictionary<string, Func<int>>()
    {
        { "{", () => instance.Increment(1, 0) },
        { "override", () => instance.On_SampleOverride() },
        { "noise", () => instance.On_SampleNoise() },
        { "display", () => instance.On_Display() },
        { "}", () => instance.Increment(0, 1) },
    };

    public Dictionary<string, Func<int>> sampleNoiseOptions = new Dictionary<string, Func<int>>()
    {
        { "{", () => instance.Increment(1, 0) },
        { "size", () => instance.On_SampleNoiseSize() },
        { "threshold", () => instance.On_SampleNoiseThreshold() },
        { "clamp", () => instance.On_SampleNoiseClamp() },
        { "amplitude", () => instance.On_SampleNoiseAmplitude() },
        { "slide", () => instance.On_SampleNoiseSlide() },
        { "smooth", () => instance.On_SampleNoiseSmooth() },
        { "invert", () => instance.On_SampleNoiseInvert() },
        { "}", () => instance.Increment(1, 1) }
    };

    public Dictionary<string, Func<int>> sampleOverrideOptions = new Dictionary<string, Func<int>>()
    {
        { "{", () => instance.Increment(1, 0) },
        { "sample", () => instance.On_SampleOverrideSample() },
        { "add", () => instance.On_SampleOverrideAdd() },
        { "threshold", () => instance.On_SampleOverrideThreshold() },
        { "clamp", () => instance.On_SampleOverrideClamp() },
        { "slide", () => instance.On_SampleOverrideSlide() },
        { "smooth", () => instance.On_SampleOverrideSmooth() },
        { "invert", () => instance.On_SampleOverrideInvert() },
        { "}", () => instance.Increment(1, 1) }
    };

    public Dictionary<string, Func<int>> labels = new Dictionary<string, Func<int>>()
    {
        { "name", () => instance.On_Name() },
    };
}

public class CWorldMask
{
    public string name;
    public CWorldNoise noise;
    public CWorldMask mask;

    public CWorldMask(string name)
    {
        this.name = name;
        noise = null;
        mask = null;
    }
    
    public CWorldMask(string name, CWorldNoise noise)
    {
        this.name = name;
        this.noise = noise;
        mask = null;
    }

    public float GetNoiseValue(int x, int y)
    {
        float value = noise.GetNoiseValue(x, y);

        if (mask != null)
            value *= mask.GetNoiseValue(x, y);

        return value;
    }
}

public class CWorldNoise
{
    public float sizeX;
    public float sizeY;

    public float t_min;
    public float t_max;

    public float c_min;
    public float c_max;

    public bool t_smooth;
    public bool t_slide;
    public bool invert;

    public CWorldNoise()
    {
        sizeX = 20;
        sizeY = 20;
        
        t_min = 0;
        t_max = 1;

        c_min = 0;
        c_max = 1;

        t_smooth = false;
        t_slide = false;
    }

    public override string ToString()
    {
        return $"{sizeX}, {sizeY}, {t_min}, {t_max}, {c_min}, {c_max},";
    }
    
    public float GetNoiseValue(int x, int y)
    {
        float height = Mathf.Clamp(Mathf.PerlinNoise((float)((float)x / sizeX + 0.001f), (float)((float)y / sizeY + 0.001f)), c_min, c_max);
        
        if (t_smooth)
            height = Mathp.PLerp(t_min, t_max, height);
        if (t_slide)
            height = Mathp.SLerp(t_min, t_max, height);
        if (invert)
            height = 1 - height;

        return height;
    }
}