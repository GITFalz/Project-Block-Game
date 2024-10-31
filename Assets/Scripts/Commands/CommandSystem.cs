using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CommandSystem : MonoBehaviour
{
    public static CommandSystem instance;
    [SerializeField]
    public TMP_Text textField;
    [SerializeField]
    public TMP_Text log;

    public World worldScript;
    public Chunk chunkScript;
    public CWorldHandler handler;

    public Transform parentChunk;

    public GameObject chunkPrefab;

    public BlockManager blockManager;
    public BiomeSO biome;

    public int threadCount = 4;
    
    private string[] args;
    
    public ConcurrentQueue<ChunkData> chunks;
    private ConcurrentQueue<Vector3Int> _sampleChunks;
    private ConcurrentQueue<Vector3Int> _biomeChunks;
    private ConcurrentQueue<Vector3Int> _mapChunks;

    private List<Task> tasks;
    private List<CWorldDataHandler> _dataHandlers;

    private Task thread1 = null;
    private Task thread2 = null;
    private string currentName = "";

    private bool lod;
    private int distance;
    private Vector3Int center;

    private CWorldDataHandler dataHandler;

    private void Start()
    {
        chunks = new ConcurrentQueue<ChunkData>();
        _sampleChunks = new ConcurrentQueue<Vector3Int>();
        _biomeChunks = new ConcurrentQueue<Vector3Int>();
        _mapChunks = new ConcurrentQueue<Vector3Int>();

        
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        if (_sampleChunks.Count > 0)
        {
            GenerateChunks();
        }
        else if (_biomeChunks.Count > 0)
        {
            GenerateBiomeChunks();
        }
        else if (_mapChunks.Count > 0)
        {
            GenerateMapChunks();
        }

        if (chunks.Count > 0)
        {
            if (chunks.TryDequeue(out var result))
            {
                GameObject newChunk = Instantiate(chunkPrefab, result.position, Quaternion.identity, parentChunk);
                newChunk.GetComponent<ChunkRenderer>().RenderChunk(result);
            }
        }
    }

    private async void GenerateChunks()
    {
        for (int i = 0; i < ChunkGenerationNodes.threadCount; i++)
        {
            if (ChunkGenerationNodes.tasks[i] == null)
            {
                if (_sampleChunks.TryDequeue(out var result))
                {
                    ChunkGenerationNodes.tasks[i] = Chunk.CreateChunk(new ChunkData(result), result, currentName, this, ChunkGenerationNodes.dataHandlers[i] , biome);
                    await ChunkGenerationNodes.tasks[i];
                    ChunkGenerationNodes.tasks[i] = null;
                }
            }
        }
    }
    
    private async void GenerateBiomeChunks()
    {
        for (int i = 0; i < ChunkGenerationNodes.threadCount; i++)
        {
            if (ChunkGenerationNodes.tasks[i] == null)
            {
                if (_biomeChunks.TryDequeue(out var result))
                {
                    ChunkGenerationNodes.tasks[i] = Chunk.CreateBiomeChunk(new ChunkData(result), result, currentName, ChunkGenerationNodes.dataHandlers[i], this);
                    await ChunkGenerationNodes.tasks[i];
                    ChunkGenerationNodes.tasks[i] = null;
                }
            }
        }
    }
    
    private async void GenerateMapChunks()
    {
        for (int i = 0; i < ChunkGenerationNodes.threadCount; i++)
        {
            if (ChunkGenerationNodes.tasks[i] == null)
            {
                if (_mapChunks.TryDequeue(out var result))
                {
                    if (Vector3Int.Distance(center, result) > distance)
                        ChunkGenerationNodes.tasks[i] = Chunk.CreateMapChunk(new ChunkData(result), result, ChunkGenerationNodes.dataHandlers[i], this, 1);
                    else
                        ChunkGenerationNodes.tasks[i] = Chunk.CreateMapChunk(new ChunkData(result), result, ChunkGenerationNodes.dataHandlers[i], this, 0);
                
                    await ChunkGenerationNodes.tasks[i];
                    ChunkGenerationNodes.tasks[i] = null;
                }
            }
        }
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
        bool setupPool = true;
        if (IsNumericValue(2, out int x1) && 
            IsNumericValue(3, out int y1) && 
            IsNumericValue(4, out int z1) &&
            IsNumericValue(5, out int x2) && 
            IsNumericValue(6, out int y2) && 
            IsNumericValue(7, out int z2)) 
        {
            x1 -= x1 & 31;
            y1 -= y1 & 31; 
            z1 -= z1 & 31;
            
            for (int x = 0; x < x2; x++)
            {
                for (int y = 0; y < y2; y++)
                {
                    for (int z = 0; z < z2; z++)
                    {
                        if (args.Length > 9)
                        {
                            if (args[8].Trim().Equals("sample"))
                            {
                                if (ChunkGenerationNodes.dataHandlers[0].sampleNodes.ContainsKey(args[9]))
                                {
                                    Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);

                                    if (setupPool)
                                    {
                                        currentName = args[9];
                                        ChunkGenerationNodes.SetupSamplePool(currentName);
                                        setupPool = false;
                                    }

                                    _sampleChunks.Enqueue(position);
                                }
                            }
                            else if (args[8].Trim().Equals("biome"))
                            {
                                if (ChunkGenerationNodes.dataHandlers[0].biomeNodes.ContainsKey(args[9]))
                                {
                                    Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);

                                    if (setupPool)
                                    {
                                        currentName = args[9];
                                        ChunkGenerationNodes.SetupSamplePool(currentName);
                                        setupPool = false;
                                    }

                                    _biomeChunks.Enqueue(position);
                                }
                            }
                        }
                        else if (args[8].Trim().Equals("map"))
                        {
                            Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);

                            _mapChunks.Enqueue(position);
                        }
                        else
                        {
                            Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);
                            ChunkData chunkData = new ChunkData(position);
                            chunkScript.CreateChunk(chunkData, position);

                            GameObject newChunk = Instantiate(chunkPrefab, position, Quaternion.identity, parentChunk);
                            newChunk.GetComponent<ChunkRenderer>().RenderChunk(chunkData);
                        }
                    }
                }
            }
            
            return "Done";
        }

        return "The values set are not valid";
    }

    public string Do_Generate_Distance()
    {
        if (args.Length > 6)
        {
            if (args[5].Trim().Equals("lod") && IsNumericValue(6, out int lodDistance))
            {
                distance = lodDistance * 32;
            }
        }
        
        if (IsNumericValue(2, out int dist) && 
            IsNumericValue(3, out int height))
        {
            int loop;
            int dir;
            int x = 0;
            int z = 0;
            
            for (int d = 0; d < dist; d++)
            {
                dir = 0;
                loop = 1 + d * 2;
                
                for (int step = 0; step < 4 + d * 8; step++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (args[4].Trim().Equals("map"))
                        {
                            Vector3Int position = new Vector3Int(x * 32, y * 32, z * 32);

                            _mapChunks.Enqueue(position);
                        }
                    }
                    
                    switch (dir)
                    {
                        case 0:
                        {
                            x++;
                            break;
                        }
                        case 1:
                        {
                            z++;
                            break;
                        }
                        case 2:
                        {
                            x--;
                            break;
                        }
                        case 3:
                        {
                            z--;
                            break;
                        }
                    }

                    loop--;

                    if (loop == 0)
                    {
                        dir++;
                        loop = 1 + d * 2;
                    }
                }

                x--;
                z--;
            }
        }

        return "The values set are not valid";
    }

    public string Do_Generate_Clear()
    {
        foreach (Transform child in parentChunk)
        {
            Destroy(child.gameObject);
        }

        return "Done";
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
        { "distance", () => instance.Do_Generate_Distance() },
        { "clear", () => instance.Do_Generate_Clear() },
    };
}