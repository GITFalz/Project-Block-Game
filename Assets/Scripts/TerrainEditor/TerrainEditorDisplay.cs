using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditorDisplay : MonoBehaviour
{
    [Header("Node Manager")]
    public CustomUICollectionManager nodeManager;
    
    [Header("Buttons")] 
    public SingleButtonHoldManager scaleButton;
    public SingleButtonHoldManager rotateButton;
    
    [Header("Parameter Managers")]
    public SingleFloatParameterManager scaleParameter;
    public SingleFloatParameterManager rotationParameter;

    [Header("Parameters")] 
    public int mapSize = 10;
    public int mapDensity = 10;
    
    public float scaleSpeed = 5;
    public float rotationSpeed = 200;
    
    [Header("Texture")]
    public Texture2D texture;

    [Header("Utility")] 
    public Transform center;
    
    private MeshFilter _meshFilter;
    private int _mapLength;
    private Vector3 _minScale = new Vector3(0.1f, 0.1f, 0.1f);

    private float _angle;
    
    private CWorldDataHandler _worldDataHandler;
    
    public void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _worldDataHandler = new CWorldDataHandler();

        UpdateValues();
        GenerateTerrain();
        CenterTerrain();
    }

    private void Update()
    {
        HandleScale();
        HandleRotation();
    }
    
    private void UpdateValues()
    {
        _mapLength = mapDensity * mapSize + 1;
        scaleParameter.ChangeValue(transform.localScale.x);
        rotationParameter.ChangeValue(0);
    }

    private void HandleScale()
    {
        if (scaleButton.IsHolding())
        {
            ScaleTerrain();
            CenterTerrain();
        }
        if (scaleButton.IsClicking())
        {
            ResetScale();
            CenterTerrain();
        }
        if (scaleParameter.ValueChanged(out float value))
        {
            transform.localScale = Vector3.Max(_minScale, new Vector3(value, value, value));
            CenterTerrain();
        }
    }
    
    private void HandleRotation()
    {
        if (rotateButton.IsHolding())
        {
            RotateTerrain();
        }
        
        if (rotateButton.IsClicking())
        {
            ResetAxis();
        }
        
        if (rotationParameter.ValueChanged(out float value))
        {
            transform.RotateAround(center.position, Vector3.up, value - _angle);
            _angle = value;
        }
    }

    private void ScaleTerrain()
    {
        float x = Input.GetAxis("Mouse X");
        Vector3 scale = transform.localScale + (Time.deltaTime * scaleSpeed * new Vector3(x, x, x));
        transform.localScale = Vector3.Max(_minScale, scale);
        
        scaleParameter.ChangeValue(transform.localScale.x);
    }
    
    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
    
    private void RotateTerrain()
    {
        float speed = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
        transform.RotateAround(center.position, Vector3.up, speed);
        
        _angle += speed;
        rotationParameter.ChangeValue(_angle);
    }
    
    private void ResetAxis()
    {
        transform.RotateAround(center.position, Vector3.up, -_angle);
    }

    private void CenterTerrain()
    {
        Vector3 direction = (center.position - transform.position).normalized;
        float size = ((mapSize + 1) * transform.localScale.x) / 2;
        
        Vector3 newPosition = center.position - (MathUtils.Sqrt2 * size * direction);
        transform.position = newPosition;
    }

    public void GenerateNoiseTerrain()
    {
        Compiler();
    }

    public async void Compiler()
    {
        _worldDataHandler = new CWorldDataHandler();
        
        ChunkGenerationNodes.localLoad = true;
        ChunkGenerationNodes.localDataHandler = _worldDataHandler;
        
        string content = nodeManager.Show();

        int result = await CWorldCommandManager.LoadContent(content);
        
        if (result == -1)
        {
            Debug.LogError("Error in the compilation");
            return;
        }
        
        Debug.Log("Compilation successful");
        
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        MeshData meshData = new MeshData();

        float[] map = TerrainEditorGenerator.GetHeight(mapSize, mapDensity, _worldDataHandler);
        
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
                meshData.verts.Add(new Vector3((float)x / mapDensity, map[x + z * (_mapLength)], (float)z / mapDensity));
            }
        }
        
        RenderMesh(meshData);
    }
    
    private Color ColorLerp(Color a, Color b, float t)
    {
        return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t);
    }

    private void RenderMesh(MeshData meshData)
    {
        if (_meshFilter == null)
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

        _meshFilter.mesh = mesh;
    }
}