using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    public NoiseSettings terrainNoise;
    public BiomeSO biome;
    public GameObject chunkPrefab;

    private int chunkSize = 32768;
    private uint[] blockMap;
    private Block[] blocks;
    private ChunkData chunkData;

    public ChunkData GenerateChunk(Vector3Int position)
    {
        chunkData = new ChunkData(position);
        chunkData.meshData = new MeshData();
        
        blockMap = GenerateTerrain(position);
        blocks = new Block[chunkSize];
        chunkData.blocks = blocks;
        
        GenerateBlocks();
        GenerateMesh();

        return chunkData;
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
                
                Debug.Log(Binary.Trailing_Ones(blockMap[index]));
                index++;
            }
        }

        return blockMap;
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
                    blocks[index] = biome.GetBlock(blockMap, x, y, z);
                    if (blocks[index] == null)
                        Debug.Log("Position: " + x + " " + y + " " + z + " Block: null ");
                    else 
                        Debug.Log("Position: " + x + " " + y + " " + z + " Block: " + blocks[index].blockData);
                    index++;
                }
            }
        }
    }

    public void GenerateMesh()
    {
        int index = 0;
        for (int y = 0; y < 32; y++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (blocks[index] != null)
                    {
                        if (GetOcclusion(index, out bool[] occlusion, x, y, z))
                        {
                            AddBlockToMesh(blocks[index], occlusion, x, y, z);
                        }
                    }
                    
                    index++;
                }
            }
        }
    }

    public bool GetOcclusion(int index, out bool[] occlusion, int x, int y, int z)
    {
        occlusion = new bool[6] { false, false, false, false, false, false };
        int offset;
        bool hasFace = false;
        for (int i = 0; i < 6; i++)
        {
            offset = index + SpacialData.sideChecks[i];
            int offsetZ = (offset >> 5) & 31;
            int offsetY = (offset >> 10) & 31;
            
            if (occlusionOffset[i](y, z, offset, offsetY, offsetZ))
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

    public static Func<int, int, int, int, int, bool>[] occlusionOffset = new Func<int, int, int, int, int, bool>[]
    {
        (y, z, offset, offsetY, offsetZ) => { return y != offsetY;    },
        (y, z, offset, offsetY, offsetZ) => { return z != offsetZ;    },
        (y, z, offset, offsetY, offsetZ) => { return offset >= 32768; },
        (y, z, offset, offsetY, offsetZ) => { return z != offsetZ;    },
        (y, z, offset, offsetY, offsetZ) => { return offset < 0;      },
        (y, z, offset, offsetY, offsetZ) => { return y != offsetY;    },
    };

    public void AddBlockToMesh(Block block, bool[] occlusion, int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);
        for (int i = 0; i < 6; i++)
        {
            if (!occlusion[i])
            {
                for (int tris = 0; tris < 6; tris++)
                {
                    chunkData.meshData.tris.Add(VoxelData.TrisIndexTable[tris] + chunkData.meshData.Count());
                }
                
                for (int vert = 0; vert < 4; vert++)
                {
                    chunkData.meshData.verts.Add(VoxelData.VertexTable[VoxelData.VertexIndexTable[i, vert]] + position);
                }
            }
        }
    }
    
    static float GetTerrainHeight(int x, int z, NoiseSettings noise)
    {
        float value = Noise.OctavePerlin((float)x, (float)z, noise);
        value = Noise.Redistribution(value, noise);

        return Mathf.Lerp(WorldInfo.worldMinTerrainHeight, WorldInfo.worldMaxTerrainHeight, value);
    }

    /**
    public void GenerateChunk(int3 position)
    {
        blockMap = new ulong[(ChunkInfo.chunkWidth + 2) * (ChunkInfo.chunkWidth + 2)];
        blockIndexes = new HashSet<int>();

        GetBaseBlockMap(position);
        MeshData meshData = GenerateBlock(position);

        GameObject newChunk = Instantiate(chunkPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        newChunk.GetComponent<ChunkRenderer>().RenderChunk(meshData);

        meshData.Clear();
    }

    public void GetBaseBlockMap(int3 position)
    {

        int size = ChunkInfo.chunkWidth + 2;

        int index = 0;
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                int y = (int)Mathf.Min(GetTerrainHeight(x + position.x - 1, z + position.z - 1, terrainNoise) + 3 - position.y, 38);
                ulong block = (1ul << y) - 1;               
                blockMap[index] = block;
                index++;
            }
        }
    }

    public void GetBlockData(int3 position)
    {
        int size = ChunkInfo.chunkWidth;

        for (int x= 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {

                }
            }
        }
    }

    public void GenerateBlockSlices(int3 position)
    {
        int size = ChunkInfo.chunkWidth;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                
            }
        }
    }

    public MeshData GenerateBlock(int3 position)
    {
        Dictionary<int, ulong[]> blockMaps = plainBiome.GetBlockData(blockMap);

        MeshData meshData = new MeshData();

        int vert = 0;

        ulong[] slice;

        for (int i = 0; i < 32; i++)
        {
            slice = blockMaps[0].Skip(32 * 32 * 4 + i * 32).Take(32).ToArray();

            List<GreedyQuad> quads = GetGreedyMeshData(slice, 32);

            foreach (GreedyQuad quad in quads)
            {
                meshData.AddVert(new Vector3(quad.y, i, quad.x));
                meshData.AddVert(new Vector3(quad.y, i, quad.x + quad.i));
                meshData.AddVert(new Vector3(quad.y + quad.j, i, quad.x + quad.i));
                meshData.AddVert(new Vector3(quad.y + quad.j, i, quad.x));               

                meshData.AddTris(vert + 0);
                meshData.AddTris(vert + 1);
                meshData.AddTris(vert + 2);
                meshData.AddTris(vert + 2);
                meshData.AddTris(vert + 3);
                meshData.AddTris(vert + 0);

                vert += 4;
            }
        }

        blockMaps.Clear();

        return meshData;
    }

    public static List<GreedyQuad> GetGreedyMeshData(ulong[] slice, int size)
    {
        List<GreedyQuad> greedyQuads = new List<GreedyQuad>();

        for (int row = 0; row < slice.Length; row++)
        {
            int y = 0;
            while (y < size)
            {
                y += Binary.Trailing_Zeros(slice[row] >> y);
                if (y >= size) continue;

                int h = Binary.Trailing_Ones(slice[row] >> y);
                ulong h_mask = CheckedShl(1ul, h) - 1ul;
                ulong mask = h_mask << y;

                int i = 1;
                while (row + i < size)
                {
                    ulong new_h = (slice[row + i] >> y) & h_mask;
                    if (new_h != h_mask)
                    {
                        break;
                    }

                    slice[row + i] = slice[row + i] & ~mask;
                    i++;
                }

                greedyQuads.Add(new GreedyQuad
                {
                    x = row,
                    y = y,
                    i = i,
                    j = h,
                });

                y += h;
            }
        }

        return greedyQuads;
    }

    private static ulong CheckedShl(ulong n, int shift)
    {
        if (shift >= 64)
            return 0;
        return n << shift;
    }

    



    static bool GetMapValue(ulong[] blockMap, int x, int y, int z)
    {
        int size = ChunkInfo.chunkWidth;

        ulong pillar = blockMap[z + x * (size+2) + 35];

        return ((pillar >> (3 + y)) & 1) == 1;
    }

    static int3[] neighbouringBlocks = new int3[6]
    {
        new int3( 1, 0, 0),
        new int3(-1, 0, 0),
        new int3( 0, 1, 0),
        new int3( 0,-1, 0),
        new int3( 0, 0, 1),
        new int3( 0, 0,-1),
    };

    static int3[] sliceDirection = new int3[6]
    {
        new int3( 0, 1, 1),
        new int3( 0, 1, 1),
        new int3( 1, 0, 1),
        new int3( 1, 0, 1),
        new int3( 1, 1, 0),
        new int3( 1, 1, 0),
    };
    */
}
