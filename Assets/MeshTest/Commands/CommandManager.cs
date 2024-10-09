using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;

    public Chunk chunk;
    public ChunkSettingsSO chunkSettings;
    
    public TMP_Text inputField;
    public TMP_Text log;
    private string[] args;

    private bool showChunkGen = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCode()
    {
        string input = inputField.text;
        input = Regex.Replace(input, @"\u200B", "").Trim();
        args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string command = args[0];
        log.text = CommandTest(0, baseCommands);
        inputField.text = "";
    }
    
    public string CommandTest(int index, Dictionary<string, Func<string>> commands)
    {
        string command = args[index];
        Debug.Log("hello1");
        if (commands.TryGetValue(command, out Func<string> func))
        {
            Debug.Log("hello2");
            return func();
        }
        return $"Invalid command: {command}";
    }

    public bool CommandWithCoordinatesTest(int index, Dictionary<string, Func<string>> commands, out Coords coords)
    {
        coords = new Coords();
        string coord1 = args[index];
        string coord2 = args[index + 1];
        string coord3 = args[index + 2];

        bool isCoord1 = float.TryParse(coord1, out float floatCoord1);
        bool isCoord2 = float.TryParse(coord2, out float floatCoord2);
        bool isCoord3 = float.TryParse(coord3, out float floatCoord3);
        
        if (isCoord1)
            coords.x = floatCoord1;
        else if (commands.TryGetValue(coord1, out Func<string> func))
            coords.x = 0;
        else
        {
            coords.result = $"Invalid command: {coord1}";
            return false;
        }
        
        if (isCoord2)
            coords.y = floatCoord2;
        else if (commands.TryGetValue(coord2, out Func<string> func))
            coords.y = 0;
        else
        {
            coords.result = $"Invalid command: {coord2}";
            return false;
        }
        
        if (isCoord3)
            coords.z = floatCoord3;
        else if (commands.TryGetValue(coord3, out Func<string> func))
            coords.z = 0;
        else
        {
            coords.result = $"Invalid command: {coord3}";
            return false;
        }


        return true;
    }

    public bool CommandWithValuesTest(int index, out float value)
    {
        value = 0;
        if (index >= args.Length)
            return false;
        
        if (float.TryParse(args[index], out float floatValue))
        {
            value = floatValue;
            return true;
        }

        return false;
    }


    public string Do_Generate()
    {
        return CommandTest(1, generateCommands);
    }

    public string Do_Generate_Raw()
    {
        if (CommandWithCoordinatesTest(2, generateCommands, out Coords coords))
        {
            int x = (int)coords.x - ((int)coords.x & 31);
            int y = (int)coords.y - ((int)coords.y & 31);
            int z = (int)coords.z - ((int)coords.z & 31);
            Vector3Int position = new Vector3Int(x, y, z);
            chunk.chunksToAdd.Add(position);
            chunk.AddChunks();
        }
        return coords.result;
    }
    
    public string Do_Generate_Add()
    {
        if (AddChunksToList(2, chunk.chunksToAdd, out string result)) 
            chunk.AddChunks();

        return result;
    }
    
    public string Do_Generate_Box()
    {
        string result = "";
        
        int xSize;
        int ySize;
        int zSize;

        if (CommandWithValuesTest(2, out float valueX))
            xSize = (int)valueX;
        else
            return "invalid or not enough values";
        
        if (CommandWithValuesTest(3, out float valueY))
            ySize = (int)valueY;
        else
            return "invalid or not enough values";
        
        if (CommandWithValuesTest(4, out float valueZ))
            zSize = (int)valueZ;
        else
            return "invalid or not enough values";
        
        Vector3Int pos = new Vector3Int(xSize, ySize, zSize);
        
        if (CommandWithValuesTest(5, out float valueX2))
            xSize = (int)valueX2;
        else
            return "invalid or not enough values";
        
        if (CommandWithValuesTest(6, out float valueY2))
            ySize = (int)valueY2;
        else
            return "invalid or not enough values";
        
        if (CommandWithValuesTest(7, out float valueZ2))
            zSize = (int)valueZ2;
        else
            return "invalid or not enough values";
        
        List<Vector3Int> discard = new List<Vector3Int>();
        
        if (args.Length > 8)
        {
            if (args[8].Equals("discard"))
            {
                if (!AddChunksToList(9, discard, out result))
                    return result;
            }
        }
        
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    Vector3Int newPos = new Vector3Int(pos.x + x * 32, pos.y + y * 32, pos.z + z * 32);
                    if (!discard.Contains(newPos))
                    {
                        chunk.chunksToAdd.Add(newPos);
                    }
                }
            }
        }
        chunk.AddChunks();
        
        return result;
    }

    public string Do_Rule()
    {
        return CommandTest(1, ruleCommands);
    }

    public string Do_Rule_Chunk()
    {
        return CommandTest(2, chunkCommands);
    }

    public string Do_Rule_Chunk_SetDensity()
    {
        if (CommandWithValuesTest(3, out float value))
        {
            chunkSettings.density = value;
            return "";
        }

        return "Invalid value";
    }

    public string Do_Rule_Chunk_SetShowNoise()
    {
        if (SetBool(ref chunkSettings.showNoise))
            return "";
        return "Invalid value";
    }

    public string Do_Rule_Chunk_SetShowGeneratingChunk()
    {
        if (SetBool(ref showChunkGen))
            return "";
        return "Invalid value";
    }

    public bool SetBool(ref bool value)
    {
        switch (args[3])
        {
            case "true": 
                value = true;
                return true;
            case "false":
                value = false;
                return true;
            default:
                return false;
        }
    }



    public bool AddChunksToList(int index, List<Vector3Int> positions, out string result)
    {
        result = "";
        if (CommandWithValuesTest(index, out float value))
        {
            for (int i = 0; i < (int)value; i++)
            {
                if (CommandWithCoordinatesTest(index + 1 + i * 3, generateCommands, out Coords coords))
                {
                    int x = (int)coords.x - ((int)coords.x & 31);
                    int y = (int)coords.y - ((int)coords.y & 31);
                    int z = (int)coords.z - ((int)coords.z & 31);
                    Vector3Int position = new Vector3Int(x, y, z);
                    positions.Add(position);
                    
                }
                else
                {
                    result = "Invalid value";
                    return false;
                }
            }
            return true;
        }
        result = "Invalid value";
        return false;
    }
    
    

    public Dictionary<string, Func<string>> baseCommands = new Dictionary<string, Func<string>>()
    {
        { "generate", () => instance.Do_Generate() },
        { "rule", () => instance.Do_Rule() },
        { "info", () => instance.Do_Rule() },
    };
    
    
    
    #region generate commands    
    public Dictionary<string, Func<string>> generateCommands = new Dictionary<string, Func<string>>()
    {
        { "raw", () => instance.Do_Generate_Raw() },
        { "add", () => instance.Do_Generate_Add() },
        { "box", () => instance.Do_Generate_Box() },
    };
    #endregion
    
    
    
    #region rule commands
    public Dictionary<string, Func<string>> ruleCommands = new Dictionary<string, Func<string>>()
    {
        { "chunk", () => instance.Do_Rule_Chunk() },
    };
    
    public Dictionary<string, Func<string>> chunkCommands = new Dictionary<string, Func<string>>()
    {
        { "density", () => instance.Do_Rule_Chunk_SetDensity() },
        { "showNoise", () => instance.Do_Rule_Chunk_SetShowNoise() },
        { "showGeneratingChunk", () => instance.Do_Rule_Chunk_SetShowGeneratingChunk() },
    };
    #endregion

    public class Coords
    {
        public float x;
        public float y;
        public float z;
        public string result;

        public Coords(float x, float y, float z, string result)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.result = result; 
        }
        
        public Coords()
        {
            x = 0;
            y = 0;
            z = 0;
            result = ""; 
        }

        public override string ToString()
        {
            return $"x: {x}, y: {y}, z: {z}, result: {result}";
        }
    }
}
