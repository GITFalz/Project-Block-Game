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
    [FormerlySerializedAs("commandSystem")] public GameCommandSystem gameCommandSystem;

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
    
    public static async Task CreateChunk(ChunkData newChunkData, Vector3Int position, string sampleName, GameCommandSystem gameCommandSystem, CWorldDataHandler handler, BiomeSO biome)
    {
        newChunkData.meshData = new MeshData();

        Block[] blocks = await CreateChunkAsync(position, sampleName, handler, biome);
        newChunkData.SetBlocks(blocks);

        GenerateMesh(newChunkData);
        
        gameCommandSystem.chunks.Enqueue(newChunkData);
    }
    
    public static Task<Block[]> CreateChunkAsync(Vector3Int position, string sampleName, CWorldDataHandler handler, BiomeSO biome)
    {
        return Task.Run(() =>
        {
            uint[] blockMap = GenerateTerrain(position, sampleName, handler);
            return GenerateBlocks(blockMap, biome);
        });
    }
    
    public static async Task CreateBiomeChunk(ChunkData newChunkData, Vector3Int position, string biomeName, CWorldDataHandler handler, GameCommandSystem gameCommandSystem)
    {
        newChunkData.meshData = new MeshData();
        
        Block[] blocks = await CreateBiomeChunkAsync(position, biomeName, handler);
        newChunkData.SetBlocks(blocks);
        
        if (WorldChunks.chunksToUpdate.TryGetValue(position, out var chunkData))
        {
            newChunkData += chunkData;
        }
        
        GenerateOcclusion(newChunkData.blocks, 32, 0);
        
        GenerateMesh(newChunkData);
        WorldChunks.activeChunkData.TryAdd(position, newChunkData);
        gameCommandSystem.chunks.Enqueue(newChunkData);
    }
    
    public static Task<Block[]> CreateBiomeChunkAsync(Vector3Int position, string biomeName, CWorldDataHandler handler)
    {
        return Task.Run(() =>
        {
            Block[] blocks = new Block[32768];
            
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    handler.Init(x + position.x, 0, z + position.z);
                    handler.GenerateBiomePillar(position, blocks, x, position.y, z, biomeName);
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
    
    public static async Task CreateMapChunk(ChunkData newChunkData, Vector3Int position, CWorldDataHandler handler, GameCommandSystem gameCommandSystem, int lod)
    {
        await GenerateMapData(newChunkData, position, handler, lod);
        gameCommandSystem.chunks.Enqueue(newChunkData);
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
            Block[] blocks = await CreateMapChunkAsync(newChunkData, position, handler);
            newChunkData.SetBlocks(blocks);
            if (WorldChunks.chunksToUpdate.TryGetValue(position, out var chunkData))
            {
                newChunkData += chunkData;
            }
            GenerateOcclusion(newChunkData.blocks, 32, 0);
            Debug.Log("Generating mesh");
            GenerateMesh(newChunkData);
            //ChunkDataHandler.WriteChunkData(newChunkData);
        }
        else
        {
            Block[] blocks = await CreateMapChunkAsync(newChunkData, position, handler, lod);
            if (WorldChunks.chunksToUpdate.TryGetValue(position, out var chunkData))
            {
                newChunkData += chunkData;
            }
            Block[] newBlocks = GenerateLodBlocks(blocks, lodWidth[lod], lodSize[lod], lod);
            GenerateOcclusion(newBlocks, lodWidth[lod], lod);
            GenerateMesh(newChunkData, newBlocks, lodWidth[lod], lodSize[lod], lod);
            //ChunkDataHandler.WriteChunkData(newChunkData);
        }
    }
    
    public static Task<Block[]> CreateMapChunkAsync(ChunkData chunkData, Vector3Int position, CWorldDataHandler handler)
    {
        return Task.Run(() =>
        {
            Block[] blocks;
            
            if (ChunkDataHandler.ReadChunkData(position, chunkData))
            {
                blocks = chunkData.blocks;
            }
            else
            {
                blocks = new Block[32768];
                
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        handler.Init(x + position.x, 0, z + position.z);
                        handler.GenerateMapPillar(position, blocks, x, position.y, z);
                    }
                }
            }

            return blocks;
        });
    }
    
    public static Task<Block[]> CreateMapChunkAsync(ChunkData chunkData, Vector3Int position, CWorldDataHandler handler, int lod)
    {
        return Task.Run(() =>
        {
            Block[] blocks;
            
            if (ChunkDataHandler.ReadChunkData(position, chunkData))
            {
                blocks = chunkData.blocks;
            }
            else
            {
                blocks = new Block[32768];
                
                for (int z = 0; z < 32; z++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        handler.Init(x + position.x, 0, z + position.z);
                        handler.GenerateMapPillar(position, blocks, x, position.y, z);
                    }
                }
                
                chunkData.blocks = blocks;
            }

            return blocks;
        });
    }
    
    public static void GenerateOcclusion(Block[] blocks, int width, int lod)
    {
        int index = 0;
        for (int y = 0; y < width; y++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (blocks[index] != null)
                    {
                        byte occlusion = 0;
                            
                        for (int i = 0; i < 6; i++)
                        {
                            if (VoxelData.InBounds(x, y, z, i, width) && blocks[index + VoxelData.IndexOffsetLod[lod, i]] != null)
                                occlusion |= VoxelData.ShiftPosition[i];
                        }
                            
                        blocks[index].occlusion = occlusion;
                    }
                        
                    index++;
                }
            }
        }
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

            if (blockIndex == 0 || blockAmount < 1)
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
                        int[] ids;
                        try
                        {
                            ids = BlockManager.GetBlock(block.blockData).GetUVs();
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Log("You might have not loaded the blocks");
                            return;
                        }
                        
                        for (int side = 0; side < 6; side++)
                        {
                            if (((block.check >> side) & 1) == 0 && ((block.occlusion >> side) & 1) == 0)
                            {
                                block.check |= (byte)(1 << side);
                                
                                for (int tris = 0; tris < 6; tris++)
                                {
                                    chunkData.meshData.tris.Add(VoxelData.TrisIndexTable[tris] +
                                                                chunkData.meshData.Count());
                                }
                                
                                int i = index;
                                int loop = VoxelData.FirstLoopBase[side](y, z);
                                int height = 1;
                                int width = 1;
                                while (loop > 0)
                                {
                                    i += VoxelData.FirstOffsetBase[side];
                                    if (chunkData.blocks[i] == null)
                                        break;

                                    if (((chunkData.blocks[i].check >> side) & 1) != 0 ||
                                        ((chunkData.blocks[i].occlusion >> side) & 1) != 0 ||
                                        chunkData.blocks[i].blockData != block.blockData)
                                        break;

                                    chunkData.blocks[i].check |= (byte)(1 << side);

                                    height++;
                                    loop--;
                                }

                                i = index;
                                loop = VoxelData.SecondLoopBase[side](x, z);
                                
                                bool quit = false;
                                
                                while (loop > 0)
                                {
                                    i += VoxelData.SecondOffsetBase[side];
                                    int up = i;
                                    
                                    for (int j = 0; j < height; j++)
                                    {
                                        if (chunkData.blocks[up] == null)
                                        {
                                            quit = true;
                                            break;
                                        }

                                        if (((chunkData.blocks[up].check >> side) & 1) != 0 ||
                                            ((chunkData.blocks[up].occlusion >> side) & 1) != 0 ||
                                            chunkData.blocks[up].blockData != block.blockData)
                                        {
                                            quit = true;
                                            break;
                                        }
                                        
                                        up += VoxelData.FirstOffsetBase[side];
                                    }
                                    
                                    if (quit) break;
                                    
                                    up = i;
                                    
                                    for (int j = 0; j < height; j++) {
                                        chunkData.blocks[up].check |= (byte)(1 << side);
                                        up += VoxelData.FirstOffsetBase[side];
                                    }

                                    width++;
                                    loop--;
                                }

                                Vector3[] positions = VoxelData.PositionOffset[side](width, height);
                                Vector3 position = new Vector3(x, y, z);

                                int id = ids[side];
                                
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
                        int[] ids = BlockManager.GetBlock(block.blockData).GetUVs();
                        
                        for (int side = 0; side < 6; side++)
                        {
                            if (((block.check >> side) & 1) == 0 && ((block.occlusion >> side) & 1) == 0)
                            {
                                block.check |= (byte)(1 << side);
                                
                                for (int tris = 0; tris < 6; tris++)
                                {
                                    chunkData.meshData.tris.Add(VoxelData.TrisIndexTable[tris] +
                                                                chunkData.meshData.Count());
                                }
                                
                                int i = index;
                                int loop = VoxelData.FirstLoop[lod][side](y, z);
                                int height = 1;
                                int width = 1;
                                while (loop > 0)
                                {
                                    i += VoxelData.FirstOffset[lod][side];
                                    if (blocks[i] == null)
                                        break;

                                    if (((blocks[i].check >> side) & 1) != 0 ||
                                        ((blocks[i].occlusion >> side) & 1) != 0 ||
                                        blocks[i].blockData != block.blockData)
                                        break;

                                    blocks[i].check |= (byte)(1 << side);

                                    height++;
                                    loop--;
                                }

                                i = index;
                                loop = VoxelData.SecondLoop[lod][side](x, z);
                                
                                bool quit = false;
                                
                                while (loop > 0)
                                {
                                    i += VoxelData.SecondOffset[lod][side];
                                    int up = i;
                                    
                                    for (int j = 0; j < height; j++)
                                    {
                                        if (blocks[up] == null)
                                        {
                                            quit = true;
                                            break;
                                        }

                                        if (((blocks[up].check >> side) & 1) != 0 ||
                                            ((blocks[up].occlusion >> side) & 1) != 0 ||
                                            blocks[up].blockData != block.blockData)
                                        {
                                            quit = true;
                                            break;
                                        }
                                        
                                        up += VoxelData.FirstOffset[lod][side];
                                    }
                                    
                                    if (quit) break;
                                    
                                    up = i;
                                    
                                    for (int j = 0; j < height; j++) {
                                        blocks[up].check |= (byte)(1 << side);
                                        up += VoxelData.FirstOffset[lod][side];
                                    }

                                    width++;
                                    loop--;
                                }

                                Vector3[] positions = VoxelData.PositionOffset[side](width, height);
                                Vector3 position = new Vector3(x * size, y * size, z * size);

                                int id = ids[side];
                                
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

    public static List<Vector3Int> Bresenham3D(Vector3Int a, Vector3Int b, float r)
    {
        List<Vector3Int> points = new List<Vector3Int>();

        int dx = Math.Abs(b.x - a.x);
        int dy = Math.Abs(b.y - a.y);
        int dz = Math.Abs(b.z - a.z);

        int xs = a.x < b.x ? 1 : -1;
        int ys = a.y < b.y ? 1 : -1;
        int zs = a.z < b.z ? 1 : -1;

        // Driving axis is X-axis
        if (dx >= dy && dx >= dz)
        {
            int p1 = 2 * dy - dx;
            int p2 = 2 * dz - dx;

            while (a.x != b.x)
            {
                a.x += xs;
                if (p1 >= 0)
                {
                    a.y += ys;
                    p1 -= 2 * dx;
                }
                if (p2 >= 0)
                {
                    a.z += zs;
                    p2 -= 2 * dx;
                }
                p1 += 2 * dy;
                p2 += 2 * dz;
                AddThickPoints(points, new Vector3Int(a.x, a.y, a.z), r);
            }
        }
        // Driving axis is Y-axis
        else if (dy >= dx && dy >= dz)
        {
            int p1 = 2 * dx - dy;
            int p2 = 2 * dz - dy;

            while (a.y != b.y)
            {
                a.y += ys;
                if (p1 >= 0)
                {
                    a.x += xs;
                    p1 -= 2 * dy;
                }
                if (p2 >= 0)
                {
                    a.z += zs;
                    p2 -= 2 * dy;
                }
                p1 += 2 * dx;
                p2 += 2 * dz;
                AddThickPoints(points, new Vector3Int(a.x, a.y, a.z), r);
            }
        }
        // Driving axis is Z-axis
        else
        {
            int p1 = 2 * dy - dz;
            int p2 = 2 * dx - dz;

            while (a.z != b.z)
            {
                a.z += zs;
                if (p1 >= 0)
                {
                    a.y += ys;
                    p1 -= 2 * dz;
                }
                if (p2 >= 0)
                {
                    a.x += xs;
                    p2 -= 2 * dz;
                }
                p1 += 2 * dy;
                p2 += 2 * dx;
                AddThickPoints(points, new Vector3Int(a.x, a.y, a.z), r);
            }
        }

        return points;
    }

    private static void Bresenham3DStep(Vector3Int a, Vector3Int b, float r, List<Vector3Int> points, int d1, int d2, int d3, int s1, int s2, int s3, int axis)
    {
        int p1 = 2 * d2 - d1;
        int p2 = 2 * d3 - d1;

        while (a[axis] != b[axis])
        {
            a[axis] += s1;
            if (p1 >= 0)
            {
                a[(axis + 1) % 3] += s2;
                p1 -= 2 * d1;
            }
            if (p2 >= 0)
            {
                a[(axis + 2) % 3] += s3;
                p2 -= 2 * d1;
            }
            p1 += 2 * d2;
            p2 += 2 * d3;
            AddThickPoints(points, a, r);
        }
    }

    private static void AddThickPoints(List<Vector3Int> points, Vector3Int center, float r)
    {
        int intR = Mathf.CeilToInt(r);
        for (int x = -intR; x <= intR; x++)
        {
            for (int y = -intR; y <= intR; y++)
            {
                for (int z = -intR; z <= intR; z++)
                {
                    Vector3Int point = new Vector3Int(center.x + x, center.y + y, center.z + z);
                    if (Vector3.Distance(center, point) <= r && !points.Contains(point))
                    {
                        points.Add(point);
                    }
                }
            }
        }
    }
    
    public static List<Vector3Int> GenerateStretchedSphere(int sizeX, int sizeY, int sizeZ, Vector3Int offset)
    {
        List<Vector3Int> points = new List<Vector3Int>();
        float radiusX = sizeX / 2f;
        float radiusY = sizeY / 2f;
        float radiusZ = sizeZ / 2f;
        
        for (int x = -sizeX; x <= sizeX; x++)
        {
            for (int y = -sizeY; y <= sizeY; y++)
            {
                for (int z = -sizeZ; z <= sizeZ; z++)
                {
                    float normalizedX = x / radiusX;
                    float normalizedY = y / radiusY;
                    float normalizedZ = z / radiusZ;

                    if (normalizedX * normalizedX + normalizedY * normalizedY + normalizedZ * normalizedZ <= 1)
                    {
                        points.Add(new Vector3Int(x, y, z) + offset);
                    }
                }
            }
        }

        return points;
    }
    
    public static Vector3Int GetChunkPosition(Vector3Int blockPosition)
    {
        int chunkX = Mathf.FloorToInt(blockPosition.x / 32f) * 32;
        int chunkY = Mathf.FloorToInt(blockPosition.y / 32f) * 32;
        int chunkZ = Mathf.FloorToInt(blockPosition.z / 32f) * 32;

        return new Vector3Int(chunkX, chunkY, chunkZ);
    }
    
    public static Vector3Int GetRelativeBlockPosition(Vector3Int chunkPosition, Vector3Int blockPosition)
    {
        return new Vector3Int(blockPosition.x - chunkPosition.x, blockPosition.y - chunkPosition.y, blockPosition.z - chunkPosition.z);
    }

    public static bool BlockInExistingChunk(Vector3Int pos)
    {
        return WorldChunks.activeChunkData.ContainsKey(GetChunkPosition(pos));
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

    public static void UpdateChunk(ChunkData chunkData)
    {
        GenerateOcclusion(chunkData.blocks, 32, 0);
        GenerateMesh(chunkData);
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

    public static int[,] sideBlockCheck = new[,]
    {
        { -32, 0 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 32, 0 },
    };
    
    static float GetTerrainHeight(int x, int z, NoiseSettings noise)
    {
        float value = Noise.OctavePerlin((float)x, (float)z, noise);
        value = Noise.Redistribution(value, noise);

        return Mathf.Lerp(WorldInfo.worldMinTerrainHeight, WorldInfo.worldMaxTerrainHeight, value);
    }
}