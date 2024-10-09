using System;
using System.Linq;
using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    MeshFilter meshFilter;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void RenderChunk(ChunkData chunkData)
    {
        RenderChunk(chunkData.meshData);
    }

    public void RenderChunk(MeshData meshData)
    {
        if (meshData?.verts.Count == 0 || meshData == null)
            return;
        
        try
        {
            ClearMesh();
            
            mesh = new Mesh();
            
            mesh.vertices = meshData.verts.ToArray();
            mesh.triangles = meshData.tris.ToArray();
            mesh.SetUVs(0, meshData.uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            meshFilter.mesh = mesh;
        }
        catch (Exception ex)
        {
            Debug.LogError($"{ex.Message} mesh vertices: {mesh?.vertices.Length} / {meshData.verts.Count}");
        }
    }

    public void ClearMesh()
    {
        mesh.Clear();
    }
}
