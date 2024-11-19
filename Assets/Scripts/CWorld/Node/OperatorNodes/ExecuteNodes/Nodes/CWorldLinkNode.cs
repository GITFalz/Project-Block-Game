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

    public void GenerateLink(Vector3Int offset, int y)
    {
        Vector3Int pointA = A.GetPosition(offset) + offset;
        
        if (pointA.y < y || pointA.y >= y + 32)
            return;
        
        Vector3Int pointB = B.GetPosition(offset) + offset;
        
        GenerateLink(pointA, pointB);
    }

    public void GenerateLink(Vector3Int a, Vector3Int b)
    {
        Debug.Log("positionA: " + a + " positionB: " + b);

        var points = Chunk.Bresenham3D(a, b, radius);
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

        GenerateSpikes(a, b);
    }
    
    public void GenerateSpikes(Vector3Int a, Vector3Int b)
    {
        foreach (var spike in spikes)
        {
            Vector3Int pointA = new Vector3Int(
                Mathf.RoundToInt(Mathf.Lerp(a.x, b.x, spike.threshold)), 
                Mathf.RoundToInt(Mathf.Lerp(a.y, b.y, spike.threshold)), 
                Mathf.RoundToInt(Mathf.Lerp(a.z, b.z, spike.threshold))
                );
            Debug.Log("Spike: " + a + " " + b + " " + pointA);

            Vector3Int pointB = spike.B.GetPosition(pointA) + pointA;
            spike.GenerateLink(pointA, pointB);
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
    
    public Vector3Int GetPosition(Vector3Int p)
    {
        return new Vector3Int(x.Get(p), y.Get(p), z.Get(p));
    }
}

public interface IPoint
{
    int Get();
    int Get(Vector3Int position);
}

public class PointRange : IPoint
{
    public IntRangeNode range;
    
    public int Get()
    {
        return (int)NoiseUtils.GetRandomRange(range.min, range.max);
    }
    
    public int Get(Vector3Int p)
    {
        return (int)NoiseUtils.GetRandomRange(range.min, range.max, p.x, p.y, p.z);
    }
}

public class PointBasic : IPoint
{
    public int point;
    
    public int Get()
    {
        return point;
    }
    
    public int Get(Vector3Int p)
    {
        return point;
    }
}