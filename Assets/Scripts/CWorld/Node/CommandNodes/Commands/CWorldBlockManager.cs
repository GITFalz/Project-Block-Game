using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CWorldBlockManager
{
    public static string name;
    public static CWorldBlock BlockNode;

    public static void SetBlock(CWorldBlock block)
    {
        BlockNode = block;
    }
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref name) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "id", async (w) =>
        {
            if (await w.GetNextInt(out int index) == -1)
                return await w.Error("height must be an integer");
            
            if (BlockManager.Exists(index))
                return await w.Error("Block already exists");
            
            BlockNode.index = index;
            
            if (!BlockManager.Add(BlockNode))
                return await w.Error("A problem occured when trying to add the block");
                
            return 0;
        } },
        { "texture", (w) => w.On_BlockSetTextures() },
        { "priority", (w) => w.On_BlockSetPriority() },
        { "}", (w) => w.Increment(0, 1) },
    };
}