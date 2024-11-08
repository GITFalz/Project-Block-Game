using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CWorldBiomeNode : CWAExecuteNode
{
    public CWorldSampleNode sample;
    public IntRangeNode sampleRange;
    public CWorldModifierNode modifier;
    public string name;
    
    public List<CWOCSequenceNode> SequenceNodes;

    public CWorldBiomeNode(string name)
    {
        SequenceNodes = new List<CWOCSequenceNode>();
        sampleRange = new IntRangeNode(0, 256);
        modifier = null;
        this.name = name;
    }
    
    public uint GetBlockPillar(Vector3Int chunkPosition, Block[] blocks, int x, int z)
    {
        return GetBlockPillar(chunkPosition, blocks, x, z, sample.noiseValue);
    }

    public uint GetBlockPillar(Vector3Int chunkPosition, Block[] blocks, int x, int z, float noise)
    {
        uint pillar = 0;
        int top = 0;

        if (noise > -.5f)
        {
            int height = (int)Mathf.Clamp(Mathf.Lerp(sample.min_height, sample.max_height, noise), sample.min_height, sample.max_height);
                        
            GetPillar(sampleRange, false, ref pillar, ref top, height, chunkPosition.y);
            
            if (modifier != null)
            {
                foreach (var gen in modifier.gen)
                {
                    height = gen.GetHeight(modifier);
                    if (height != -1)
                        GetPillar((gen.range + modifier.GetMaxHeight()), false, ref pillar, ref top,
                            height, chunkPosition.y);
                }
            }
        }

        int index = x + z * 32;
        int[] distance = ToList(top, pillar);
        
        for (int y = 0; y < 32; y++)
        {
            int i = index + y * 1024;
            blocks[i] = null;

            foreach (var sq in SequenceNodes)
            {
                Block block = sq.GetBlock(distance[y]);
                if (block != null)
                {
                    blocks[i] = new Block(block.blockData, block.state);
                    break;
                }
            }
        }

        return pillar;
    }
    
    public static void GetPillar(IntRangeNode range, bool flip, ref uint pillar, ref int tops, int height, int y)
    {
        if (pillar != ~0u && range.min < y + 32 && range.max >= y)
        {
            bool top = false;
            
            if (!flip)
            {
                if (height >= y && height < y + 32 && range.min <= y)
                { pillar |= (uint)((1ul << ((height - y) + 1)) - 1); top = true; }
                else if (height >= y && height < y + 32 && range.min > y)
                { pillar |= (uint)(((1ul << ((height - range.min) + 1)) - 1) << range.min - y); top = true; }
                else if (height >= y + 32 && range.min >= y)
                { pillar |= (uint)(((1ul << ((y + 32 - range.min) + 1)) - 1) << range.min - y); top = true; }
                else if (height >= y + 32 && range.min <= y)
                { pillar = ~0u; top = true; }
                
                if (top)
                    tops = height - (y + 31);
            }
            else
            {
                int newHeight = range.min + (range.max - height);
                if (newHeight >= y && newHeight < y + 32 && range.max >= y + 32)
                { pillar |= (uint)(((1ul << ((y + 32 - newHeight) + 1)) - 1) << newHeight - y); top = true; }
                else if (newHeight >= y && newHeight < y + 32 && range.max < y + 32)
                { pillar |= (uint)(((1ul << ((range.max - newHeight) + 1)) - 1) << newHeight - y); top = true; }
                else if (newHeight <= y && range.max < y + 32)
                { pillar |= (uint)((1ul << ((range.max - y) + 1)) - 1); top = true; }
                else if (newHeight <= y && range.max >= y + 32) 
                { pillar = ~0u; top = true; }
                
                if (top)
                    tops = range.max - newHeight;
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