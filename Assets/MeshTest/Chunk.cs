using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int size = 100;
    public BiomeSO biome;
    public GameObject chunkPrefab;
    public BlockManager blockManager;
    
    public Vector3Int position;

    public List<Vector3Int> chunksToAdd;
    public ChunkSettingsSO chunkSettings;

    public World worldScript;

    private int chunkSize = 32768;
    private uint[] slices;
	private Block[] blocks;
    private MeshData meshData;
	
    
    
    public void Start()
    {
        worldScript.Init();
        
        AddChunks();
    }

    public void TerrainGeneration(Vector3Int position)
    {

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    Vector3 pos = new Vector3((float)x + .001f + position.x, (float)y + .001f + position.y, (float)z + .001f + position.z);
                    float noise = Noise.Get3DNoise(pos.x / 100f, pos.y / 100f, pos.z / 100f);
                    
                    if (chunkSettings.showNoise)
                        Debug.Log(noise);
                    
                    if (noise > chunkSettings.density)
                        slices[index] |= 1u << y;
                }
                index++;
            }
        }
    }

    public void GenerateBlocks()
    {
        int index = 0;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    blocks[index] = biome.GetBlock(slices, x, y, z);
                    index++;
                }
            }
        }
    }

    public void GenerateMesh()
    {
        int index = 0;
        bool[] occlusion;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (blocks[index] != null)
                    {
                        if (GetOcclusion(blocks, index, y, z, out occlusion))
                            AddToMesh(blocks[index], occlusion, x, y, z);
                    }

                    index++;
                }
            }
        }
    }

    public void AddToMesh(Block block, bool[] occlusion, int x, int y, int z)
    {
        Vector3 pos = new Vector3(x, y, z);

        int i = 0;
        foreach (bool occlude in occlusion)
        {
            if (!occlude)
            {
                for (int t = 0; t < 6; t++)
                {
                    meshData.triangles.Add(VoxelData.tris[t] + meshData.Count());
                    
                }

                int uvIndex = 0;

                for (int v = 0; v < 4; v++)
                {
                    uvIndex = blockManager.blocks[block.index].blockUvs.uvIndex[i];
                    
                    meshData.vertices.Add(VoxelData.vertPos[VoxelData.vertIndicies[i][v]] + pos); ;
                    meshData.uvs.Add(new Vector3(VoxelData.uvs[v].x, VoxelData.uvs[v].y, uvIndex));
                }
            }
            i++;
        }
    }

    public bool GetOcclusion(Block[] blocks, int index, int y, int z, out bool[] occlusion)
    {
        occlusion = new bool[6] { false, false, false, false, false, false };
        int offset;
        bool hasFace = false;
        
        for (int i = 0; i < 6; i++)
        {
            offset = index + SpacialData.sideChecks[i];
            int offsetZ = (offset >> 5) & 31;
            int offsetY = (offset >> 10) & 31;
            
            if (occlusionChecks[i](y, z, offset, offsetY, offsetZ))
                hasFace = true;
            
            else
            {
                if (blocks[offset] == null)
                    hasFace = true;
                
                else
                    occlusion[i] = true;
            }
        }
        return hasFace;
    }

    public static Func<int, int, int, int, int, bool>[] occlusionChecks = new Func<int, int, int, int, int, bool>[]
    {
        (y, z, offset, offsetY, offsetZ) => { return offsetY != y; },
        (y, z, offset, offsetY, offsetZ) => { return offsetZ != z; },
        (y, z, offset, offsetY, offsetZ) => { return offset >= 32768; },
        (y, z, offset, offsetY, offsetZ) => { return offsetZ != z; },
        (y, z, offset, offsetY, offsetZ) => { return offset < 0; },
        (y, z, offset, offsetY, offsetZ) => { return offsetY != y; },
    };

    public uint GetSliceValue(uint[] slices, int x, int y, int z)
    {
        uint value = slices[z + x * 32];
        return (value >> y) & 1u;
    }

    public static float GetTerrainHeight(int x, int z, int size)
    {
        float posX = (float)(((float)x) / size);
        float posZ = (float)(((float)z) / size);
        return Mathf.Lerp(WorldData.minWorldHeight, WorldData.maxWorldHeight, Mathf.PerlinNoise((float)posX, (float)posZ)); 
    }

    public void AddChunks()
    {
        foreach (var pos in chunksToAdd)
        {
            meshData = new MeshData();
            ChunkData chunkData = new ChunkData();

            slices = new uint[32 * 32];
            blocks = new Block[32 * 32 * 32];
        
            TerrainGeneration(pos);
            GenerateBlocks();
            GenerateMesh();

            chunkData.blocks = blocks;

            if (worldScript.AddChunk(pos, chunkData))
            {
                GameObject newChunk = Instantiate(chunkPrefab, pos, Quaternion.identity);
                newChunk.GetComponent<ChunkRenderer>().RenderChunk(meshData);
            }
            
            meshData.Clear();
        }
        
        chunksToAdd.Clear();
    }
    /**
    
    public static List<Quad> GreedyMesh(uint[] slices, int size)
    {
        List<Quad> quads = new List<Quad>();
        for (int row = 0; row < slices.Length; row++)
        {
            int y = 0;
            while (y < size)
            {
                y += TrailingZeros(slices[row] >> y);
                if (y >= size)
                    continue;

                int height = TrailingOnes(slices[row] >> y);

                uint height_as_mask = (height < 32) ? ((1u << height) - 1) : 0xFFFFFFFF;
                uint mask = height_as_mask << y;

                int width = 0;
                while (row + width < size)
                {
                    var next_row = (slices[row + width] >> y) & height_as_mask;
                    if (next_row != height_as_mask)
                        break;

                    slices[row + width] = slices[row + width] & ~mask;
                    width++;
                }

                quads.Add(new Quad(row, y, width, height));
            }
        }

        return quads;
    }
    
    public static int TrailingZeros(uint slice)
    {
        if (slice == 0)
            return 32;

        int i = 0;
        while ((slice & 1) == 0)
        {
            i++;
            slice >>= 1;
        }

        return i;
    }

    public static int TrailingOnes(uint slice)
    {
        if (slice == 0)
            return 0;

        int i = 0;
        while ((slice & 1) == 1)
        {
            i++;
            slice >>= 1;
        }

        return i;
    }
    */
}

