using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWorldLinkNode
{
    public string name;

    public LinkPoint A;
    public LinkPoint B;
    public float radius;
    public float threshold = 0;
    
    public List<CWorldLinkNode> spikes = new List<CWorldLinkNode>();
    
    public CWorldLinkNode(string name)
    {
        this.name = name;
        
        A = new LinkPoint();
        B = new LinkPoint();
        
        radius = 1;
    }

    public void GenerateLink(Vector3Int offset)
    {
        Vector3Int pointA = A.GetPosition() + offset;
        Vector3Int pointB = B.GetPosition() + offset;
        
        Debug.Log("positionA: " + pointA + " positionB: " + pointB);

        var points = Chunk.Bresenham3D(pointA, pointB, radius);
        var chunkDataCache = new Dictionary<Vector3Int, ChunkData>();

        foreach (var point in points)
        {
            Vector3Int chunkPosition = Chunk.GetChunkPosition(point);

            if (!chunkDataCache.TryGetValue(chunkPosition, out var chunkData))
            {
                if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out chunkData))
                {
                    chunkData = new ChunkData(chunkPosition);
                    chunkData.blocks ??= new Block[32768];

                    WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
                }

                chunkDataCache[chunkPosition] = chunkData;
            }
            
            if (chunkData == null)
                continue;

            Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
            int index = position.x + position.z * 32 + position.y * 1024;
            chunkData.blocks[index] ??= new Block(4, 0);
        }
        
        foreach (var chunkData in chunkDataCache)
        {
            WorldChunks.chunksToUpdate[chunkData.Key] = chunkData.Value;
        }

        GenerateSpikes(pointA, pointB);
    }
    
    public void GenerateSpikes(Vector3Int a, Vector3Int b)
    {
        foreach (var spike in spikes)
        {
            Vector3Int lerpOffset = new Vector3Int(
                Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, spike.threshold)), 
                Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, spike.threshold)), 
                Mathf.RoundToInt(Mathf.Lerp(a.z, b.z, spike.threshold))
                );
            Debug.Log("Spike: " + a + " " + b + " " + lerpOffset);
            
            spike.GenerateLink(lerpOffset);
        }
    }

    private void GenerateDebug(Vector3Int pos)
    {
        Vector3Int chunkPosition = Chunk.GetChunkPosition(pos);

        if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out var chunkData))
        {
            chunkData = new ChunkData(chunkPosition);
            chunkData.blocks ??= new Block[32768];

            WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
        }
            
            
        if (chunkData == null)
            return;

        Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, pos);
        int index = position.x + position.z * 32 + position.y * 1024;
        chunkData.blocks[index] = new Block(9999, 0);
    }
}

public class LinkPoint
{
    public IPoint x;
    public IPoint y;
    public IPoint z;
    
    public Vector3Int GetPosition()
    {
        return new Vector3Int(x.Get(), y.Get(), z.Get());
    }
}

public interface IPoint
{
    int Get();
}

public class PointRange : IPoint
{
    public IntRangeNode range;
    
    public int Get()
    {
        return (int)NoiseUtils.GetRandomRange(range.min, range.max);
    }
}

public class PointBasic : IPoint
{
    public int point;
    
    public int Get()
    {
        return point;
    }
}