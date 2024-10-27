using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldInfo
{
    public static short worldChunkHeight = 5;
    public static int worldSeaLevel = 60;
    public static short worldMaxTerrainHeight = 256;
    public static int worldMinTerrainHeight = 1;
    public static short worldTerrainHeight = 160;

    public static float terrainSurface = 0f;
    public static float terrainDensity = 100f;

    public static int DropOfLOD = 5;
    //Between 0 and 4
    public static int MaxLODlevel = 4;
}
