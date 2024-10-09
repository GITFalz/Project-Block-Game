using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Mathematics;
using UnityEngine;

public class BlockData
{
    public int3 position;
    public BlockSO block;

    public BlockData(int3 position)
    {
        this.position = position;
    }

    public int[] GetBlockUVs()
    {
        return block.GetUVs();
    }
}
