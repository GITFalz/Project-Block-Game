using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static Chunk instance;
    
    public NoiseSettings terrainNoise;
    public BiomeSO biome;
    public BlockManager blockManager;
    public GameObject chunkPrefab;
    
    public List<Vector3Int> chunksToAdd;

    private int chunkSize = 32768;
    private uint[] blockMap;
    private Block[] blocks;
    public Texture2D texture;
    public World worldScript;

    public CWorldHandler handler;

    public bool generateSingle;

    private Vector3Int chunkPos;

    private void Start()
    {
        if (generateSingle)
        {
            texture = new Texture2D(20 * 32, 32, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            
            blockMap = GenerateTerrain(new Vector3Int(0, 0, 0));
            blocks = GenerateBlocks();
            
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
        blocks = GenerateBlocks();
        newChunkData.SetBlocks(blocks);
        
        //World.WriteChunkData(chunkData, writer);
        GenerateMesh(newChunkData);

        return newChunkData;
    }
    
    public ChunkData CreateChunk(ChunkData newChunkData, Vector3Int position, string sampleName)
    {
        newChunkData.meshData = new MeshData();
        
        blockMap = GenerateTerrain(position, sampleName);
        blocks = GenerateBlocks();
        newChunkData.SetBlocks(blocks);
        
        //World.WriteChunkData(chunkData, writer);
        GenerateMesh(newChunkData);

        return newChunkData;
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
    
    public uint[] GenerateTerrain(Vector3Int position, string sampleName)
    {
        uint[] blockMap = new uint[32 * 32];

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                float value = handler.GetSampleNoise(x + position.x, z + position.z, sampleName);
                int height = (int)Mathf.Clamp((Mathf.Lerp(WorldInfo.worldMinTerrainHeight, WorldInfo.worldMaxTerrainHeight, value) + 1) - position.y, 0, 32);
                if (height < 32)
                    blockMap[index] = (1u << height) - 1;
                else
                    blockMap[index] = ~0u;
                index++;
            }
        }

        return blockMap;
    }

    private Block[] GenerateBlocks()
    {
        Block[] newBlocks = new Block[chunkSize];
        int index = 0;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    newBlocks[index] = biome.GetBlock(blockMap, x, y, z);
                    if (newBlocks[index] != null) newBlocks[index].occlusion = GetOcclusion(blockMap, x, y, z);
                    index++;
                }
            }
        }

        return newBlocks;
    }

    private byte GetOcclusion(uint[] blockMap, int x, int y, int z)
    {
        byte occlusion = 0;

        int pos = x + z * 32;
        
        for (int i = 0; i < 6; i++)
        {
            int index = pos + sideBlockCheck[i, 0];
            int height = y + sideBlockCheck[i, 1];
            
            if (index < 0 || index >= 1024 || height < 0 || height >= 32)
                occlusion |= (byte)(1 << i);
            else
                occlusion |= (byte)(((blockMap[index] >> height) & 1u) << i);
        }

        return occlusion;
    }

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

    /**
     * Generate the terrain mesh using the blockMap
     */
    public void GenerateMesh(ChunkData chunkData)
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
                        if (block.occlusion < 0b00111111) 
                            AddBlockToMesh(chunkData, block, x, y, z);
                    }
                    
                    index++;
                }
            }
        }
    }
    
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
                    chunkData.meshData.uvs.Add(new Vector3(uv.x, uv.y, blockManager.GetBlock(block.blockData).GetUVs()[i]));
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


    public void GreedyMesh()
    {
        
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
            GenerateBlocks();
            
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

    public int[,] sideBlockCheck = new[,]
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
        (blocks, index) => { return blocks[index + 1024] != null; },
        (blocks, index) => { return blocks[index - 32] != null; },
        (blocks, index) => { return blocks[index - 32768] != null; },
        (blocks, index) => { return blocks[index + 32] != null; },
        (blocks, index) => { return blocks[index + 32768] != null; },
        (blocks, index) => { return blocks[index - 1024] != null; },
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
    public int i;
    public int j;
}
