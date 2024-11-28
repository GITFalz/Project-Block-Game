using System;
using UnityEngine;

public class VisualEditorDisplay : MonoBehaviour
{
    MeshFilter meshFilter;

    [Header("Buttons")] 
    public OnButtonHold scaleButton;
    public OnButtonHold xRotationButton;
    public OnButtonHold yRotationButton;
    public OnButtonHold zRotationButton;
    
    [Header("Properties")]
    public float scaleSpeed = 1;
    public float rotationSpeed = 50;
    
    [Header("Manager")]
    public VisualEditorManager visualEditorManager;
    
    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (scaleButton.IsHolding())
        {
            ScaleModel();
            CenterModel(visualEditorManager.modelSize);
        }
        
        if (xRotationButton.IsHolding())
        {
            RotateModel(new Vector3(1, 0, 0));
        }
        
        if (yRotationButton.IsHolding())
        {
            RotateModel(new Vector3(0, 1, 0));
        }
        
        if (zRotationButton.IsHolding())
        {
            RotateModel(new Vector3(0, 0, 1));
        }
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
    
    public void ScaleModel()
    {
        float x = Input.GetAxis("Mouse X");
        
        transform.localScale +=  scaleSpeed * Time.deltaTime * new Vector3(x, x, x);
    }

    public void RotateModel(Vector3 direction)
    {
        Vector3 rotation = transform.eulerAngles;
        float x = Input.GetAxis("Mouse X");
        
        rotation.x += x * rotationSpeed * Time.deltaTime * direction.x;
        rotation.y += x * rotationSpeed * Time.deltaTime * direction.y;
        rotation.z += x * rotationSpeed * Time.deltaTime * direction.z;
        
        transform.eulerAngles = rotation;
        
        CenterModel(visualEditorManager.modelSize);
    }
    
    public void ResetRotation(int i)
    {
        Vector3 rotation = transform.eulerAngles;
        rotation[i] = 0;
        transform.eulerAngles = rotation;
        
        CenterModel(visualEditorManager.modelSize);
    }

    public void CenterModel(Vector3Int size)
    {
        float scale = transform.localScale.x;
        
        float x = size.x / 2f * scale;
        float y = size.y / 2f * scale;
        float z = size.z / 2f * scale;
        
        transform.localPosition = new Vector3(-x, -y, -z);
    }
}