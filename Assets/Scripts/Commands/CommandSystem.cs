using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandSystem : MonoBehaviour
{
    public static CommandSystem instance;
    [SerializeField]
    public TMP_Text textField;
    [SerializeField]
    public TMP_Text log;

    public World worldScript;
    public Chunk chunkScript;

    public GameObject chunkPrefab;
    
    private string[] args;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ExecuteCommand()
    {
        string input = textField.text;
        input = input.Replace("\u200B", "").Trim();
        args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        log.text = CommandTest(0, baseCommands);
        textField.text = "";
        args = null;
    }

    private string CommandTest(int index, Dictionary<string, Func<string>> commands)
    {
        string command = (string)args[index].Trim();
        
        if (commands.TryGetValue(command, out var function))
        {
            return function();
        }
        
        return $"The command {command} doesn't exist";
    }


    private bool IsNumericValue(int index, out float number)
    {
        return float.TryParse(args[index].Trim(), out number);
    }
    
    private bool IsNumericValue(int index, out int number)
    {
        return int.TryParse(args[index].Trim(), out number);
    }
    
    

    public string Do_Rule()
    {
        return CommandTest(1, ruleCommands);
    }

    
    
    public string Do_Info()
    {
        return CommandTest(1, infoCommands);
    }
    
    public string Do_Info_World()
    {
        return CommandTest(2, worldInfoCommands);
    }
    
    public string Do_Info_World_Chunk()
    {
        return CommandTest(3, chunkWorldInfoCommands);
    }
    
    public string Do_Info_World_Chunk_Debug()
    {
        return CommandTest(4, debugChunkWorldInfoCommands);
    }

    public string Do_Info_World_Chunk_Debug_Side()
    {
        string result = "";

        foreach (var chunks in worldScript.worldData.activeChunkData)
        {
            Debug.Log($"Position: {chunks.Key} sideChunks: {ChunkInfo.GetSideChunks(chunks.Value.sideChunks)}\n");
        }
 
        return "done...";
    }

    public string Do_Rule_WorldGen()
    {
        return CommandTest(2, worldGenCommands);
    }

    public string Do_Rule_WorldGen_Delay()
    {
        return "hello";
    }

    public string Do_Generate()
    {
        return CommandTest(1, generateCommands);
    }

    public string Do_Generate_Box()
    {
        if (IsNumericValue(2, out int x1))
        {
            if (IsNumericValue(3, out int y1))
            {
                if (IsNumericValue(4, out int z1))
                {
                    x1 -= x1 & 31;
                    y1 -= y1 & 31; 
                    z1 -= z1 & 31;
                    
                    if (IsNumericValue(5, out int x2))
                    {
                        if (IsNumericValue(6, out int y2))
                        {
                            if (IsNumericValue(7, out int z2))
                            {
                                for (int x = 0; x < x2; x++)
                                {
                                    for (int y = 0; y < y2; y++)
                                    {
                                        for (int z = 0; z < z2; z++)
                                        {
                                            Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);
                                            ChunkData chunkData = new ChunkData(position);
                                            chunkScript.CreateChunk(chunkData, position);

                                            GameObject newChunk = Instantiate(chunkPrefab, position, Quaternion.identity);
                                            newChunk.GetComponent<ChunkRenderer>().RenderChunk(chunkData);
                                        }
                                    }
                                }
                                
                                return "Done";
                            }
                        }
                    }
                }
            }
        }

        return "The values set are not valid";
    }
    

    private Dictionary<string, Func<string>> baseCommands = new Dictionary<string, Func<string>>
    {
        { "rule", () => instance.Do_Rule() },
        { "info", () => instance.Do_Info() },
        { "teleport", () => instance.Do_Rule() },
        { "generate", () => instance.Do_Generate() },
    };
    
    private Dictionary<string, Func<string>> ruleCommands = new Dictionary<string, Func<string>>
    {
        { "worldGeneration", () => instance.Do_Rule_WorldGen() },
        { "wg", () => instance.Do_Rule_WorldGen() },
        { "teleport", () => instance.Do_Rule() },
    };
    
    private Dictionary<string, Func<string>> worldGenCommands = new Dictionary<string, Func<string>>
    {
        { "delay", () => instance.Do_Rule_WorldGen_Delay() },
        { "teleport", () => instance.Do_Rule_WorldGen_Delay() },
    };
    
    private Dictionary<string, Func<string>> infoCommands = new Dictionary<string, Func<string>>
    {
        { "world", () => instance.Do_Info_World() },
    };
    
    private Dictionary<string, Func<string>> worldInfoCommands = new Dictionary<string, Func<string>>
    {
        { "chunk", () => instance.Do_Info_World_Chunk() },
    };
    
    private Dictionary<string, Func<string>> chunkWorldInfoCommands = new Dictionary<string, Func<string>>
    {
        { "debug", () => instance.Do_Info_World_Chunk_Debug() },
    };
    
    private Dictionary<string, Func<string>> debugChunkWorldInfoCommands = new Dictionary<string, Func<string>>
    {
        { "side", () => instance.Do_Info_World_Chunk_Debug_Side() },
    };

    private Dictionary<string, Func<string>> generateCommands = new Dictionary<string, Func<string>>
    {
        { "box", () => instance.Do_Generate_Box() },
    };
}