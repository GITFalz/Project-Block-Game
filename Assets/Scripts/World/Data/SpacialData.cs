using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class SpacialData
{
    public static int[] sideChecks = new int[6]
    {
        -32, 1, 1024, -1, -1024, 32
    };
    
    public static int[] oppositeFace = new int[6]
    {
        5, 3, 4, 1, 2, 0
    };
}
