using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct BlockSequence
{
    public Sequence[] sequence;
    public int heightVariation;
    public BlockSO block;

    [System.Serializable]
    public struct Sequence
    {
        public int offset;
        public bool isTerrain;
    }
}
