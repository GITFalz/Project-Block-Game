using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    public ChunkData chunkData;
    public void RenderChunk(MeshData meshData)
    {
        Mesh mesh = new Mesh();
        
        mesh.vertices = meshData.vertices.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.SetUVs(0, meshData.uvs.ToArray());

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        
        meshFilter.mesh = mesh;
        
        meshData.Clear();
    }
}
