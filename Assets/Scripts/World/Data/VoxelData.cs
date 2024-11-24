using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class VoxelData
{
    public static readonly Vector3[] VertexTable = new Vector3[8]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f),
    };

    public static readonly int[,] VertexIndexTable = new int[,]
    {
        { 0, 3, 2, 1 },
        { 1, 2, 6, 5 },
        { 2, 3, 7, 6 },
        { 4, 7, 3, 0 },
        { 5, 4, 0, 1 },
        { 5, 6, 7, 4 },
    };

    public static readonly int[] TrisIndexTable = new int[]
    {
        0, 1, 2, 2, 3, 0
    };

    public static readonly float2[] UVTable = new float2[]
    {
        new float2(0, 0),
        new float2(0, 1),
        new float2(1, 1),
        new float2(1, 0),
    };
    
    public static readonly int[] IndexOffset = { -32, 1, 1024, -1, -1024, 32 };
    
    public static readonly int[,] IndexOffsetLod =
    {
        { -32, 1, 1024, -1, -1024, 32 },
        { -16, 1, 256, -1, -256, 16 },
    };
    
    public static readonly Func<int, int, int>[] IndexOffsetVE =
    {
        (X, Z) => -X,
        (X, Z) => 1,
        (X, Z) => X * Z,
        (X, Z) => -1,
        (X, Z) => -X * Z,
        (X, Z) => X,
    };
    
    public static readonly byte[] ShiftPosition = { 1, 2, 4, 8, 16, 32 };

    public static bool InBounds(int x, int y, int z, int side, int size)
    {
        return side switch
        {
            0 => z - 1 >= 0,
            1 => x + 1 < size,
            2 => y + 1 < size,
            3 => x - 1 >= 0,
            4 => y - 1 >= 0,
            5 => z + 1 < size,
            _ => false
        };
    }
    
    public static bool InBounds(int x, int y, int z, int side, int X, int Y, int Z)
    {
        return side switch
        {
            0 => z - 1 >= 0,
            1 => x + 1 < X,
            2 => y + 1 < Y,
            3 => x - 1 >= 0,
            4 => y - 1 >= 0,
            5 => z + 1 < Z,
            _ => false
        };
    }
    
    public static bool BlockIsValid(Block a, Block b, int side)
    {
        return
            (a.check & VoxelData.ShiftPosition[side]) == 0 &&
            (a.occlusion & VoxelData.ShiftPosition[side]) == 0 &&
            a.blockData != b.blockData;
    }
    
    public static readonly int[] FirstOffsetBase =
    {
        1024, 1024, 32, 1024, 32, 1024
    };

    public static readonly int[][] FirstOffset = 
    {
        new int[]
        {
            1024, 1024, 32, 1024, 32, 1024
        },
        new int[]
        {
            256, 256, 16, 256, 16, 256
        },
    };
    
    public static readonly Func<int, int, int>[] FirstOffsetVe = 
    {
        (X, Z) => X * Z,
        (X, Z) => X * Z,
        (X, Z) => X,
        (X, Z) => X * Z,
        (X, Z) => X,
        (X, Z) => X * Z,
    };
    
    public static readonly int[] SecondOffsetBase = { 1, 32, 1, 32, 1, 1 };
    
    public static readonly int[][] SecondOffset =
    {
        new int[]
        {
            1, 32, 1, 32, 1, 1
        },
        new int[]
        {
            1, 16, 1, 16, 1, 1
        },
    };
    
    public static readonly Func<int, int>[] SecondOffsetVe = 
    {
        (X) => 1,
        (X) => X,
        (X) => 1,
        (X) => X,
        (X) => 1,
        (X) => 1,
    };
    
    public static readonly Func<int, int, int>[] FirstLoopBase =
    {
        (y, z) => 31 - y,
        (y, z) => 31 - y,
        (y, z) => 31 - z,
        (y, z) => 31 - y,
        (y, z) => 31 - z,
        (y, z) => 31 - y,
    };

    public static readonly Func<int, int, int>[][] FirstLoop =
    {
        new Func<int, int, int>[]
        {
            (y, z) => 31 - y,
            (y, z) => 31 - y,
            (y, z) => 31 - z,
            (y, z) => 31 - y,
            (y, z) => 31 - z,
            (y, z) => 31 - y,
        },
        new Func<int, int, int>[]
        {
            (y, z) => 15 - y,
            (y, z) => 15 - y,
            (y, z) => 15 - z,
            (y, z) => 15 - y,
            (y, z) => 15 - z,
            (y, z) => 15 - y,
        },
    };
    
    public static readonly Func<int, int, int, int, int>[] FirstLoopVe =
    {
        (y, z, Y, Z) => Y - y,
        (y, z, Y, Z) => Y - y,
        (y, z, Y, Z) => Z - z,
        (y, z, Y, Z) => Y - y,
        (y, z, Y, Z) => Z - z,
        (y, z, Y, Z) => Y - y,
    };
    
    public static readonly Func<int, int, int, int>[] Loop1 =
    {
        (a, y, z) => a - y,
        (a, y, z) => a - y,
        (a, y, z) => a - z,
        (a, y, z) => a - y,
        (a, y, z) => a - z,
        (a, y, z) => a - y,
    };
    
    public static readonly Func<int, int, int, int>[] Loop2 = 
    {
        (a, x, z) => a - x,
        (a, x, z) => a - z,
        (a, x, z) => a - x,
        (a, x, z) => a - z,
        (a, x, z) => a - x,
        (a, x, z) => a - x,
    };
    
    public static readonly Func<int, int, int>[] SecondLoopBase = 
    {
        (x, z) => 31 - x,
        (x, z) => 31 - z,
        (x, z) => 31 - x,
        (x, z) => 31 - z,
        (x, z) => 31 - x,
        (x, z) => 31 - x,
    };
    
    public static readonly Func<int, int, int>[][] SecondLoop = 
    {
        new Func<int, int, int>[]
        {
            (x, z) => 31 - x,
            (x, z) => 31 - z,
            (x, z) => 31 - x,
            (x, z) => 31 - z,
            (x, z) => 31 - x,
            (x, z) => 31 - x,
        },
        new Func<int, int, int>[]
        {
            (x, z) => 15 - x,
            (x, z) => 15 - z,
            (x, z) => 15 - x,
            (x, z) => 15 - z,
            (x, z) => 15 - x,
            (x, z) => 15 - x,
        },
    };
    
    public static readonly Func<int, int, int, int, int>[] SecondLoopVe =
    {
        (x, z, X, Z) => X - x,
        (x, z, X, Z) => Z - z,
        (x, z, X, Z) => X - x,
        (x, z, X, Z) => Z - z,
        (x, z, X, Z) => X - x,
        (x, z, X, Z) => X - x,
    };

    public static readonly Func<int, int, Vector3[]>[] PositionOffset =
    {
        (width, height) => new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(width, height, 0), new Vector3(width, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, height, 0), new Vector3(1, height, width), new Vector3(1, 0, width), },
        (width, height) => new Vector3[] { new Vector3(0, 1, 0), new Vector3(0, 1, height), new Vector3(width, 1, height), new Vector3(width, 1, 0), },
        (width, height) => new Vector3[] { new Vector3(0, 0, width), new Vector3(0, height, width), new Vector3(0, height, 0), new Vector3(0, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(width, 0, 0), new Vector3(width, 0, height), new Vector3(0, 0, height), new Vector3(0, 0, 0), },
        (width, height) => new Vector3[] { new Vector3(width, 0, 1), new Vector3(width, height, 1), new Vector3(0, height, 1), new Vector3(0, 0, 1), },
    };

}