public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector3> uvs;

    public MeshData()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector3>();
    }

    public int Count()
    {
        return vertices.Count;
    }

    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
    }
}

public struct Quad
{
    public int x;
    public int y;
    public int width;
    public int height;

    public Quad(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y; 
        this.width = width;
        this.height = height;
    }

    public override string ToString()
    {
        return x + " " + y + " " + width + " " + height;
    }
}

public static class VoxelData
{
    public static Vector3[] vertPos = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),
    };

    public static int[][] vertIndicies = new int[][]
    {
        new int[] { 0, 3, 2, 1 },
        new int[] { 1, 2, 6, 5 },
        new int[] { 2, 3, 7, 6 },
        new int[] { 4, 7, 3, 0 },
        new int[] { 4, 0, 1, 5 },
        new int[] { 5, 6, 7, 4 },
    };
    
    public static int[] tris = new int[6] { 0, 1, 2, 2, 3, 0 };

    public static Vector2[] uvs = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
    };
}

public static class SpacialData
{
    public static int[] sideChecks = new int[6] {
        -32, 1, 1024, -1, -1024, 32,
    };
}

public static class WorldData
{
    public static int minWorldHeight = 10;
    public static int maxWorldHeight = 20;
}

[Serializable]
public struct BlockSequence 
{
	public Sequence[] sequence;
    public BlockSO block;
	
    [Serializable]
	public struct Sequence 
    {
		public int offset;
        public bool isTerrain ;
	}
}