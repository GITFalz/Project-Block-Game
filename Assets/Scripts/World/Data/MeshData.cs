using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MeshData
{
    public List<Vector3> verts;
    public List<int> tris;
    public List<Vector3> uvs;

    public MeshData()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        uvs = new List<Vector3>();
    }

    public MeshData(MeshData meshData)
    {
        verts = new List<Vector3>();
        verts.AddRange(meshData.verts);
        
        tris = new List<int>();
        tris.AddRange(meshData.tris);
        
        uvs = new List<Vector3>();
        uvs.AddRange(meshData.uvs);
    }

    public int Count()
    {
        return verts.Count;
    }

    public void Clear()
    {
        verts.Clear();
        tris.Clear();
        uvs.Clear();
    }
}