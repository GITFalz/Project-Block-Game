using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool
{
    public Dictionary<Vector3Int, GameObject> currentChunks;
    public List<GameObject> emptyChunks;

    public ChunkPool()
    {
        currentChunks = new Dictionary<Vector3Int, GameObject>();
        emptyChunks = new List<GameObject>();
    }
}
