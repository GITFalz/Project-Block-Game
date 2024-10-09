using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Blocks")]
public class BlockSO : ScriptableObject
{
    public string blockName;
    public short index;
    public UVmaps blockUVs = UVmaps.DefaultIndexUVmap;

    public int[] GetUVs()
    {
        return blockUVs.textureIndices;
    }
}

public enum BlockType
{
    static_block,
    gravity_block,
    animated_block,

}