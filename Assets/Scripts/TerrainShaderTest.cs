using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainShaderTest : MonoBehaviour
{
    private void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 1),
            new Vector3(0,32, 1),
            new Vector3(32,32, 1),
            new Vector3(32, 0, 1),
        };

        int[] triangles = new int[]
        {
            0, 1, 2, 2, 3, 0
        };

        Vector3[] uvs = new Vector3[]
        {
            new Vector3( 0 , 0 , 0),
            new Vector3( 0 , 1 , 0),
            new Vector3(.5f, 1 , 0),
            new Vector3(.5f, 0 , 0),
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.SetUVs(0, uvs);

        MeshFilter meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = mesh;
    }
}
