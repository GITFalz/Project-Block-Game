using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkInfo
{
    public static byte chunkWidth = 32;
    public static int check_offset = 256;
    
    public static int GetBlockCount(ChunkData chunkData)
    {
        int nb = 0;
        foreach (Block block in chunkData.blocks)
        {
            if (block != null)
                nb++;
        }

        return nb;
    }

    public static string GetSideChunks(ChunkData[] sideChunks)
    {
        string value = "";
        for (int i = 0; i < 6; i++)
        {
            if (sideChunks[i] == null)
                value += $"Chunk: {i} is null\n";
            else
                value += $"Chunk: {i} is not null, position: {sideChunks[i].position}\n";
        }
        return value;
    }
}
