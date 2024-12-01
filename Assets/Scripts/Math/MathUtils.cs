using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtils
{
    public const float Sqrt2 = 1.2247448f;
    
    
    // Chunk functions
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
    
    
    
    // Terrain modification functions
    public static float PLerp(float min, float max, float value)
    {
        float midpoint = (min + max) / 2;
        if (value < min || value > max)
            return 0;

        float distanceFromMidpoint = Mathf.Abs(value - midpoint);
        float totalDistance = (max - min) / 2;
        
        float proximityValue = 1 - (distanceFromMidpoint / totalDistance);

        return proximityValue;
    }
    
    public static float SLerp(float min, float max, float value)
    {
        if (value <= min) return 0f;
        if (value >= max) return 1f;
        return (value - min) / (max - min);
    }

    
    // Blend functions
    public static float NoiseLerp(float noiseA, float noiseB, float minB, float maxA, float t)
    {
        if (maxA - minB == 0)
            return noiseA;
        
        float nt = (Clamp(t, minB, maxA) - minB) / (maxA - minB);
        return Lerp(noiseA, noiseB, nt);
    }
    
    
    
    // List functions
    public static void AddCount(List<float> list, int count, float value)
    {
        for (int i = 0; i < count; i++)
            list.Add(value);
    }
    
    public static void AddCount(List<int> list, int count, int value)
    {
        for (int i = 0; i < count; i++)
            list.Add(value);
    }
    
    public static float Avg(List<float> values)
    {
        if (values.Count == 0)
            return 0;
        return Total(values) / values.Count;
    }

    public static float Max(List<float> values)
    {
        if (values.Count == 0)
            return 0;

        float max = values[0];
        
        for (int i = 1; i < values.Count; i++)
            if (values[i] > max)
                max = values[i];

        return max;
    }
    
    public static float Min(List<float> values)
    {
        if (values.Count == 0)
            return 0;

        float min = values[0];
        
        for (int i = 1; i < values.Count; i++)
            if (values[i] < min)
                min = values[i];

        return min;
    }

    public static float Total(List<float> values)
    {
        float total = 0;
        foreach (float value in values)
            total += value;
        return total;
    }
    
    

    
    // Unity's Mathf functions
    public static float Clamp(float v, float min, float max)
    {
        return Mathf.Clamp(v, min, max);
    }

    public static float Lerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }
}