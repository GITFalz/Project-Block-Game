using UnityEngine;

public class BlockColliders
{
    public BlockCollider[] colliders;

    public BlockColliders(params BlockCollider[] colliders)
    {
        this.colliders = colliders;
    }
}

public struct BlockCollider
{
    public float x1;
    public float y1;
    public float z1;

    public float x2;
    public float y2;
    public float z2;
    

    public Vector3 x1y1z1;
    public Vector3 x2y1z1;
    public Vector3 x2y2z1;
    public Vector3 x1y2z1;

    public Vector3 x2y1z2;
    public Vector3 x1y1z2;
    public Vector3 x1y2z2;
    public Vector3 x2y2z2;
    
    /**
    public float frontDistance;
    public float backDistance;
    
    public float rightDistance;
    public float leftDistance;
    
    public float topDistance;
    public float bottomDistance;
    */
    

    public BlockCollider(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.z1 = z1;
        
        this.x2 = x2;
        this.y2 = y2;
        this.z2 = z2;
        
        x1y1z1 = new Vector3(x1, y1, z1);
        x2y1z1 = new Vector3(x2, y1, z1);
        x2y2z1 = new Vector3(x2, y2, z1);
        x1y2z1 = new Vector3(x1, y2, z1);

        x2y1z2 = new Vector3(x2, y1, z2);
        x1y1z2 = new Vector3(x1, y1, z2);
        x1y2z2 = new Vector3(x1, y2, z2);
        x2y2z2 = new Vector3(x2, y2, z2);
    }
    
    public BlockCollider(Vector3 a, Vector3 b)
    {
        this = new BlockCollider(a.x, a.y, a.z, b.x, b.y, b.z);
    }
}