using System;
using UnityEngine;

public class TerrainEditorDisplay : MonoBehaviour
{
    [Header("Buttons")] 
    public SingleButtonHoldManager scaleButton;
    public TripleButtonHoldManager rotateButton;

    [Header("Parameters")] 
    public int mapSize = 10;
    public int mapDensity = 10;
    
    public float scaleSpeed = 5;
    
    MeshFilter meshFilter;

    private int _mapLength;
    
    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        UpdateValues();
        GenerateTerrain();
    }

    private void Update()
    {
        if (scaleButton.IsHolding())
        {
            ScaleTerrain();
        }
        
        if (rotateButton.IsHoldingInt(out int i))
        {
            RotateTerrain(i);
        }
    }
    
    public void UpdateValues()
    {
        _mapLength = mapDensity * mapSize + 1;
    }

    public void ScaleTerrain()
    {
        float x = Input.GetAxis("Mouse X");
        
        transform.localScale += Time.deltaTime * scaleSpeed * new Vector3(x, x, x);
    }
    
    public void RotateTerrain(int i)
    {
        Vector3 rotation = transform.eulerAngles;
        rotation[i] += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
        transform.eulerAngles = rotation;
    }

    public void CenterTerrain()
    {
        
    }

    public void GenerateTerrain()
    {
        MeshData meshData = new MeshData();
        
        float[] map = TerrainEditorGenerator.GetHeight(mapSize, mapDensity);

        int verts = 0;
        
        for (int z = 0; z < _mapLength - 1; z++)
        {
            for (int x = 0; x < _mapLength - 1; x++)
            {
                meshData.tris.Add(x + (_mapLength - 1) * z + verts);
                meshData.tris.Add(x + (_mapLength - 1) * z + 1 + verts);
                meshData.tris.Add(x + (_mapLength - 1) * z + _mapLength + verts);
                meshData.tris.Add(x + (_mapLength - 1) * z + 1 + verts);
                meshData.tris.Add(x + (_mapLength - 1) * z + _mapLength + 1 + verts);
                meshData.tris.Add(x + (_mapLength - 1) * z + _mapLength + verts);
            }
            
            verts++;
        }
        
        for (int z = 0; z < _mapLength; z++)
        {
            for (int x = 0; x < _mapLength; x++)
            {
                meshData.verts.Add(new Vector3(x, map[x + z * (_mapLength)], z));
            }
        }
        
        RenderMesh(meshData);
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