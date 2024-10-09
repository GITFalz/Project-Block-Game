using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class VoxelData
{
    public static readonly Vector3[] vertexTable = new Vector3[8]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f),
    };

    public static readonly int[,] vertexIndexTable = new int[,]
    {
        { 0, 3, 2, 1 },
        { 1, 2, 6, 5 },
        { 2, 3, 7, 6 },
        { 4, 7, 3, 0 },
        { 5, 4, 0, 1 },
        { 5, 6, 7, 4 },        
    };

    public static readonly int[] trisIndexTable = new int[]
    {
        0, 1, 2, 2, 3, 0
    };
    
    

    public static readonly Func<GreedyQuad, float3[]>[] greedyOffsetTable = new Func<GreedyQuad, float3[]>[6]
    {
        quad => new float3[]
        {
            new float3(quad.x, quad.y, 0),
            new float3(0, quad.j, 0),
            new float3(quad.i, quad.j, 0),
            new float3(quad.i, 0, 0),
        },

        quad => new float3[]
        {
            new float3(1, quad.y, quad.x),
            new float3(0, quad.j, 0),
            new float3(0, quad.j, quad.i),
            new float3(0, 0, quad.i),
        },

        quad => new float3[]
        {
            new float3(quad.x, 1, quad.y),
            new float3(0, 0, quad.j),
            new float3(quad.i, 0, quad.j),
            new float3(quad.i, 0, 0),
        },

        quad => new float3[]
        {
            new float3(0, quad.y, quad.x + quad.i),
            new float3(0, quad.j, 0),
            new float3(0, quad.j, -quad.i),
            new float3(0, 0, -quad.i),
        },

        quad => new float3[]
        {
            new float3(quad.x, 0, quad.y),
            new float3(quad.i, 0, 0),
            new float3(quad.i, quad.j, 0),
            new float3(0, 0, quad.j),
        },

        quad => new float3[]
        {
            new float3(quad.x + quad.i, quad.y, 1),
            new float3(0, quad.j, 0),
            new float3(-quad.i, quad.j, 0),
            new float3(-quad.i, 0, 0),
        },
    };

    public static readonly Func<GreedyQuad, float3[]>[] greedyOffsetTable1 = new Func<GreedyQuad, float3[]>[6]
    {
        quad => new float3[]
        {
            new float3(quad.x, quad.y, 0),
            new float3(0, 1, 0),
            new float3(1, 0, 0),
        },

        quad => new float3[]
        {
            new float3(1, quad.y, quad.x),
            new float3(0, 1, 0),
            new float3(0, 0, 1),                    
        },

        quad => new float3[]
        {
            new float3(quad.x, 1, quad.y),
            new float3(0, 0, 1),
            new float3(1, 0, 0),
        },

        quad => new float3[]
        {
            new float3(0, quad.y, quad.x + quad.i),
            new float3(0, 1, 0),        
            new float3(0, 0, -1),                      
        },

        quad => new float3[]
        {
            new float3(quad.x, 0, quad.y),
            new float3(1, 0, 0),
            new float3(0, 0, 1),           
        },

        quad => new float3[]
        {
            new float3(quad.x + quad.i, quad.y, 1),
            new float3(0, 1, 0),
            new float3(-1, 0, 0),                   
        },
    };

    public static readonly float2[] uvTable = new float2[]
    {
        new float2( 0, 0),
        new float2( 0, 1),
        new float2( 1, 1),
        new float2( 1, 0),
    }; 

    public static readonly int3[] SurroundingVectors = new int3[6]
    {
        new int3( 0, 0,-1 ),
        new int3( 1, 0, 0 ),
        new int3( 0, 1, 0 ),
        new int3(-1, 0, 0 ),
        new int3( 0,-1, 0 ),
        new int3( 0, 0, 1 ),
    };

    public static readonly int3[,] greedyMeshScaleTable = new int3[6, 2]
    {
        { new int3( 1, 1, 0), new int3(0, 0, 0) },
        { new int3( 1, 1, 0), new int3(0, 0, 0) },
        { new int3( 1, 1, 0), new int3(0, 0, 0) },
        { new int3( 1, 1, 0), new int3(0, 0, 0) },
        { new int3( 1, 1, 0), new int3(0, 0, 0) },
        { new int3( 1, 1, 0), new int3(0, 0, 0) }
    };

    public static void GenerateBlock(Vector3 position, ref VertexBuffer v_buffer, ref MeshData meshData, bool[] occlusion, int[] textureIndices)
    {
        for (int i = 0; i < 6; i++)
        {
            if (occlusion[i] == true)
                continue;

            int[] indices = new int[4];

            for (int vert = 0; vert < 4; vert++)
            {
                Vector3 vertexPosition = position + vertexTable[vertexIndexTable[i, vert]];
                if (!v_buffer.TryAdd(vertexPosition, out int newIndex))
                {
                    indices[vert] = newIndex;
                }
                else
                {
                    indices[vert] = v_buffer.Count() - 1;

                    Vector4 uvWithIndex = new Vector4(uvTable[vert].x, uvTable[vert].y, 0, textureIndices[i]);
                    //meshData.AddUvs(uvWithIndex);
                }               
            }

            for (int j = 0; j < 6; j++)
            {
                meshData.tris.Add(indices[trisIndexTable[j]]);
            }

            
        }
    }

    public static void GenerateVoxel(ref MeshData meshData, bool[] occlusion)
    {

    }

    public static void GenerateGreedyQuad(ref MeshData meshData, GreedyQuad quad, float3 height_offset, int direction, BlockSO block)
    {       
        float3[] offsets = greedyOffsetTable1[direction](quad);
        float3 vert = height_offset + offsets[0];

        int width = quad.i;
        int height = quad.j;

        int vIndex = meshData.verts.Count;

        for (int i = 0; i <= width; i++)
        {
            for (int j = 0; j <= height; j++)
            {
                float3 newVert = vert + offsets[1] * j + offsets[2] * i;
                meshData.verts.Add(newVert);
                Vector3 uv = new Vector3(i ,j, block.GetUVs()[direction]);
                meshData.uvs.Add(uv);

                if (i < width && j < height)
                {
                    meshData.tris.Add(vIndex);
                    meshData.tris.Add(vIndex + 1);
                    meshData.tris.Add(vIndex + height + 1);
                    meshData.tris.Add(vIndex + height + 1);
                    meshData.tris.Add(vIndex + 1);                    
                    meshData.tris.Add(vIndex + height + 2);
                }

                vIndex++;
            }
        }

        /**
        //GreedyMesh
        float3[] offsets = greedyOffsetTable[direction](quad);

        float3 vert1 = height + offsets[0];
        float3 vert2 = vert1 + offsets[1];
        float3 vert3 = vert1 + offsets[2];
        float3 vert4 = vert1 + offsets[3];

        meshData.AddTris(meshData.verts.Count);
        meshData.AddTris(meshData.verts.Count + 1);
        meshData.AddTris(meshData.verts.Count + 2);
        meshData.AddTris(meshData.verts.Count + 2);
        meshData.AddTris(meshData.verts.Count + 3);
        meshData.AddTris(meshData.verts.Count);

        meshData.AddVert(vert1); 
        meshData.AddVert(vert2); 
        meshData.AddVert(vert3); 
        meshData.AddVert(vert4);
        */
    }

    /**
    public static bool Occlusion(long[] blockMap, int x, int y, int z, ref bool[] occlusion)
    {

    }
    */
}
