using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class WMWriter : MonoBehaviour
{
    public static WMWriter instance;
    
    public TMP_Text inputField;
    public TMP_Text log;
    private string[] lines;

    private bool showChunkGen = false;
    
    private char[] charactersToReplace = new char[] { '(', ')', '=', '{', '}', ',', ':', '/'};

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCode()
    {
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
        
        lines = input.Split(new[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            Debug.Log(line.Trim());
        }
        
        int index = IsComment(0);
    }
    
    public string CommandTest(int index, Dictionary<string, Func<string>> commands)
    {
        string command = lines[index];
        Debug.Log("hello1");
        if (commands.TryGetValue(command, out Func<string> func))
        {
            Debug.Log("hello2");
            return func();
        }
        return $"Invalid command: {command}";
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

    public string Do_Mask()
    {
        return "";
    }

    public string Do_Biome()
    {
        return "";
    }
    
    public Dictionary<string, Func<string>> types = new Dictionary<string, Func<string>>()
    {
        { "Mask", () => instance.Do_Mask() },
        { "Biome", () => instance.Do_Biome() },
    };
}
