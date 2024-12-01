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
    
    public static Dictionary<string, Func<Task<int>>> labels = new Dictionary<string, Func<Task<int>>>()
    {
        { "(", async () => await Increment(1, 0) },
        { "name", async () => await CWorldNodesManager.On_Name(ref name) },
        { ")", async () => await Increment(1, 1) },
    };
    
    public static Dictionary<string, Func<Task<int>>> settings = new Dictionary<string, Func<Task<int>>>()
    {
        { "{", async () => await Increment(1, 0) },
        { "id", async () =>
        {
            if (await CWorldCommandManager.GetNextInt(out int index) == -1)
                return await Console.LogErrorAsync("height must be an integer");
            
            if (BlockManager.Exists(index))
                return await Console.LogErrorAsync("Block already exists");
            
            BlockNode.index = index;
            
            if (!BlockManager.Add(BlockNode))
                return await Console.LogErrorAsync("A problem occurred when trying to add the block");
                
            return 0;
        }},
        { "texture", async () => await CWorldNodesManager.On_BlockSetTextures() },
        { "priority", async () => await CWorldNodesManager.On_BlockSetPriority() },
        { "}", async () => await Increment(0, 1) },
    };
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }

    private static async Task<int> Increment(int i, int result)
    {
        return await CWorldCommandManager.Increment(i, result);
    }
}