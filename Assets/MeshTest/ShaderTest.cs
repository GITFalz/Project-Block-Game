using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour
{

    public bool generate = false;
    public uint[] blockMap;
    public Block[] blocks;

    public BiomeSO biome;
    
    public float uvSide = 0.00520833333f;
    public Texture2D texture;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uv1;
    private List<Vector2> uv2;
    
    private 
    void Start()
    {
        if (generate)
        {
            Mesh mesh = new Mesh();

            blockMap = new uint[32 * 32];

            texture = new Texture2D(32 * 192, 32, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            vertices = new List<Vector3>();
            triangles = new List<int>();
            uv1 = new List<Vector2>();
            uv2 = new List<Vector2>();

            TerrainGeneration(new Vector3Int(0, 0, 0));
            SetPixels();
            GenerateMesh();

            texture.Apply();

            SaveTexture(texture);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.SetUVs(0, uv1);
            mesh.SetUVs(1, uv2);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            GetComponent<Renderer>().material.SetTexture("_BlockTexture", texture);
        }
    }
    
    public void TerrainGeneration(Vector3Int position)
    {

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                float noise = (Noise.Generate((float)x / 50f, (float)z / 50f) + 1) / 3f;
                int height = (int)Mathf.Lerp(5, 27, noise);
                
                Debug.Log(noise + " " + height);
                
                blockMap[index] |= (ushort)((1ul << (height + 1)) - 1);
                index++;
            }
        }
    }

    public void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                for (int t = 0; t < 6; t++)
                {
                    triangles.Add(trisIndex[t] + vertices.Count);
                }
                
                vertices.Add(quadPos[i][0] + quadPos[i][4] * j);
                vertices.Add(quadPos[i][1] + quadPos[i][4] * j);
                vertices.Add(quadPos[i][2] + quadPos[i][4] * j);
                vertices.Add(quadPos[i][3] + quadPos[i][4] * j);

                float offset1 = (float)((float)(j+(float)i*32f) / 192);
                float offset2 = (float)((float)(j+1+(float)i*32f) / 192);
                
                uv1.Add(new Vector2(offset1, .0000000001f));
                uv1.Add(new Vector2(offset1, .9999999999f));
                uv1.Add(new Vector2(offset2, .9999999999f));
                uv1.Add(new Vector2(offset2, .0000000001f));

                uv2.Add(new Vector2(0, 0));
                uv2.Add(new Vector2(0, 1));
                uv2.Add(new Vector2(1, 1));
                uv2.Add(new Vector2(1, 0));
            }
        }
    }

    public void SetPixels()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int z = 0; z < 32; z++)
            {
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        int index1 = pixelIndex[i](x, y, z).x;
                        int index2 = pixelIndex[i](x, y, z).y;
                        int shift = pixelIndex[i](x, y, z).z;
                        
                        float col = ((blockMap[index1] >> shift) & 1);
                        Color newColor = new Color(0, 0, col * 0x00 / 255f, col);
                        texture.SetPixel(index2, y, newColor);
                    }
                }
            }
        }
    }


    public Vector2[] uvPos = new Vector2[4]
    {
        new Vector2(0, 0),    
        new Vector2(0, 1),
        new Vector2(1, 1), 
        new Vector2(1, 0),
    };

    public int[] trisIndex = new int[]
    {
        0, 1, 2, 2, 3, 0,
    };

    public Vector3[][] quadPos = new Vector3[][]
    {
        new Vector3[]{
            new Vector3(0, 0, 0),
            new Vector3(0,32, 0),
            new Vector3(32,32, 0),
            new Vector3(32, 0, 0),
            new Vector3(0, 0, 1),
        },
        new Vector3[]{
            new Vector3(1, 0, 0),
            new Vector3(1,32, 0),
            new Vector3(1,32,32),
            new Vector3(1, 0,32),
            new Vector3(1, 0, 0),
        },
        new Vector3[]{
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 32),
            new Vector3(32, 1, 32),
            new Vector3(32, 1, 0),
            new Vector3(0, 1, 0),
        },
        new Vector3[]{
            new Vector3(0, 0, 32),
            new Vector3(0, 32, 32),
            new Vector3(0, 32, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
        },
        new Vector3[]{
            new Vector3(32, 0, 0),
            new Vector3(32, 0, 32),
            new Vector3(0, 0, 32),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
        },
        new Vector3[]{
            new Vector3(32, 0, 1),
            new Vector3(32, 32, 1),
            new Vector3(0, 32, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
        },
    };

    public Func<int, int, int, Vector3Int>[] pixelIndex = new Func<int, int, int, Vector3Int>[]
    {
        (x, y, z) =>
        {
            return new Vector3Int(x + z * 32, x + z * 32, y);
        },
        (x, y, z) =>
        {
            return new Vector3Int(z + x * 32, x + 1024 + z * 32, y);
        },
        (x, y, z) =>
        {
            return new Vector3Int(x + y * 32, x + 2048 + z * 32, z);
        },
        (x, y, z) =>
        {
            return new Vector3Int(z + x * 32, 31 - x + 3072 + z * 32, y);
        },
        (x, y, z) =>
        {
            return new Vector3Int(x + y * 32, 31 - x + 4096 + z * 32, z);
        },
        (x, y, z) =>
        {
            return new Vector3Int(x + z * 32, 31 - x + 5120 + z * 32, y);
        },
    };
    
    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/RenderOutput";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/R_textureTest.png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
