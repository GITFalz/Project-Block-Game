using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionLibrary
{
    public static BlockCollider[] blockColliders = new BlockCollider[]
    {
        //Bottom
        new BlockCollider(0, 0, 0, 0.5f, 0.5f, 0.5f),
        new BlockCollider(0.5f, 0, 0, 1, 0.5f, 0.5f),
        new BlockCollider(0.5f, 0, 0.5f, 1, 0.5f, 1),
        new BlockCollider(0, 0, 0.5f, 0.5f, 0.5f, 1),
        
        new BlockCollider(0, 0, 0, 1, 0.5f, 0.5f),
        new BlockCollider(0.5f, 0, 0, 1, 0.5f, 1),
        new BlockCollider(0.5f, 0, 0.5f, 1, 0.5f, 1),
        new BlockCollider(0, 0, 0.5f, 0.5f, 0.5f, 1),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        //Top
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        //Both
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(),
        
        new BlockCollider(),
        new BlockCollider(),
        new BlockCollider(0, 0, 0, 1, 1, 1),
    };

    public static Vector3[][] subBlockCoords = new Vector3[][]
    {
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), },
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), },
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), },
        new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), },
    };

    public static int[][] collision_indices = new int[][]
    {
        new[] { 0 },
        new[] { 1 },
    };
}
