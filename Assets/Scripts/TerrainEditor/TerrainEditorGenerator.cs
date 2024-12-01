using UnityEngine;

public static class TerrainEditorGenerator
{
    public static float[] GetHeight(int mapSize, int mapDensity)
    {
        int lenght = mapDensity * mapSize + 1;
        int size = lenght * lenght;
        
        float[] map = new float[size];
        
        for (int x = 0; x < lenght; x++)
        {
            for (int z = 0; z < lenght; z++)
            {
                map[x + z * lenght] = Mathf.PerlinNoise((float)((float)(x + 0.001f) / (float)mapDensity) + 0.001f, (float)((float)(z + 0.001f) / (float)mapDensity) + 0.001f);
            }
        }

        return map;
    }
}