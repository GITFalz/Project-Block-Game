using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CWorldTreeNode
{
    public string name;
    public ITreeSampler sampler;
    public IntRangeNode range;
    public CWorldLinkNode link;
    
    public CWorldTreeNode(string name)
    {
        this.name = name;
    }

    public Vector3Int GetBasePosition()
    {
        return link.A.GetPosition();
    }

    public void GenerateTree(int x, int z)
    {
        if (sampler == null || range == null || link == null)
            return;
        
        if (sampler.Ignore() == -1)
            return;
        
        int height = sampler.Sample();
        
        link.GenerateLink(new Vector3Int(x, height, z));

        /*
        int treeHeight = (int)NoiseUtils.GetRandomRange(range.min, range.max);

        Vector3Int treeTop = new Vector3Int(x, height + treeHeight - 1, z);
        
        for (int y = 0; y < treeHeight; y++)
        {
            Vector3Int point = new Vector3Int(x, y + height, z);
            Vector3Int chunkPosition = Chunk.GetChunkPosition(point);
            
            if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out var chunkData))
            {
                chunkData = new ChunkData(chunkPosition);
                chunkData.blocks ??= new Block[32768];
                WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
                Debug.Log("New chunk added to update list: " + chunkPosition);
            }

            Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point);
            int index = position.x + position.z * 32 + position.y * 1024;
            chunkData.blocks[index] = new Block(4, 0);
        }
        */

        /*
        List<Vector3Int> points = Chunk.GenerateStretchedSphere(3, 3, 3);

        foreach (var point in points)
        {
            Vector3Int chunkPosition = Chunk.GetChunkPosition(point + treeTop);
            
            if (!WorldChunks.chunksToUpdate.TryGetValue(chunkPosition, out var chunkData))
            {
                chunkData = new ChunkData(chunkPosition);
                chunkData.blocks ??= new Block[32768];
                WorldChunks.chunksToUpdate.TryAdd(chunkPosition, chunkData);
                Debug.Log("New chunk added to update list: " + chunkPosition);
            }

            Vector3Int position = Chunk.GetRelativeBlockPosition(chunkPosition, point + treeTop);
            int index = position.x + position.z * 32 + position.y * 1024;
            if (chunkData.blocks[index] == null)
                chunkData.blocks[index] = new Block(5, 0);
        }
        */
    }
}

public interface ITreeSampler
{
    int Sample();
    int Ignore();
}

public class TreeBasic : ITreeSampler
{
    private int _height;
    public TreeBasic(int height = 0)
    {
        _height = height;
    }
    public int Sample()
    {
        return 0 + _height;
    }
    
    public int Ignore()
    {
        return 0;
    }
}

public class TreeSample : ITreeSampler
{
    public CWorldSampleNode sampleNode = null;
    public IntRangeNode range = new IntRangeNode(0, 256);

    public int Sample()
    {
        if (sampleNode == null || range == null)
            return -1;
        
        return (int)Mathf.Lerp(range.min, range.max, sampleNode.noiseValue);
    }
    
    public int Ignore()
    {
        return 0;
    }
}

public class TreeModifier : ITreeSampler
{
    public CWorldModifierNode modifierNode = null;

    public int Sample()
    {
        if (modifierNode == null || modifierNode.gen.Count == 0)
            return -1;

        return modifierNode.GetMaxHeight() + modifierNode.gen[0].GetHeight(modifierNode);
    }
    
    public int Ignore()
    {
        foreach (var gen in modifierNode.gen)
        {
            if (gen.GetHeight(modifierNode) == -1)
                return -1;
        }

        return 0;
    }
}