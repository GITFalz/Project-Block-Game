using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWorldLinkNode
{
    public string name;
    public List<I_CWorldCondition> Conditions;

    public CWorldLinkPoint A;
    public CWorldLinkPoint B;
    public float radius;
    
    public CWorldLinkNode(string name)
    {
        this.name = name;
        Conditions = new List<I_CWorldCondition>();
        A = new CWorldLinkPoint();
        B = new CWorldLinkPoint();
        radius = 1;
    }

    public void GenerateLine(int x, int y, int z)
    {
        if (Conditions.Any(condition => !condition.IsTrue()))
            return;

        Vector3Int pointA = A.GetPosition();
        Vector3Int pointB = B.GetPosition();

        if ((pointA.x != x || pointA.z != z) && (pointB.x != x || pointB.z != z))
            return;

        if ((pointA.y < y || pointA.y > y + 32) && (pointB.y < y || pointB.y > y + 32) &&
            !((pointA.y < y && pointB.y > y + 32) || (pointB.y < y && pointA.y > y + 32)))
        {
            return;
        }

        var points = Chunk.Bresenham3D(pointA, pointB, radius);

        foreach (var point in points)
        {
            Vector3Int chunkPosition = Chunk.GetChunkPosition(point);

            if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out var chunkData))
            {
                chunkData = new ChunkData(chunkPosition);
                chunkData.blocks ??= new Block[32768];

                WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
            }
            else
            {
                WorldChunks.activeChunkData.TryGetValue(chunkPosition, out chunkData);
            }
            
            if (chunkData == null)
                continue;

            Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
                
            int index = position.x + position.z * 32 + position.y * 1024;
            
            chunkData.blocks[index] = new Block(2, 0);
        }
    }
}

public class CWorldLinkPoint
{
    public Vector3Int position = Vector3Int.zero;
    public I_CWorldLinkPosition height;

    public Vector3Int GetPosition()
    {
        return new Vector3Int(position.x, position.y + height.GetHeight(), position.z);
    }
}

public interface I_CWorldLinkPosition
{
    int GetHeight();
}

public class CWorldLinkSample : I_CWorldLinkPosition
{
    public CWorldSampleNode sample;
    public IntRangeNode sampleRange;

    public int GetHeight()
    {
        if (sample == null || sampleRange == null)
            return 0;

        return (int)Mathf.Lerp(sampleRange.min, sampleRange.max, sample.noiseValue);
    }
}

public class CWorldLinkModifier : I_CWorldLinkPosition
{
    public CWorldModifierNode modifier;
    public int index;

    public int GetHeight()
    {
        if (modifier == null || modifier.gen.Count == 0 || index >= modifier.gen.Count)
            return 0;

        return modifier.gen[index].GetHeight(modifier);
    }
}

public interface I_CWorldCondition
{
    bool IsTrue();
}

public class CWorldConditionSample : I_CWorldCondition
{
    public CWorldSampleNode sample;
    public FloatRangeNode range;

    public bool IsTrue()
    {
        return sample.noiseValue >= range.min && sample.noiseValue <= range.max;
    }
}