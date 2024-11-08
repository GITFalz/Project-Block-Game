using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Chunk : MonoBehaviour
{
    public static Chunk instance;
    
    public NoiseSettings terrainNoise;
    public BiomeSO biome;
    [FormerlySerializedAs("blockManager")] public BlockManagerSO blockManagerSo;
    public GameObject chunkPrefab;
    
    public List<Vector3Int> chunksToAdd;

    private int chunkSize = 32768;
    private uint[] blockMap;
    private uint[][] greedyMap;
    private Block[] blocks;
    public Texture2D texture;
    public World worldScript;

    public CWorldHandler handler;
    public CommandSystem commandSystem;

    public bool generateSingle;

    private Vector3Int chunkPos;

    private void Start()
    {
        if (generateSingle)
        {
            texture = new Texture2D(20 * 32, 32, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            
            blockMap = GenerateTerrain(new Vector3Int(0, 0, 0));
            blocks = GenerateBlocks(blockMap, biome);
            
            SetPixels();
            
            texture.Apply();
            GetComponent<Renderer>().material.SetTexture("_terrainTexture", texture);
            
            SaveTexture(texture);
        }
    }
    
    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/RenderOutput";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/R_textureNew.png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public ChunkData CreateChunk(ChunkData newChunkData, Vector3Int position)
    {
        newChunkData.meshData = new MeshData();
        
        blockMap = GenerateTerrain(position);
        blocks = GenerateBlocks(blockMap, biome);
        newChunkData.SetBlocks(blocks);
        
        //World.WriteChunkData(chunkData, writer);
        GenerateMesh(newChunkData);

        return newChunkData;
    }
    
    public static async Task CreateChunk(ChunkData newChunkData, Vector3Int position, string sampleName, CommandSystem commandSystem, CWorldDataHandler handler, BiomeSO biome)
    {
        newChunkData.meshData = new MeshData();

        Block[] blocks = await CreateChunkAsync(position, sampleName, handler, biome);
        newChunkData.SetBlocks(blocks);

        GenerateMesh(newChunkData);
        
        commandSystem.chunks.Enqueue(newChunkData);
    }
    
    public static Task<Block[]> CreateChunkAsync(Vector3Int position, string sampleName, CWorldDataHandler handler, BiomeSO biome)
    {
        return Task.Run(() =>
        {
            uint[] blockMap = GenerateTerrain(position, sampleName, handler);
            return GenerateBlocks(blockMap, biome);
        });
    }
    
    public static async Task CreateBiomeChunk(ChunkData newChunkData, Vector3Int position, string biomeName, CWorldDataHandler handler, CommandSystem commandSystem)
    {
        newChunkData.meshData = new MeshData();
        
        Block[] blocks = await CreateBiomeChunkAsync(position, biomeName, handler);
        newChunkData.SetBlocks(blocks);

        GenerateMesh(newChunkData);
        
        commandSystem.chunks.Enqueue(newChunkData);
    }
    
    public static Task<Block[]> CreateBiomeChunkAsync(Vector3Int position, string biomeName, CWorldDataHandler handler)
    {
        return Task.Run(() =>
        {
            uint[] blockMap = new uint[1024];
            Block[] blocks = new Block[32768];
            
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    handler.Init(x + position.x, 0, z + position.z);
                    blockMap[x + z * 32] = handler.GenerateBiomePillar(position, blocks, x, z, biomeName);
                }
            }

            int index = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        if (blocks[index] != null)
                        {
                            blocks[index].occlusion = GetOcclusion(blockMap, x, y, z);
                        }
                        index++;
                    }
                }
            }

            return blocks;
        });
    }

    public static int[] lodWidth = new int[]
    {
        32, 16, 8, 4, 2, 1
    };
    
    public static int[] lodSize = new int[]
    {
        1, 2, 4, 8, 16, 32
    };
    
    public static async Task CreateMapChunk(ChunkData newChunkData, Vector3Int position, CWorldDataHandler handler, CommandSystem commandSystem, int lod)
    {
        await GenerateMapData(newChunkData, position, handler, lod);
        commandSystem.chunks.Enqueue(newChunkData);
    }
    
    public static async Task CreateMapChunk(ChunkData newChunkData, Vector3Int position, CWorldDataHandler handler, int lod)
    {
        await GenerateMapData(newChunkData, position, handler, lod);
        WorldGeneration.chunksToRender.Enqueue(newChunkData);
    }

    public static async Task GenerateMapData(ChunkData newChunkData, Vector3Int position, CWorldDataHandler handler, int lod)
    {
        newChunkData.meshData = new MeshData();
        
        if (lod == 0)
        {
            Block[] blocks = await CreateMapChunkAsync(position, handler);
            newChunkData.SetBlocks(blocks);
            GenerateMesh(newChunkData);
        }
        else
        {
            Block[] blocks = await CreateMapChunkAsync(position, handler, 1);
            GenerateMesh(newChunkData, blocks, lodWidth[lod], lodSize[lod], lod);
        }
    }
    
    public static Task<Block[]> CreateMapChunkAsync(Vector3Int position, CWorldDataHandler handler)
    {
        return Task.Run(() =>
        {
            uint[] blockMap = new uint[1024];
            Block[] blocks = new Block[32768];
            
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    handler.Init(x + position.x, 0, z + position.z);
                    blockMap[x + z * 32] = handler.GenerateMapPillar(position, blocks, x, z);
                }
            }

            int index = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        if (blocks[index] != null)
                        {
                            blocks[index].occlusion = GetOcclusion(blockMap, x, y, z);
                        }
                        index++;
                    }
                }
            }

            return blocks;
        });
    }
    
    public static Task<Block[]> CreateMapChunkAsync(Vector3Int position, CWorldDataHandler handler, int lod)
    {
        return Task.Run(() =>
        {
            uint[] blockMap = new uint[1024];
            Block[] blocks = new Block[32768];
            
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    handler.Init(x + position.x, 0, z + position.z);
                    blockMap[x + z * 32] = handler.GenerateMapPillar(position, blocks, x, z);
                }
            }

            Block[] newBlocks = GenerateLodBlocks(blocks, 16, 2, 1);

            int height = lodWidth[1] * lodWidth[1];

            int index = 0;
            for (int y = 0; y < lodWidth[1]; y++)
            {
                for (int z = 0; z < lodWidth[1]; z++)
                {
                    for (int x = 0; x < lodWidth[1]; x++)
                    {
                        if (newBlocks[index] != null)
                        {
                            newBlocks[index].occlusion = 0;
                            
                            byte occlusion = 0;

                            if (z - 1 >= 0 && newBlocks[index - lodWidth[1]] != null)
                                occlusion = 1;
                            
                            if (x + 1 < lodWidth[1] && newBlocks[index + 1] != null)
                                occlusion |= (1 << 1);
                            
                            if (y + 1 < lodWidth[1] && newBlocks[index + height] != null)
                                occlusion |= (1 << 2);
                            
                            if (x - 1 >= 0 && newBlocks[index - 1] != null)
                                occlusion |= (1 << 3);
                            
                            if (y - 1 >= 0 && newBlocks[index - height] != null)
                                occlusion |= (1 << 4);
                            
                            if (z + 1 < lodWidth[1] && newBlocks[index + lodWidth[1]] != null)
                                occlusion |= (1 << 5);

                            newBlocks[index].occlusion = occlusion;
                        }
                        index++;
                    }
                }
            }

            return newBlocks;
        });
    }

    public uint[] GenerateTerrain(Vector3Int position)
    {
        uint[] blockMap = new uint[32 * 32];

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                int height = (int)Mathf.Clamp((GetTerrainHeight(x + position.x, z + position.z, terrainNoise) + 1) - position.y, 0, 32);
                if (height < 32)
                    blockMap[index] = (1u << height) - 1;
                else
                    blockMap[index] = ~0u;
                index++;
            }
        }

        return blockMap;
    }
    
    public static uint[] GenerateTerrain(Vector3Int position, string sampleName, CWorldDataHandler handler)
    {
        if (handler.SampleHandler == null)
            throw new Exception("can't find sample");
        
        uint[] blockMap = new uint[32 * 32];

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                float value = handler.SampleNoise(x + position.x, position.y, z + position.z, handler.mainPoolSample);
                
                if (value > -.5f)
                {
                    int height = (int)Mathf.Clamp(
                        (Mathf.Lerp(WorldInfo.worldMinTerrainHeight, WorldInfo.worldMaxTerrainHeight, value) + 1) - position.y, 0, 32);

                    if (height < 32)
                        blockMap[index] = (1u << height) - 1;
                    else
                        blockMap[index] = ~0u;
                }
                else
                    blockMap[index] = 0;
                
                index++;
            }
        }

        return blockMap;
    }

    private static Block[] GenerateBlocks(uint[] blockMap, BiomeSO biome)
    {
        Block[] newBlocks = new Block[32768];
        
        int index = 0;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    newBlocks[index] = biome.GetBlock(blockMap, x, y, z);
                    if (newBlocks[index] != null)
                    {
                        newBlocks[index].occlusion = GetOcclusion(blockMap, x, y, z);
                    }
                    index++;
                }
            }
        }

        return newBlocks;
    }

    public static byte GetOcclusion(uint[] blockMap, int x, int y, int z)
    {
        byte occlusion = 0;

        int pos = x + z * 32;
        
        for (int i = 0; i < 6; i++)
        {
            int index = pos + sideBlockCheck[i, 0];
            int height = y + sideBlockCheck[i, 1];
            
            if (chunkSide[i](x, z, xzSides[i]) || height < 0 || height >= 32)
            {
                //occlusion |= (byte)(1 << i);
            }
            else
            {
                occlusion |= (byte)(((blockMap[index] >> height) & 1u) << i);
            }
        }

        return occlusion;
    }
    

    private static Func<int, int, int, bool>[] chunkSide = new Func<int, int, int, bool>[]
    {
        (x, z, offset) => { int i = z + offset; return i is < 0 or >= 32; },
        (x, z, offset) => { int i = x + offset; return i is < 0 or >= 32; },
        (x, z, offset) => false,
        (x, z, offset) => { int i = x + offset; return i is < 0 or >= 32; },
        (x, z, offset) => false,
        (x, z, offset) => { int i = z + offset; return i is < 0 or >= 32; }
    };

    private static int[] xzSides = new[]
    {
        -1, 1, 0, -1, 0, 1
    };

    public void SetPixels()
    {
        int index = 0;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Color color = new Color(0, 0, 0, 100);
                    texture.SetPixel(x, z, Color.black);
                    index++;
                }
            }
        }
    }

    public static Block[] GenerateLodBlocks(Block[] blocks, int width, int size, int lod)
    {
        Block[] newBlocks = new Block[width * width * width];
        
        int index = 0;
        int mainIndex = 0;

        for (int y = 0; y < width; y++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    newBlocks[index] = lodBlock[0](blocks, mainIndex);
                    
                    index++;
                    mainIndex += size;
                }
                mainIndex += 32 * (size - 1);
            }
            mainIndex += 1024 * (size - 1);
        }

        return newBlocks;
    }

    public static Func<Block[], int, Block>[] lodBlock = new Func<Block[], int, Block>[]
    {
        (blocks, index) =>
        {
            int offset = 0;
            int blockAmount = 0;
            short blockIndex = 0;
            int priority = 0;
            
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        if (blocks[index + offset] != null)
                        {
                            blockAmount++;
                            CWorldBlock b = BlockManager.GetBlock(blocks[index + offset].blockData);

                            if (b != null && b.priority > priority)
                            {
                                blockIndex = (short)b.index;
                            }
                        }
                        
                        offset++;
                    }

                    offset += 30;
                }
                
                offset += 960;
            }

            if (blockIndex == 0 || blockAmount < 4)
                return null;
            return new Block(blockIndex, 0);
        },
    };

    /**
     * Generate the terrain mesh using the blockMap
     */
    public static void GenerateMesh(ChunkData chunkData)
    {
        int index = 0;
        
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Block block = chunkData.blocks[index];
                    
                    if (block != null)
                    {
                        for (int side = 0; side < 6; side++)
                        {
                            if (((block.check >> side) & 1) == 0 && ((block.occlusion >> side) & 1) == 0)
                            {
                                for (int tris = 0; tris < 6; tris++)
                                {
                                    chunkData.meshData.tris.Add(VoxelData.trisIndexTable[tris] +
                                                                chunkData.meshData.Count());
                                }

                                block.occlusion |= 1 << 7;
                                int i = index;
                                int loop = firstLoopBase[side](y, z);
                                int height = 1;
                                int width = 1;
                                while (loop > 0)
                                {
                                    i += firstOffsetBase[side];
                                    try
                                    {
                                        if (chunkData.blocks[i] == null)
                                            break;
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("i: " + i + " side: " + side);
                                    }

                                    if (((chunkData.blocks[i].check >> side) & 1) == 1 ||
                                        ((chunkData.blocks[i].occlusion >> side) & 1) == 1 ||
                                        chunkData.blocks[i].blockData != block.blockData)
                                        break;

                                    chunkData.blocks[i].check |= (byte)(1 << side);

                                    height++;
                                    loop--;
                                }

                                i = index;
                                loop = secondLoopBase[side](x, z);
                                
                                bool quit = false;
                                
                                while (loop > 0)
                                {
                                    i += secondOffsetBase[side];
                                    int up = i;
                                    for (int j = 0; j < height; j++)
                                    {
                                        try
                                        {
                                            if (chunkData.blocks[up] == null)
                                            {
                                                quit = true;
                                                break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.Log("i: " + i + " side: " + side);
                                        }

                                        if (((chunkData.blocks[up].check >> side) & 1) == 1 ||
                                            ((chunkData.blocks[up].occlusion >> side) & 1) == 1 ||
                                            chunkData.blocks[up].blockData != block.blockData)
                                        {
                                            quit = true;
                                            break;
                                        }
                                        
                                        up += firstOffsetBase[side];
                                    }
                                    
                                    if (quit) break;
                                    
                                    up = i;
                                    for (int j = 0; j < height; j++) {
                                        chunkData.blocks[up].check |= (byte)(1 << side);
                                        up += firstOffsetBase[side];
                                    }

                                    width++;
                                    loop--;
                                }

                                Vector3[] positions = positionOffset[side](width, height);
                                Vector3 position = new Vector3(x, y, z);

                                int id = BlockManager.GetBlock(block.blockData).GetUVs()[side];
                                
                                chunkData.meshData.uvs.Add(new Vector3(0, 0, id));
                                chunkData.meshData.uvs.Add(new Vector3(0, height, id));
                                chunkData.meshData.uvs.Add(new Vector3(width, height, id));
                                chunkData.meshData.uvs.Add(new Vector3(width, 0, id));
                                
                                chunkData.meshData.verts.Add(position + positions[0]);
                                chunkData.meshData.verts.Add(position + positions[1]);
                                chunkData.meshData.verts.Add(position + positions[2]);
                                chunkData.meshData.verts.Add(position + positions[3]);
                            }
                        }
                    }
                    
                    index++;
                }
            }
        }
    }
    
    public static void GenerateMesh(ChunkData chunkData, Block[] blocks, int w, int size, int lod)
    {
        int index = 0;
        
        for (int y = 0; y < w; y++)
        {
            for (int z = 0; z < w; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    Block block = blocks[index];
                    
                    if (block != null)
                    {
                        for (int side = 0; side < 6; side++)
                        {
                            if (((block.check >> side) & 1) == 0 && ((block.occlusion >> side) & 1) == 0)
                            {
                                for (int tris = 0; tris < 6; tris++)
                                {
                                    chunkData.meshData.tris.Add(VoxelData.trisIndexTable[tris] +
                                                                chunkData.meshData.Count());
                                }

                                block.occlusion |= 1 << 7;
                                int i = index;
                                int loop = firstLoop[1][side](y, z);
                                int height = 1;
                                int width = 1;
                                while (loop > 0)
                                {
                                    i += firstOffset[1][side];
                                    try
                                    {
                                        if (blocks[i] == null)
                                            break;
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("i: " + i + " side: " + side);
                                    }

                                    if (((blocks[i].check >> side) & 1) == 1 ||
                                        ((blocks[i].occlusion >> side) & 1) == 1 ||
                                        blocks[i].blockData != block.blockData)
                                        break;

                                    blocks[i].check |= (byte)(1 << side);

                                    height++;
                                    loop--;
                                }

                                i = index;
                                loop = secondLoop[1][side](x, z);
                                
                                bool quit = false;
                                
                                while (loop > 0)
                                {
                                    i += secondOffset[1][side];
                                    int up = i;
                                    for (int j = 0; j < height; j++)
                                    {
                                        try
                                        {
                                            if (blocks[up] == null)
                                            {
                                                quit = true;
                                                break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.Log("i: " + i + " side: " + side);
                                        }

                                        if (((blocks[up].check >> side) & 1) == 1 ||
                                            ((blocks[up].occlusion >> side) & 1) == 1 ||
                                            blocks[up].blockData != block.blockData)
                                        {
                                            quit = true;
                                            break;
                                        }
                                        
                                        up += firstOffset[1][side];
                                    }
                                    
                                    if (quit) break;
                                    
                                    up = i;
                                    for (int j = 0; j < height; j++) {
                                        blocks[up].check |= (byte)(1 << side);
                                        up += firstOffset[1][side];
                                    }

                                    width++;
                                    loop--;
                                }

                                Vector3[] positions = positionOffset[side](width, height);
                                Vector3 position = new Vector3(x * size, y * size, z * size);

                                int id = BlockManager.GetBlock(block.blockData).GetUVs()[side];
                                
                                chunkData.meshData.uvs.Add(new Vector3(0, 0, id));
                                chunkData.meshData.uvs.Add(new Vector3(0, height, id));
                                chunkData.meshData.uvs.Add(new Vector3(width, height, id));
                                chunkData.meshData.uvs.Add(new Vector3(width, 0, id));
                                
                                chunkData.meshData.verts.Add(position + positions[0] * size);
                                chunkData.meshData.verts.Add(position + positions[1] * size);
                                chunkData.meshData.verts.Add(position + positions[2] * size);
                                chunkData.meshData.verts.Add(position + positions[3] * size);
                            }
                        }
                    }
                    
                    index++;
                }
            }
        }
    }
    public static int[] firstOffsetBase = new int[]
    {
        1024, 1024, 32, 1024, 32, 1024
    };

    public static int[][] firstOffset = new int[][]
    {
        new int[]
        {
            1024, 1024, 32, 1024, 32, 1024
        },
        new int[]
        {
            256, 256, 16, 256, 16, 256
        },
    };
    
    public static int[] secondOffsetBase = new int[]
    {
        1, 32, 1, 32, 1, 32
    };
    
    public static int[][] secondOffset = new int[][]
    {
        new int[]
        {
            1, 32, 1, 32, 1, 32
        },
        new int[]
        {
            1, 16, 1, 16, 1, 16
        },
    };
    
    public static Func<int, int, int>[] firstLoopBase = new Func<int, int, int>[]
    {
        (y, z) => 31 - y,
        (y, z) => 31 - y,
        (y, z) => 31 - z,
        (y, z) => 31 - y,
        (y, z) => 31 - z,
        (y, z) => 31 - y,
    };

    public static Func<int, int, int>[][] firstLoop = new Func<int, int, int>[][]
    {
        new Func<int, int, int>[]
        {
            (y, z) => 31 - y,
            (y, z) => 31 - y,
            (y, z) => 31 - z,
            (y, z) => 31 - y,
            (y, z) => 31 - z,
            (y, z) => 31 - y,
        },
        new Func<int, int, int>[]
        {
            (y, z) => 15 - y,
            (y, z) => 15 - y,
            (y, z) => 15 - z,
            (y, z) => 15 - y,
            (y, z) => 15 - z,
            (y, z) => 15 - y,
        },
    };
    
    public static Func<int, int, int>[] secondLoopBase = new Func<int, int, int>[]
    {
        (x, z) => 31 - x,
        (x, z) => 31 - z,
        (x, z) => 31 - x,
        (x, z) => 31 - z,
        (x, z) => 31 - x,
        (x, z) => 31 - z,
    };
    
    public static Func<int, int, int>[][] secondLoop = new Func<int, int, int>[][]
    {
        new Func<int, int, int>[]
        {
            (x, z) => 31 - x,
            (x, z) => 31 - z,
            (x, z) => 31 - x,
            (x, z) => 31 - z,
            (x, z) => 31 - x,
            (x, z) => 31 - z,
        },
        new Func<int, int, int>[]
        {
            (x, z) => 15 - x,
            (x, z) => 15 - z,
            (x, z) => 15 - x,
            (x, z) => 15 - z,
            (x, z) => 15 - x,
            (x, z) => 15 - z,
        },
    };

    public static Func<int, int, Vector3[]>[] positionOffset = new Func<int, int, Vector3[]>[]
    {
        (width, height) => new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(width, height, 0), new Vector3(width, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, height, 0), new Vector3(1, height, width), new Vector3(1, 0, width), },
        (width, height) => new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, height), new Vector3(width, 1, height), new Vector3(width, 1, 0), },
        (width, height) => new Vector3[] { new Vector3(0, 0, width), new Vector3(0, height, width), new Vector3(0, height, 0), new Vector3(0, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(width, 0, 0), new Vector3(width, 0, height), new Vector3(0, 0, height), new Vector3(0, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(width, 0, 1), new Vector3(width, height, 1), new Vector3(0, height, 1), new Vector3(0, 0, 1), },
    };
    
    /**
     * Add a certain block to the mesh based on it's x, y, z coords and what face to occlude
     */
    public void AddBlockToMesh(ChunkData chunkData, Block block, int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);
        for (int i = 0; i < 6; i++)
        {
            if ((block.occlusion & (1 << i)) == 0)
            {
                for (int tris = 0; tris < 6; tris++)
                {
                    chunkData.meshData.tris.Add(VoxelData.trisIndexTable[tris] + chunkData.meshData.Count());
                }
                
                for (int vert = 0; vert < 4; vert++)
                {
                    chunkData.meshData.verts.Add(VoxelData.vertexTable[VoxelData.vertexIndexTable[i, vert]] + position);
                    float2 uv = VoxelData.uvTable[vert];
                    chunkData.meshData.uvs.Add(new Vector3(uv.x, uv.y, blockManagerSo.GetBlock(block.blockData).GetUVs()[i]));
                }
            }
        }
    }

    
    
    /**
     * Update the chunk if a new chunk is generated next to it
     */
    public SideUpdate UpdateChunkFromSide(SideUpdate sideUpdate)
    {
        
        sideUpdate.mainChunk.meshData = new MeshData();
        sideUpdate.sideChunk.meshData = new MeshData();
        
        GenerateMesh(sideUpdate.mainChunk);
        GenerateMesh(sideUpdate.sideChunk);

        return sideUpdate;
    }
    
    /**
     * Updates the side of two chunks facing each other
     */
    public SideUpdate UpdateSides(SideUpdate sideUpdate)
    {
        return sideUpdate;
    }

    /**
     * Loop trough the blocks on the side of the chunk and checks if a blocks is in the chunk next to it
     */
    public SideUpdate UpdateSideMesh(SideUpdate sideUpdate)
    {
        int mainFace = sideUpdate.mainFace;
        int sideFace = SpacialData.oppositeFace[mainFace];

        Block[] mainBlocks = sideUpdate.mainChunk.blocks;
        Block[] sideBlocks = sideUpdate.sideChunk.blocks;
        
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {

            }
        }

        return sideUpdate;
    }
    
    /**
     * Get the occlusion of each face of a block based on the blocks beside it
     */
    public static bool GetOcclusion(ChunkData chunkData, int index, int x, int y, int z)
    {
        int offset;
        bool hasFace = false;
        
        for (int i = 0; i < 6; i++)
        {
            offset = index + SpacialData.sideChecks[i];
            
            if (occlusionOffset[i](x, y, z))
            {
                if (chunkData.sideChunks[i] != null)
                {
                    if (sideChunkBlockCheck[i](chunkData.sideChunks[i].blocks, offset))
                    {
                        chunkData.blocks[index].occlusion |= (byte)(1 << i);
                    }
                }
                else
                {
                    hasFace = true;
                }
            }
            else
            {
                if (chunkData.blocks[offset] == null)
                {
                    hasFace = true;
                }
                else
                {
                    chunkData.blocks[index].occlusion |= (byte)(1 << i);
                }
            }
        }

        return hasFace;
    }
    
    
    public int TrailingZeros(uint pillar)
    {
        if (pillar == 0) return 32;
        int y = 0;
        while ((pillar >> y & 1u) == 0)
            y++;
        return y;
    }
    
    public int TrailingOnes(uint pillar)
    {
        int y = 0;
        while ((pillar >> y & 1u) == 1)
            y++;
        return y;
    }
    
    public uint HAsMask(uint h)
    {
        if (h >= 32)
        {
            return 0xFFFFFFFF; // Return all bits set to 1 if h >= 32 (since shifting by 32 or more bits is invalid)
        }
        else
        {
            return (1u << (int)h) - 1; // Perform the shift and subtract 1
        }
    }
    
    public void AddChunks()
    {
        foreach (var pos in chunksToAdd)
        {
            MeshData meshData = new MeshData();
            ChunkData chunkData = new ChunkData(pos);

            uint[] slices = new uint[32 * 32];
            blocks = new Block[32 * 32 * 32];
        
            GenerateTerrain(pos);
            GenerateBlocks(blockMap, biome);
            
            chunkData.blocks = blocks;
            
            GenerateMesh(chunkData);

            if (worldScript.AddChunk(pos, chunkData))
            {
                GameObject newChunk = Instantiate(chunkPrefab, pos, Quaternion.identity);
                newChunk.GetComponent<ChunkRenderer>().RenderChunk(meshData);
            }
            
            meshData.Clear();
        }
        
        chunksToAdd.Clear();
    }

    public static Func<int, int, int>[] sideIndex = new Func<int, int, int>[]
    {
        (i, j) => { return i + j * 1024; },
        (i, j) => { return i * 32 + j * 1024 + 31; },
        (i, j) => { return i + j * 32 + 31744; }, //31744
        (i, j) => { return i * 32 + j * 1024; },
        (i, j) => { return i + j * 32; },
        (i, j) => { return i + j * 1024 + 992; },
    };

    public static int[,] sideBlockCheck = new[,]
    {
        { -32, 0 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 32, 0 },
    };
    

    public static Func<int, int, int, bool>[] occlusionOffset = new Func<int, int, int, bool>[]
    {
        (x, y, z) => { return z == 0; },
        (x, y, z) => { return x == 31;},
        (x, y, z) => { return y == 31;},
        (x, y, z) => { return x == 0; },
        (x, y, z) => { return y == 0; },
        (x, y, z) => { return z == 31;},
    };

    public static Func<Block[], int, bool>[] sideChunkBlockCheck = new Func<Block[], int, bool>[]
    {
        (blocks, index) => { return blocks[index + 992] != null; },
        (blocks, index) => { return blocks[index - 31] != null; },
        (blocks, index) => { return blocks[index - 31744] != null; },
        (blocks, index) => { return blocks[index + 31] != null; },
        (blocks, index) => { return blocks[index + 31744] != null; },
        (blocks, index) => { return blocks[index - 992] != null; },
    };
    
    static float GetTerrainHeight(int x, int z, NoiseSettings noise)
    {
        float value = Noise.OctavePerlin((float)x, (float)z, noise);
        value = Noise.Redistribution(value, noise);

        return Mathf.Lerp(WorldInfo.worldMinTerrainHeight, WorldInfo.worldMaxTerrainHeight, value);
    }
}

public struct GreedyQuad
{
    public int x;
    public int y;
    public int w;
    public int h;
}
