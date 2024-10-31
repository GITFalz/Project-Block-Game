using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class BlockManager : MonoBehaviour
{
    public static Dictionary<int, CWorldBlock> Blocks;
    public List<CWorldBlock> inspectorBlocks;

    public static void Init()
    {
        Blocks = new Dictionary<int, CWorldBlock>();
    }

    public static CWorldBlock GetBlock(int index)
    {
        return Blocks.GetValueOrDefault(index);
    }

    public static bool Exists(CWorldBlock block)
    {
        return Blocks.ContainsKey(block.index);
    }
    
    public static bool Exists(int index)
    {
        return Blocks.ContainsKey(index);
    }

    public static bool Add(CWorldBlock block)
    {
        if (Exists(block))
            Blocks.Remove(block.index);
            
        return Blocks.TryAdd(block.index, block);
    }
    
    public static bool Add(int index)
    {
        return Blocks.TryAdd(index, new CWorldBlock(index));
    }

    public static bool SetUv(int index, int uv, int value)
    {
        if (uv >= 6 || !Blocks.TryGetValue(index, out var block))
            return false;
        
        block.SetUv(uv, value);
        return true;
    }

    public static bool SetPriority(int index, int priority)
    {
        if (!Blocks.TryGetValue(index, out var block))
            return false;

        block.priority = priority;
        return true;
    }

    public void UpdateInspector()
    {
        if (inspectorBlocks == null)
            inspectorBlocks = new List<CWorldBlock>();
        
        inspectorBlocks.Clear();
        
        foreach (var block in Blocks)
        {
            inspectorBlocks.Add(block.Value);
        }
    }
}

[System.Serializable]
public class CWorldBlock
{
    public string blockName;
    public int index;
    public int priority;
    public UVmaps blockUVs = UVmaps.DefaultIndexUVmap;

    public CWorldBlock()
    {
        blockName = "";
        index = 0;
    }
    public CWorldBlock(int index)
    {
        blockName = "";
        this.index = index;
    }

    public void SetUv(int index, int value)
    {
        blockUVs.textureIndices[index] = value;
    }

    public int[] GetUVs()
    {
        return blockUVs.textureIndices;
    }
}