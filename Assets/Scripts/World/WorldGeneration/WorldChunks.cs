using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public static class WorldChunks
{
    public static ConcurrentDictionary<Vector3Int, ChunkRenderer> activeChunks = new();
    public static ConcurrentDictionary<Vector3Int, ChunkData> activeChunkData = new();
    public static ConcurrentDictionary<Vector3Int, ChunkData> chunksToUpdate = new();
    
    public static HashSet<Vector3Int> existingChunks = new();
    public static HashSet<Vector3Int> chunksToIgnore = new();

    public static void ClearChunks()
    {
        activeChunkData.Clear();
        activeChunks.Clear();
        existingChunks.Clear();
        chunksToIgnore.Clear();
    }

    public static bool Exists(Vector3Int pos)
    {
        return activeChunks.ContainsKey(pos);
    }
    
    public static bool AddChunk(Vector3Int chunkPosition, ChunkData chunkData, ChunkRenderer chunkRenderer)
    {
        return activeChunkData.TryAdd(chunkPosition, chunkData) && activeChunks.TryAdd(chunkPosition, chunkRenderer);
    }

    public static bool RemoveChunk(Vector3Int chunkPosition)
    {
        bool data = activeChunkData.Remove(chunkPosition, out var chunkData);
        bool render = activeChunks.Remove(chunkPosition, out var chunkRenderer);
        
        if (data) chunkData.Clear();

        return data || render;
    }
}