using System;
using UnityEngine;

public class VisualEditorDisplay : MonoBehaviour
{
    MeshFilter meshFilter;
    
    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void RenderMesh(MeshData meshData)
    {
        if (meshFilter == null)
        {
            Awake();
            return;
        }
        
        Mesh mesh = new Mesh();
        
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();
        mesh.SetUVs(0, meshData.uvs);
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.mesh = mesh;
    }
}