using System;
using UnityEngine;

public static class BlockCheck
{
    //Bottom Section
 
    /**
     * ..
     * #.
     */
    private static bool B_state_0(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * ..
     * .#
     */
    private static bool B_state_1(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * ..
     */
    private static bool B_state_2(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /** 
     * #.
     * ..
     */
    private static bool B_state_3(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * ..
     * ##
     */
    private static bool B_state_4(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * .#
     */
    private static bool B_state_5(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    /**
     * ##
     * ..
     */
    private static bool B_state_6(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * #.
     * #.
     */
    private static bool B_state_7(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }

    /**
     * ##
     * ##
     */
    private static bool B_state_14(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 0.5f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    //Top section
    
    /**
     * ..
     * #.
     */
    private static bool T_state_0(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * ..
     * .#
     */
    private static bool T_state_1(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * ..
     */
    private static bool T_state_2(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /** 
     * #.
     * ..
     */
    private static bool T_state_3(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * ..
     * ##
     */
    private static bool T_state_4(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * .#
     */
    private static bool T_state_5(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    /**
     * ##
     * ..
     */
    private static bool T_state_6(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * #.
     * #.
     */
    private static bool T_state_7(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }

    /**
     * ##
     * ##
     */
    private static bool T_state_14(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y + 0.5f && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    //2 tall
    
    /**
     * ..
     * #.
     */
    private static bool BT_state_0(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * ..
     * .#
     */
    private static bool BT_state_1(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * ..
     */
    private static bool BT_state_2(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /** 
     * #.
     * ..
     */
    private static bool BT_state_3(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * ..
     * ##
     */
    private static bool BT_state_4(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 0.5f;
    }
    
    /**
     * .#
     * .#
     */
    private static bool BT_state_5(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x + 0.5f && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    /**
     * ##
     * ..
     */
    private static bool BT_state_6(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z + 0.5f && point.z <= block.z + 1f;
    }
    
    /**
     * #.
     * #.
     */
    private static bool BT_state_7(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 0.5f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }

    /**
     * ##
     * ##
     */
    private static bool BT_state_14(Vector3 point, Vector3Int block)
    {
        return point.x >= block.x && point.x <= block.x + 1f && 
               point.y >= block.y && point.y <= block.y + 1f && 
               point.z >= block.z && point.z <= block.z + 1f;
    }
    
    
    public static Func<Vector3, Vector3Int, bool>[] blockCollisionCheck = new Func<Vector3, Vector3Int, bool>[]
    {
        (point, block) => { return B_state_0(point, block); },
        (point, block) => { return B_state_1(point, block); },
        (point, block) => { return B_state_2(point, block); },
        (point, block) => { return B_state_3(point, block); },
        (point, block) => { return B_state_4(point, block); },
        (point, block) => { return B_state_5(point, block); },
        (point, block) => { return B_state_6(point, block); },
        (point, block) => { return B_state_7(point, block); },
        (point, block) => { return B_state_0(point, block) || B_state_2(point, block); },
        (point, block) => { return B_state_1(point, block) || B_state_3(point, block); },
        (point, block) => { return B_state_4(point, block) || B_state_3(point, block); },
        (point, block) => { return B_state_4(point, block) || B_state_2(point, block); },
        (point, block) => { return B_state_6(point, block) || B_state_1(point, block); },
        (point, block) => { return B_state_6(point, block) || B_state_0(point, block); },
        (point, block) => { return B_state_14(point, block); },
        
        (point, block) => { return T_state_0(point, block); },
        (point, block) => { return T_state_1(point, block); },
        (point, block) => { return T_state_2(point, block); },
        (point, block) => { return T_state_3(point, block); },
        (point, block) => { return T_state_4(point, block); },
        (point, block) => { return T_state_5(point, block); },
        (point, block) => { return T_state_6(point, block); },
        (point, block) => { return T_state_7(point, block); },
        (point, block) => { return T_state_0(point, block) || T_state_2(point, block); },
        (point, block) => { return T_state_1(point, block) || T_state_3(point, block); },
        (point, block) => { return T_state_4(point, block) || T_state_3(point, block); },
        (point, block) => { return T_state_4(point, block) || T_state_2(point, block); },
        (point, block) => { return T_state_6(point, block) || T_state_1(point, block); },
        (point, block) => { return T_state_6(point, block) || T_state_0(point, block); },
        (point, block) => { return T_state_14(point, block); },
        
        (point, block) => { return BT_state_0(point, block); },
        (point, block) => { return BT_state_1(point, block); },
        (point, block) => { return BT_state_2(point, block); },
        (point, block) => { return BT_state_3(point, block); },
        (point, block) => { return BT_state_4(point, block); },
        (point, block) => { return BT_state_5(point, block); },
        (point, block) => { return BT_state_6(point, block); },
        (point, block) => { return BT_state_7(point, block); },
        (point, block) => { return BT_state_0(point, block) || BT_state_2(point, block); },
        (point, block) => { return BT_state_1(point, block) || BT_state_3(point, block); },
        (point, block) => { return BT_state_4(point, block) || BT_state_3(point, block); },
        (point, block) => { return BT_state_4(point, block) || BT_state_2(point, block); },
        (point, block) => { return BT_state_6(point, block) || BT_state_1(point, block); },
        (point, block) => { return BT_state_6(point, block) || BT_state_0(point, block); },
        (point, block) => { return BT_state_14(point, block); },
    };

    public static int[][] blockCollisionChecks = new int[][]
    {
     new []{ 0 },//00000000
     new []{ 1 },//00000001
     new []{ 2 },//00000010
     new []{ 3 },//00000011
     new []{ 4 },//00000100
     new []{ 5 },//00000101
     new []{ 6 },//00000110
     new []{ 7 },//00000111
     new []{ 8 },    //00001000
     new []{ 9 },    //00001001
     new []{ 10 },   //00001010
     new []{ 11 },   //00001011
     new []{ 12 },   //00001100
     new []{ 13 },   //00001101
     new []{ 14 },   //00001110
     new []{ -1 },   //00001111 //Impossible
     new []{ 30 },   //00010000
     new []{ 15, 1 },//00000000
     new []{ 15, 2 },//00000000
     new []{ 15, 3 },//00000000
     new []{ 15, 4 },//00000000
     new []{ 15, 5 },//00000000
     new []{ 15, 6 },//00000000
     new []{ 15, 7 },//00000000
     new []{ 30, 2 },//00000000
     new []{ 15, 9 },//00000000
     new []{ 15, 10 },//00000000
     new []{ 30, 5 },//00000000
     new []{ 15, 12 },//00000000
     new []{ 30, 6 },//00000000
     new []{ 15, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 16, 0 },//00000000
     new []{ 31 },//00000000
     new []{ 16, 2 },//00000000
     new []{ 16, 3 },//00000000
     new []{ 16, 4 },//00000000
     new []{ 16, 5 },//00000000
     new []{ 16, 6 },//00000000
     new []{ 16 ,7 },//00000000
     new []{ 16, 8 },//00000000
     new []{ 31, 3 },//00000000
     new []{ 31, 7 },//00000000
     new []{ 16, 11 },//00000000
     new []{ 31, 6 },//00000000
     new []{ 16, 13 },//00000000
     new []{ 16, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 17, 0 },//00000000
     new []{ 17, 1 },//00000000
     new []{ 32 },//00000000
     new []{ 17, 3 },//00000000
     new []{ 17, 4 },//00000000
     new []{ 17, 5 },//00000000
     new []{ 17, 6 },//00000000
     new []{ 17, 7 },//00000000
     new []{ 32, 2 },//00000000
     new []{ 17, 9 },//00000000
     new []{ 17, 10 },//00000000
     new []{ 32, 4 },//00000000
     new []{ 17, 12 },//00000000
     new []{ 32, 7 },//00000000
     new []{ 17, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 18, 0 },//00000000
     new []{ 18, 1 },//00000000
     new []{ 18, 2 },//00000000
     new []{ 33 },//00000000
     new []{ 18, 4 },//00000000
     new []{ 18, 5 },//00000000
     new []{ 18, 6 },//00000000
     new []{ 18, 7 },//00000000
     new []{ 18, 8 },//00000000
     new []{ 33, 1 },//00000000
     new []{ 33, 4 },//00000000
     new []{ 18, 11 },//00000000
     new []{ 33, 5 },//00000000
     new []{ 18, 13 },//00000000
     new []{ 18, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 19, 0 },//00000000
     new []{ 19, 1 },//00000000
     new []{ 19, 2 },//00000000
     new []{ 19, 3 },//00000000
     new []{ 34 },//00000000
     new []{ 19, 5 },//00000000
     new []{ 19, 6 },//00000000
     new []{ 19, 7 },//00000000
     new []{ 19, 8 },//00000000
     new []{ 19, 9 },//00000000
     new []{ 34, 3 },//00000000
     new []{ 34, 2 },//00000000
     new []{ 19, 12 },//00000000
     new []{ 19, 13 },//00000000
     new []{ 19, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 20, 0 },//00000000
     new []{ 20, 1 },//00000000
     new []{ 20, 2 },//00000000
     new []{ 20, 3 },//00000000
     new []{ 20, 4 },//00000000
     new []{ 35 },//00000000
     new []{ 20, 6 },//00000000
     new []{ 20, 7 },//00000000
     new []{ 20, 8 },//00000000
     new []{ 20, 9 },//00000000
     new []{ 20, 10 },//00000000
     new []{ 35, 0 },//00000000
     new []{ 35, 3 },//00000000
     new []{ 20, 13 },//00000000
     new []{ 20, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 21, 0 },//00000000
     new []{ 21, 1 },//00000000
     new []{ 21, 2 },//00000000
     new []{ 21, 3 },//00000000
     new []{ 21, 4 },//00000000
     new []{ 21, 5 },//00000000
     new []{ 36 },//00000000
     new []{ 21, 7 },//00000000
     new []{ 21, 8 },//00000000
     new []{ 21, 9 },//00000000
     new []{ 21, 10 },//00000000
     new []{ 21, 11 },//00000000
     new []{ 36, 1 },//00000000
     new []{ 36, 0 },//00000000
     new []{ 21, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 22, 0 },//00000000
     new []{ 22, 1 },//00000000
     new []{ 22, 2 },//00000000
     new []{ 22, 3 },//00000000
     new []{ 22, 4 },//00000000
     new []{ 22, 5 },//00000000
     new []{ 22, 6 },//00000000
     new []{ 37 },//00000000
     new []{ 22, 8 },//00000000
     new []{ 22, 9 },//00000000
     new []{ 37, 1 },//00000000
     new []{ 22, 11 },//00000000
     new []{ 22, 12 },//00000000
     new []{ 37, 2 },//00000000
     new []{ 22, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 30, 17 },//00000000
     new []{ 23, 1 },//00000000
     new []{ 32, 15 },//00000000
     new []{ 30, 32 },//00000000
     new []{ 23, 4 },//00000000
     new []{ 23, 5 },//00000000
     new []{ 23, 6 },//00000000
     new []{ 23, 7 },//00000000
     new []{ 23, 8 },//00000000
     new []{ 23, 9 },//00000000
     new []{ 23, 10 },//00000000
     new []{ 23, 11 },//00000000
     new []{ 23, 12 },//00000000
     new []{ 23, 13 },//00000000
     new []{ 23, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 24, 0 },//00000000
     new []{ 31, 18 },//00000000
     new []{ 24, 2 },//00000000
     new []{ 33, 16 },//00000000
     new []{ 24, 4 },//00000000
     new []{ 24, 5 },//00000000
     new []{ 24, 6 },//00000000
     new []{ 24, 7 },//00000000
     new []{ 24, 8 },//00000000
     new []{ 31, 33 },//00000000
     new []{ 24, 10 },//00000000
     new []{ 24, 11 },//00000000
     new []{ 24, 12 },//00000000
     new []{ 24, 13 },//00000000
     new []{ 24, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 25, 0 },//00000000
     new []{ 31, 22 },//00000000
     new []{ 25, 2 },//00000000
     new []{ 33, 19 },//00000000
     new []{ 34, 18 },//00000000
     new []{ 25, 5 },//00000000
     new []{ 25, 6 },//00000000
     new []{ 37, 16 },//00000000
     new []{ 25, 8 },//00000000
     new []{ 39, 15 },//00000000
     new []{ 34, 33 },//00000000
     new []{ 25, 11 },//00000000
     new []{ 25, 12 },//00000000
     new []{ 25, 13 },//00000000
     new []{ 25, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 30, 20 },//00000000
     new []{ 26, 1 },//00000000
     new []{ 32, 19 },//00000000
     new []{ 26, 3 },//00000000
     new []{ 34, 17 },//00000000
     new []{ 35, 15 },//00000000
     new []{ 26, 6 },//00000000
     new []{ 26, 7 },//00000000
     new []{ 38, 16 },//00000000
     new []{ 26, 9 },//00000000
     new []{ 26, 10 },//00000000
     new []{ 26, 11 },//00000000
     new []{ 26, 12 },//00000000
     new []{ 26, 13 },//00000000
     new []{ 26, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 27, 0 },//00000000
     new []{ 31, 21 },//00000000
     new []{ 27, 2 },//00000000
     new []{ 33, 20 },//00000000
     new []{ 27, 4 },//00000000
     new []{ 35, 18 },//00000000
     new []{ 36, 16 },//00000000
     new []{ 27, 7 },//00000000
     new []{ 27, 8 },//00000000
     new []{ 39, 17 },//00000000
     new []{ 27, 10 },//00000000
     new []{ 27, 11 },//00000000
     new []{ 27, 12 },//00000000
     new []{ 27, 13 },//00000000
     new []{ 27, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 30, 21 },//00000000
     new []{ 28, 1 },//00000000
     new []{ 32, 22 },//00000000
     new []{ 28, 3 },//00000000
     new []{ 28, 4 },//00000000
     new []{ 28, 5 },//00000000
     new []{ 36, 15 },//00000000
     new []{ 37, 17 },//00000000
     new []{ 38, 18 },//00000000
     new []{ 28, 9 },//00000000
     new []{ 28, 10 },//00000000
     new []{ 28, 11 },//00000000
     new []{ 28, 12 },//00000000
     new []{ 28, 13 },//00000000
     new []{ 28, 14 },//00000000
     new []{ -1 },//00000000
     new []{ 29, 0 },//00000000
     new []{ 29, 1 },//00000000
     new []{ 29, 2 },//00000000
     new []{ 29, 3 },//00000000
     new []{ 29, 4 },//00000000
     new []{ 29, 5 },//00000000
     new []{ 29, 6 },//00000000
     new []{ 29, 7 },//00000000
     new []{ 29, 8 },//00000000
     new []{ 29, 9 },//00000000
     new []{ 29, 10 },//00000000
     new []{ 29, 11 },//00000000
     new []{ 29, 12 },//00000000
     new []{ 29, 13 },//00000000
     new []{ 44 },//00000000
    };
}