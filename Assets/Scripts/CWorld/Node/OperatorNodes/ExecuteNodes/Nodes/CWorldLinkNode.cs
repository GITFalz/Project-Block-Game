using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWorldLinkNode
{
    public string name;

    public LinkPoint A;
    public LinkPoint B;

    public Vector3Int posA = Vector3Int.zero;
    public Vector3Int posB = Vector3Int.zero;
    
    public float radius = 1;
    public float threshold = 0;
    
    public List<CWorldLinkNode> spikes = new List<CWorldLinkNode>();
    
    public CWorldLinkNode(string name)
    {
        this.name = name;
        
        A = new LinkPoint();
        B = new LinkPoint();
    }
    
    public void GetPositions(out Vector3Int a, out Vector3Int b)
    {
        a = posA;
        b = posB;
    }

    public void SetPositions(Vector3Int offset, int height)
    {
        posA = offset;
        posB = offset + new Vector3Int(0, height, 0);
    }
    
    public void SetPositions(Vector3 a, Vector3 b)
    {
        posA = Vector3Int.RoundToInt(a);
        posB = Vector3Int.RoundToInt(b);
    }

    public void GenerateLink(float r)
    {
        var points = Chunk.Bresenham3D(posA, posB, r);
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
    }
    
    public List<Vector3Int> GenerateLinkVE(float r)
    {
        return Chunk.Bresenham3D(posA, posB, r);
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
    public IPoint X = new PointBasic { point = 0 };
    public IPoint Y = new PointBasic { point = 0 };
    public IPoint Z = new PointBasic { point = 0 };
    
    public Vector3Int GetPosition(int x, int y, int z)
    {
        return new Vector3Int(X.Get(x, y, z), Y.Get(y, z, x), Z.Get(z, x ,y));
    }
}

public interface IPoint
{
    int Get(int x, int y, int z);
}

public class PointRange : IPoint
{
    public IntRangeNode range;
    
    public int Get(int x, int y, int z)
    {
        float X = (float)((float)x + (float)y + 0.001f);
        float Y = (float)((float)z + (float)y + 0.001f);
        
        return (int)Mathf.Lerp(range.min, range.max, Mathf.PerlinNoise(X / 4, Y / 4));
    }
}

public class PointBasic : IPoint
{
    public int point;
    
    public int Get(int x, int y, int z)
    {
        return point;
    }
}