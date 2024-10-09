using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BlockManager")]
public class BlockManager : ScriptableObject
{
    public List<BlockSO> blocks;

    public BlockSO GetBlock(int index)
    {
        return blocks[index];
    }

    public bool GetAir(out BlockSO airBlock)
    {
        airBlock = null;
        if (blocks.Count > 0)
        {
            airBlock = blocks[blocks.Count - 1];
            return true;
        }
        return false;
    }

    public int[] GetIndices(int index)
    {
        return blocks[index].blockUVs.textureIndices;
    }
}
