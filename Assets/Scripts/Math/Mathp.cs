﻿using System;
using UnityEngine;

public static class Mathp
{
    public static int PosIndex(Vector3 position)
    {
        //positive relative coordinates
        int relative_x = position.x >= 0 ? (int)position.x & 31 : 31 - ((int)-position.x & 31);
        int relative_y = position.y >= 0 ? (int)position.y & 31 : 31 - ((int)-position.y & 31);
        int relative_z = position.z >= 0 ? (int)position.z & 31 : 31 - ((int)-position.z & 31);

        return relative_x + relative_z * 32 + relative_y * 1024;
    }
    
    public static Vector3Int ChunkPos(Vector3 position)
    {
        //chunk origin coordinates
        int chunk_x = position.x >= 0 ? (int)position.x & ~31 : -((int)-position.x & ~31) - 32;
        int chunk_y = position.y >= 0 ? (int)position.y & ~31 : -((int)-position.y & ~31) - 32;
        int chunk_z = position.z >= 0 ? (int)position.z & ~31 : -((int)-position.z & ~31) - 32;

        return new Vector3Int(chunk_x, chunk_y, chunk_z);
    }
}