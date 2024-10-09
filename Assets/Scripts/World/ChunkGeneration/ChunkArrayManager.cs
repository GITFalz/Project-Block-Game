using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkArrayManager
{
    public static uint[] indexOffset = new uint[]
    {
        0, 1, 32, 1024,
    };

    public static uint GetBlockIndex(uint[] blocks, int x, int y, int z)
    {
        int index = z + x * 32 + y * 32 * 32;
        int max = blocks.Length - 1;

        uint pos = 0;
        int i = 0;
        while (pos < index)
        {
            uint fill = (blocks[i] >> 10) & 3;

            if (fill == 0)
            {
                pos++;
            }
            else
            {
                pos += (((blocks[i] >> 5) & 31) + 1) * indexOffset[fill];
            }

            if (pos >= index)
            {
                return (blocks[i] >> 12) & 0b11111111111111111111;
            }

            Debug.Log(i + " fill: " + fill + " pos: " + pos + " index: " + index);

            if (i == max)
            {
                return 0;
            }

            i++;           
        }
        return 0;
    }

    /**
    //Temp function for testing purposes
    public static uint[] CreateBlockType(Block[] blocks)
    {
        List<uint> blockTypes = new List<uint>();

        foreach(Block block in blocks)
        {
            uint newBlock = 0;

            newBlock |= block.index << 20;
            newBlock |= block.state << 12;
            newBlock |= block.fill << 10;
            newBlock |= (block.length - 1) << 5;

            blockTypes.Add(newBlock);
        }

        return blockTypes.ToArray();
    }
    */
}