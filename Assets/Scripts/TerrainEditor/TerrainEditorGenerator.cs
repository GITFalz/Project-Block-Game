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
                map[x + z * lenght] = 0;
            }
        }

        return map;
    }
    
    public static float[] GetHeight(int mapSize, int mapDensity, CWorldDataHandler dataHandler)
    {
        if (dataHandler.sampleNodes.Count == 0)
            return GetHeight(mapSize, mapDensity);
        
        int lenght = mapDensity * mapSize + 1;
        int size = lenght * lenght;
        
        float[] map = new float[size];
        
        for (int x = 0; x < lenght; x++)
        {
            for (int z = 0; z < lenght; z++)
            {
                dataHandler.sampleNodes["Sample0"].Init((float)((float)(x * 10 + 0.001f) / (float)mapDensity), 0, (float)((float)(z * 10 + 0.001f) / (float)mapDensity));
                map[x + z * lenght] = 1 - dataHandler.sampleNodes["Sample0"].GetNoise();
            }
        }

        return map;
    }
}