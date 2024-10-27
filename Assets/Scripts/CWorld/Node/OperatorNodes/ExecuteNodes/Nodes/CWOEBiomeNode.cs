using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWOEBiomeNode : CWAExecuteNode
{
    public CWOISampleNode sample;
    
    public List<CWOISampleNode> Overlays;
    public List<CWOCSequenceNode> SequenceNodes;

    public int min_height;
    public int max_height;

    public CWOEBiomeNode()
    {
        Overlays = new List<CWOISampleNode>();
        SequenceNodes = new List<CWOCSequenceNode>();

        min_height = 0;
        max_height = 320;
    }
    
    public override float GetNoise()
    {
        return sample.GetNoise();
    }

    public override int GetBlock(int x, int y, int z)
    {
        if (y >= min_height && y <= max_height)
        {

        }

        return -1;
    }

    public override Block[] GetBlocks(Vector3Int chunkPosition, Block[] blocks, CWorldHandler handler)
    {
        if (chunkPosition.y > max_height || chunkPosition.y + 31 < min_height)
            return blocks;

        uint[] blockMap = new uint[1024];
        int[] tops = new int[1024];

        int index = 0;
        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                blockMap[index] = 0;
                tops[index] = 0;
                    
                handler.Init(x + chunkPosition.x, 0, z + chunkPosition.z);
                
                foreach (var newSample in Overlays)
                {
                    float noise = newSample.GetNoise();

                    if (noise > -.5f)
                    {
                        int height = (int)Mathf.Clamp(Mathf.Lerp(newSample.min_height, newSample.max_height, noise), newSample.min_height, newSample.max_height);
                        
                        GetPillar(newSample, blockMap, height, chunkPosition.y, index);
                    }
                }

                index++;
            }
        }


        for (int z = 0; z < 32; z++)
        {
            for (int x = 0; x < 32; x++)
            {
                handler.Init(x + chunkPosition.x, 0, z + chunkPosition.z);
                index = x + z * 32;
                int[] distance = ToList(tops[index], blockMap[index]);
                
                //Debug.Log($"[{string.Join(", ", distance.Take(32))}]");
                
                for (int y = 0; y < 32; y++)
                {
                    int i = index + y * 1024;

                    if (distance[y] == 0)
                        blocks[i] = null;
                    else if (distance[y] == 1)
                        blocks[i] = new Block(0, 0);
                    else if (distance[y] == 2 || distance[y] == 3)
                        blocks[i] = new Block(1, 0);
                    else
                        blocks[i] = new Block(2, 0);
                    
                    if (blocks[i] != null)
                        blocks[i].occlusion = Chunk.GetOcclusion(blockMap, x, y, z);

                    /**
                    foreach (var sequence in SequenceNodes)
                    {
                        Block newBlock = sequence.GetBlock(distance[y], 0);
                        blocks[index] = newBlock;

                        if (newBlock != null)
                        {
                            newBlock.occlusion = Chunk.GetOcclusion(blockMap, x, y, z);
                            break;
                        }
                    }
                    */
                }
            }
        }
        
        return blocks;
    }

    public void GetPillar(CWOISampleNode sample, uint[] blockMap, int height, int y, int index)
    {
        if (blockMap[index] != ~0u && sample.min_height < y + 32 && sample.max_height >= y)
        {
            if (!sample.flip)
            {
                if (height >= y && height < y + 32 && sample.min_height <= y)
                    blockMap[index] |= (uint)((1ul << ((height - y) + 1)) - 1);
                else if (height >= y && height < y + 32 && sample.min_height > y)
                    blockMap[index] |= (uint)(((1ul << ((height - sample.min_height) + 1)) - 1) << sample.min_height - y);
                else if (height >= y + 32 && sample.min_height >= y)
                    blockMap[index] |= (uint)(((1ul << ((y + 32 - sample.min_height) + 1)) - 1) << sample.min_height - y);
                else if (height >= y + 32 && sample.min_height <= y)
                    blockMap[index] = ~0u;
            }
            else
            {
                int newHeight = sample.min_height + (sample.max_height - height);
                if (newHeight >= y && newHeight < y + 32 && sample.max_height >= y + 32)
                    blockMap[index] |= (uint)(((1ul << ((y + 32 - newHeight) + 1)) - 1) << newHeight - y);
                else if (newHeight >= y && newHeight < y + 32 && sample.max_height < y + 32)
                    blockMap[index] |= (uint)(((1ul << ((sample.max_height - newHeight) + 1)) - 1) << newHeight - y);
                else if (newHeight <= y && sample.max_height < y + 32)
                    blockMap[index] |= (uint)((1ul << ((sample.max_height - y) + 1)) - 1);
                else if (newHeight <= y && sample.max_height >= y + 32)
                    blockMap[index] = ~0u;
            }
        }
    }

    public int[] ToList(int start, uint pillar)
    {
        int[] list = new int[32];
        int count = start;
        
        for (int i = 0; i < 32; i++)
        {
            if (((pillar >> (31 - i)) & 1u) == 1)
                count++;
            else
                count = 0;

            list[31 - i] = count;
        }

        return list;
    }
}