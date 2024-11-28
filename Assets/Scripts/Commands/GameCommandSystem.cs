using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameCommandSystem : MonoBehaviour
{
    public static GameCommandSystem instance;
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
                /*
                if (!WorldChunks.activeChunkData.ContainsKey(result.position)) 
                    return;
                    */

                if (result.isEmpty())
                    return;
                
                Debug.Log("Rendering: " + result.position);
                
                GameObject newChunk = Instantiate(chunkPrefab, result.position, Quaternion.identity, parentChunk);
                ChunkRenderer chunkRenderer = newChunk.GetComponent<ChunkRenderer>();
                chunkRenderer.RenderChunk(result);
                
                WorldChunks.activeChunks.TryAdd(result.position, chunkRenderer);
                
                result.Clear();
                result = null;
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
                    Debug.Log("Dequeued: " + result);
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
                    ChunkData chunkData = new ChunkData(result);
                    //WorldChunks.activeChunkData.TryAdd(result, );
                    
                    if (Vector3Int.Distance(center, result) > distance)
                        ChunkGenerationNodes.tasks[i] = Chunk.CreateMapChunk(chunkData, result, ChunkGenerationNodes.dataHandlers[i], this, 1);
                    else
                        ChunkGenerationNodes.tasks[i] = Chunk.CreateMapChunk(chunkData, result, ChunkGenerationNodes.dataHandlers[i], this, 0);
                    
                    await ChunkGenerationNodes.tasks[i];
                    ChunkGenerationNodes.tasks[i] = null;
                }
            }
        }
    }

    public void ExecuteCommand()
    {
        ExecuteCommand(textField.text);
        textField.text = "";
    }
    
    public void ExecuteCommand(string content)
    {
        string input = content;
        input = input.Replace("\u200B", "").Trim();
        args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        log.text = CommandTest(0, baseCommands);
        args = null;
    }
    
    public void ExecuteCommand(string[] preArgs)
    {
        args = preArgs;
        log.text = CommandTest(0, baseCommands);
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
            
            if (args[8].Trim().Equals("sample"))
            {
                if (ChunkGenerationNodes.dataHandlers[0].sampleNodes.ContainsKey(args[9]))
                {
                    Console.Log("The sample was found");
                    Console.Log("Generating sample...");
                    ChunkGenerationNodes.SetupSamplePool(args[9]);
                    
                    for (int x = 0; x < x2; x++)
                    {
                        for (int y = 0; y < y2; y++)
                        {
                            for (int z = 0; z < z2; z++)
                            {
                                Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);
                                _sampleChunks.Enqueue(position);
                            }
                        }
                    }
                }
                else
                {
                    Console.Log("The sample doesn't exist");
                }
            }
            else if (args[8].Trim().Equals("biome"))
            {
                if (ChunkGenerationNodes.dataHandlers[0].biomeNodes.ContainsKey(args[9]))
                {
                    Console.Log("The biome was found");
                    Console.Log("Generating biome...");
                    
                    currentName = args[9];
                    
                    for (int x = 0; x < x2; x++)
                    {
                        for (int y = 0; y < y2; y++)
                        {
                            for (int z = 0; z < z2; z++)
                            {
                                Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);
                                _biomeChunks.Enqueue(position);
                                
                                Debug.Log("Enqueued: " + position);
                            }
                        }
                    }
                }
                else
                {
                    Console.Log("The biome doesn't exist");
                }
            }
            else if (args[8].Trim().Equals("map"))
            {
                if (ChunkGenerationNodes.dataHandlers[0].MapNode != null)
                {
                    Console.Log("Generating map...");
                    
                    for (int x = 0; x < x2; x++)
                    {
                        for (int y = 0; y < y2; y++)
                        {
                            for (int z = 0; z < z2; z++)
                            {
                                Vector3Int position = new Vector3Int(x1 + x * 32, y1 + y * 32, z1 + z * 32);
                                _mapChunks.Enqueue(position);
                            }
                        }
                    }
                }
                else
                {
                    Console.Log("The map doesn't exist");
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
        
        WorldChunks.ClearChunks();

        return "Done";
    }

    public string Do_Generate_Line()
    {
        if (IsNumericValue(2, out int x1) && 
            IsNumericValue(3, out int y1) && 
            IsNumericValue(4, out int z1) &&
            IsNumericValue(5, out int x2) && 
            IsNumericValue(6, out int y2) && 
            IsNumericValue(7, out int z2) &&
            IsNumericValue(8, out float r))
        {
            Vector3Int a = new Vector3Int(x1, y1, z1);
            Vector3Int b = new Vector3Int(x2, y2, z2);

            List<Vector3Int> points = Chunk.Bresenham3D(a, b, r);
            HashSet<Vector3Int> chunksToUpdate = new HashSet<Vector3Int>();
            HashSet<Vector3Int> chunksToCreate = new HashSet<Vector3Int>();

            foreach (var point in points)
            {
                Vector3Int chunkPosition = Chunk.GetChunkPosition(point);
                chunksToUpdate.Add(chunkPosition);

                if (WorldChunks.activeChunkData.TryGetValue(chunkPosition, out var chunkData))
                {
                    chunkData ??= new ChunkData(chunkPosition);
                    chunkData.blocks ??= new Block[32768];
                }
                else
                {
                    chunkData = new ChunkData(chunkPosition)
                    {
                        meshData = new MeshData(),
                        blocks = new Block[32768]
                    };
                    
                    if (WorldChunks.activeChunkData.TryAdd(chunkPosition, chunkData))
                        chunksToCreate.Add(chunkPosition);
                }

                Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
                int index = position.x + position.z * 32 + position.y * 1024;
                chunkData.blocks[index] = new Block(2, 0);
            }
            
            foreach (var chunk in chunksToCreate)
            {
                if (WorldChunks.activeChunks.ContainsKey(chunk)) continue;
                
                GameObject newChunk = Instantiate(chunkPrefab, chunk, Quaternion.identity, parentChunk);
                WorldChunks.activeChunks.TryAdd(chunk, newChunk.GetComponent<ChunkRenderer>());
            }
            
            foreach (var chunk in chunksToUpdate)
            {
                Console.Log(chunk.ToString());
                if (!WorldChunks.activeChunkData.TryGetValue(chunk, out var chunkData) || 
                    !WorldChunks.activeChunks.TryGetValue(chunk, out var chunkRenderer)) continue;
                
                Chunk.UpdateChunk(chunkData);
                chunkRenderer.RenderChunk(chunkData);
            }
            
            return "Done";
        }
        
        return "The values set are not valid";
    }

    public string Do_Generate_Foliage()
    {
        Console.Log("Generating tree...");
        if (IsNumericValue(2, out int x1) && 
            IsNumericValue(3, out int y1) && 
            IsNumericValue(4, out int z1))
        {
            if (ChunkGenerationNodes.dataHandlers[0].foliageNodes.TryGetValue(args[5], out var foliageNode))
            {
                Console.Log("The tree was found");
                foliageNode.Generate(x1, y1, z1);
                foreach (var chunk in WorldChunks.chunksToUpdate)
                {
                    ChunkData chunkData = chunk.Value;
                    chunkData.meshData = new MeshData();
                    Chunk.UpdateChunk(chunkData);
                    WorldChunks.activeChunkData.TryAdd(chunk.Key, chunkData);
                    chunks.Enqueue(chunkData);
                }
                
                return "Done";
            }
            
            Console.Log("The tree was not found, generating default tree");
            
            if (IsNumericValue(5, out int min) && 
                IsNumericValue(6, out int max))
            {
                HashSet<Vector3Int> chunksToUpdate = new HashSet<Vector3Int>();
                HashSet<Vector3Int> chunksToCreate = new HashSet<Vector3Int>();

                ITreeSampler sampler = new TreeBasic(y1);
                IntRangeNode range = new IntRangeNode(min, max);

                int height = sampler.Sample();
                int treeHeight = (int)NoiseUtils.GetRandomRange(range.min, range.max);

                Debug.Log(height + " " + treeHeight);

                for (int y = 0; y < treeHeight; y++)
                {
                    Vector3Int point = new Vector3Int(x1, y + height, z1);
                    Debug.Log(point);
                    Vector3Int chunkPosition = Chunk.GetChunkPosition(point);
                    chunksToUpdate.Add(chunkPosition);

                    if (WorldChunks.activeChunkData.TryGetValue(chunkPosition, out var chunkData))
                    {
                        chunkData ??= new ChunkData(chunkPosition);
                        chunkData.blocks ??= new Block[32768];
                    }
                    else
                    {
                        chunkData = new ChunkData(chunkPosition)
                        {
                            meshData = new MeshData(),
                            blocks = new Block[32768]
                        };

                        if (WorldChunks.activeChunkData.TryAdd(chunkPosition, chunkData))
                            chunksToCreate.Add(chunkPosition);
                    }

                    Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
                    int index = position.x + position.z * 32 + position.y * 1024;
                    chunkData.blocks[index] = new Block(2, 0);
                }

                foreach (var chunk in chunksToCreate)
                {
                    if (WorldChunks.activeChunks.ContainsKey(chunk)) continue;

                    GameObject newChunk = Instantiate(chunkPrefab, chunk, Quaternion.identity, parentChunk);
                    WorldChunks.activeChunks.TryAdd(chunk, newChunk.GetComponent<ChunkRenderer>());
                }

                foreach (var chunk in chunksToUpdate)
                {
                    Console.Log(chunk.ToString());
                    if (!WorldChunks.activeChunkData.TryGetValue(chunk, out var chunkData) ||
                        !WorldChunks.activeChunks.TryGetValue(chunk, out var chunkRenderer)) continue;

                    Chunk.UpdateChunk(chunkData);
                    chunkRenderer.RenderChunk(chunkData);
                }
                return "Done";
            }
        }

        return "Error";
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
        { "line", () => instance.Do_Generate_Line() },
        { "foliage", () => instance.Do_Generate_Foliage() },
    };
}