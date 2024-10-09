using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Biome")]
public class BiomeSO : ScriptableObject
{
    public string biomeName;
    public BlockSequence[] blockSequences;

    public Block GetBlock(uint[] blockMap, int x, int y, int z)
    {
        int index = x + z * 32;
        
        if (!IsTerrain(blockMap[index], y))
            return null;

        foreach (var blockSequence in blockSequences)
        {
            if (IsBlock(blockMap[index], blockSequence, y, out short blockIndex))
                return new Block(blockIndex, 0);             
        }

        return null;
    }

    private static bool IsBlock(uint pillar, BlockSequence Bs, int y, out short blockIndex)
    {
        blockIndex = -1;

        int offset;
        foreach (var Sq in Bs.sequence)
        {
            offset = y + Sq.offset;

            if (offset < 32)
            {
                if (IsTerrain(pillar, offset) != Sq.isTerrain)
                    return false;
            }
            else
            {
                blockIndex = 0;
                return true;
            }
        }

        blockIndex = Bs.block.index;
        return true;
    }

    private static bool IsTerrain(uint pillar, int y)
    {
        return (pillar & (1u << y)) != 0;
    }
}
