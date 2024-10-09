using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    public Camera cam;
    private Plane[] frustumPlanes;

    public void Cull(WorldData worldData)
    {
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach(ChunkRenderer chunks in worldData.activeChunks.Values)
        {
            MeshFilter meshFilter = chunks.gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null )
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;
                if ( vertices != null )
                {
                    Bounds bounds = CalculateBoundingBox(vertices, chunks.transform);

                    bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);
                    chunks.gameObject.SetActive(isVisible);
                }               
            }
        }

        Array.Clear(frustumPlanes, 0, frustumPlanes.Length);   
    }

    Bounds CalculateBoundingBox(Vector3[] vertices, Transform transform)
    {
        Bounds bounds = new Bounds(transform.TransformPoint(vertices[0]), Vector3.zero);
        foreach (Vector3 vertex in vertices)
        {
            bounds.Encapsulate(transform.TransformPoint(vertex));
        }

        return bounds;
    }
}
