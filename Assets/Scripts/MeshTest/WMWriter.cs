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

    private Dictionary<string, > samples;

    private string currentName = "";
    private string currentType = "";

    private void Start()
    {
        masks = new Dictionary<string, CWorldMask>();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCode()
    {
        masks.Clear();
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
        if (CommandsTest(sampleLabel) == -1) return Error("Problem in the sample label found");
        if (CommandsTest(sampleSettings) == -1) return Error("Problem in the sample settings found");
        return 0;
    }

    public int On_SampleName()
    {
        index++;
        
    }

    public int On_SampleNoise()
    {
        
    }

    #region noise settings
    public int On_SampleNoiseSize()
    {
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the size");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.sizeX = floats.x;
        sampleNode.noise.sizeY = floats.y;

        return 0;
    }

    public int On_SampleNoiseThreshold()
    {
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the threshold");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.t_min = floats.x;
        sampleNode.noise.t_max = floats.y;

        return 0;
    }

    public int On_SampleNoiseClamp()
    {
        if (GetNext2Floats(out Vector2 floats) == -1)
            return Error("A problem was found while writing the clamp");
        
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");

        sampleNode.noise.c_min = floats.x;
        sampleNode.noise.c_max = floats.y;

        return 0;
    }

    public int On_SampleNoiseSlide()
    {
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.noise.t_slide = true;
        return 0;
    }

    public int On_SampleNoiseSmooth()
    {
        if (currentNode is not CSampleNode sampleNode) return Error("Something went wrong");
        sampleNode.noise.t_smooth = true;
        return 0;
    }

    public int On_SampleNoiseInvert()
    {
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
            if (!float.TryParse(lines[index], out float x)) return Error("No valid min value found");
            index++;
            if (!lines[index].Equals(",")) return Error("',' is missing");
            index++;
            if (!float.TryParse(lines[index], out float y)) return Error("No valid max value found");

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

    public int On_SampleSettings()
    {
        
    }
    
    public int On_Biome() { return 0; }

    public int On_Name()
    {
        Debug.Log("Name test");
        index++;
        
        if (lines[index].Equals(")"))
            return Error("'= name' expected but ')' found");
        
        if (!lines[index].Equals("="))
            return Error("No '=' found");
        index++;

        if (lines[index].Equals(")"))
            return Error("'name' expected but ) found");
        
        currentName = lines[index];
        return 2;
    } 
    
    
    public int On_Override() { return 0; }

    public int On_Noise()
    {
        int message;
        Debug.Log(currentType + " " + currentName);
        if (currentType.Equals("Mask"))
        {
            if (currentName.Equals(""))
                return Error("A name is expected when doing a Mask 'Mask ( name = 'name' )'");
            
            masks[currentName].noise = new CWorldNoise();

            message = Handle_Options(4);
        }

        return 0;
    } 
    public int On_Specifics() { return 0; }

    public int On_Do()
    {
        return Handle_Options(0);
    }

    public int On_S_Mask()
    {
        if (!MaxIndex(2))
            return Error("Not enough parameters in the option section");
        
        index++;
        
        if (!lines[index].Equals(":")) 
            return Error("':' expected");
        index++;

        if (!masks.TryGetValue(lines[index], out CWorldMask mask))
        {
            int value = Error("No valid min float value");
            Debug.Log(value);
            return 0;
        }

        Debug.Log("Helloooooooo");

        masks[currentName].mask = mask;
        
        if (!lines[index].Equals(","))
        {
            return 0;
        }
            
        return 0;
    }

    public int On_Parameters()
    {
        if (!MaxIndex(4))
            return Error("Not enough parameters in the option section");
        
        index++;
        
        if (!lines[index].Equals(":")) 
            return Error("':' expected");
        index++;

        if (!float.TryParse(lines[index], out float x))
            return Error("No valid x float value");
        index++;
        
        if (!lines[index].Equals(",")) 
            return Error("At least one more value expectec");
        index++;
        
        if (!float.TryParse(lines[index], out float y))
            return Error("No valid y float value");

        masks[currentName].noise.sizeX = x;
        masks[currentName].noise.sizeY = y;
        
        if (!lines[index].Equals(","))
        {
            return 0;
        }
            
        return 0;
    }

    public int On_Threshold()
    {
        if (!MaxIndex(4))
            return Error("Not enough parameters in the option section");
        
        index++;
        
        if (!lines[index].Equals(":")) 
            return Error("':' expected");
        index++;

        if (!float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float min))
        {
            Debug.Log(lines[index]);
            return Error("No valid min float value");
        }
        index++;
        
        if (!lines[index].Equals(",")) 
            return Error("At least one more value expectec");
        index++;
        
        if (!float.TryParse(lines[index], NumberStyles.Float, CultureInfo.InvariantCulture, out float max))
            return Error("No valid min float value");

        masks[currentName].noise.t_min = min;
        masks[currentName].noise.t_max = max;
        
        if (!lines[index].Equals(","))
        {
            return 0;
        }
            
        return 0;
    }

    public int On_Clamp()
    {
        if (!MaxIndex(4))
            return Error("Not enough parameters in the option section");
        
        index++;
        
        if (!lines[index].Equals(":")) 
            return Error("':' expected");
        index++;

        if (!float.TryParse(lines[index], out float min))
            return Error("No valid min float value");
        index++;
        
        if (!lines[index].Equals(",")) 
            return Error("At least one more value expectec");
        index++;
        
        if (!float.TryParse(lines[index], out float max))
            return Error("No valid min float value");

        masks[currentName].noise.c_min = min;
        masks[currentName].noise.c_max = max;
        
        if (!lines[index].Equals(","))
        {
            return 0;
        }
            
        return 0;
    }

    public int On_TSmooth()
    {
        if (currentType.Equals("Mask"))
        {
            masks[currentName].noise.t_smooth = true;
        }

        return 0;
    }

    public int On_TSlide()
    {
        if (currentType.Equals("Mask"))
        {
            masks[currentName].noise.t_slide = true;
        }

        return 0;
    }

    public int On_Invert()
    {
        if (currentType.Equals("Mask"))
        {
            masks[currentName].noise.invert = true;
        }

        return 0;
    }

    public int On_Display()
    {
        if (currentType.Equals("Mask"))
        {
            textureGeneration.UpdateTexture(masks[currentName]);
        }

        return 0;
    }


    public int Handle_Options(int maxIndex)
    {
        int message;
        index++;
        if (lines[index].Equals("{"))
        {
            Debug.Log("setting start...");
            while (index < lines.Length)
            {
                index++;
                
                if (lines[index].Equals("{"))
                    return Error("Extra '{' found");
            
                if (lines[index].Equals("}"))
                    break;
            
                if (!MaxIndex(maxIndex)) 
                    return Error("not enough parameters in the settings section");
            
                Debug.Log("settings test start...");
                message = CommandTest(index, settings);
                Debug.Log("settings test end");

                if (message.Equals("error"))
                    return Error("label error");
            }
            Debug.Log("setting done");
        }

        return 0;
    }
    
    public Dictionary<string, Func<int>> types = new Dictionary<string, Func<int>>()
    {
        { "Sample", () => instance.On_Sample() },
        { "Biome", () => instance.On_Biome() },
    };
    
    public Dictionary<string, Func<int>> sampleLabel = new Dictionary<string, Func<int>>()
    {
        { "(", () => { instance.index++; return 0; } },
        { "name", () => instance.On_SampleName() },
        { ")", () => { instance.index++; return 1; } },
    };
    
    public Dictionary<string, Func<int>> sampleSettings = new Dictionary<string, Func<int>>()
    {
        { "{", () => { instance.index++; return 0; } },
        { "override", () => instance.On_SampleSettings() },
        { "noise", () => instance.On_SampleNoise() },
        { "display", () => instance.On_Display() },
        { "}", () => { instance.index++; return 1; } },
    };

    public Dictionary<string, Func<int>> sampleNoiseOptions = new Dictionary<string, Func<int>>()
    {
        { "size", () => instance.On_SampleNoiseSize() },
        { "threshold", () => instance.On_SampleNoiseThreshold() },
        { "clamp", () => instance.On_SampleNoiseClamp() },
        { "slide", () => instance.On_SampleNoiseSlide() },
        { "smooth", () => instance.On_SampleNoiseSmooth() },
        { "invert", () => instance.On_SampleNoiseInvert() },
    };

    public Dictionary<string, Func<int>> labels = new Dictionary<string, Func<int>>()
    {
        { "name", () => instance.On_Name() },
    };

    public Dictionary<string, Func<int>> options = new Dictionary<string, Func<int>>()
    {
        { "override", () => instance.On_Override() },
        { "noise", () => instance.On_Noise() },
        { "specifics", () => instance.On_Specifics() },
        { "do", () => instance.On_Do() },
    };

    public Dictionary<string, Func<int>> settings = new Dictionary<string, Func<int>>()
    {
        { "mask", () => instance.On_S_Mask() },
        { "parameters", () => instance.On_Parameters() },
        { "threshold", () => instance.On_Threshold() },
        { "clamp", () => instance.On_Clamp() },
        { "t-smooth", () => instance.On_TSmooth() },
        { "t-slide", () => instance.On_TSlide() },
        { "invert", () => instance.On_Invert()},
        { "display", () => instance.On_Display() },
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