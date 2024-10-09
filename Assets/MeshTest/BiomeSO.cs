using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Biome")]
public class BiomeSO : ScriptableObject
{
    public BlockSequence[] blockSequences;

    public Block GetBlock(uint[] blockMap, int x, int y, int z)
    {
        int index = x + z * 32;
        
        foreach (var blockSequence in blockSequences)
        {
            if (!IsTerrain(blockMap[index], y))
            {
                return null;
            }

            if (IsBlock(blockMap[index], blockSequence, index, y, out short blockIndex))
            {
                return new Block(blockIndex, 0);
            }
        }

        return null;
    }

    public static bool IsBlock(uint pillar, BlockSequence Bs, int index, int y, out short blockIndex)
    {
        blockIndex = 0;

        int offset;
        foreach (var Sq in Bs.sequence)
        {
            offset = y + Sq.offset;

            if (offset < 32)
            {
                if (IsTerrain(pillar, offset) != Sq.isTerrain)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        
        blockIndex = Bs.block.index;
        return true;
    }

    public static bool IsTerrain(uint pillar, int y)
    {
        return (pillar & (1u << y)) != 0;
    }
}
