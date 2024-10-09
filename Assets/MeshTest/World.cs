using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Dictionary<Vector3Int, ChunkData> chunks;

    public void Init()
    {
        chunks = new Dictionary<Vector3Int, ChunkData>();
    }

    public ChunkData GetChunk(Vector3Int chunkPos)
    {
        return chunks.TryGetValue(chunkPos, out ChunkData chunk) ? chunk : null;
    }

    public bool AddChunk(Vector3Int chunkPos, ChunkData chunk)
    {
        return chunks.TryAdd(chunkPos, chunk);
    }
}