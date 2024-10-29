using System;
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

    public static int Round(float value, float midPoint)
    {
        return value < midPoint ? 0 : 1;
    }
    
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

    public static float NoiseLerp(float noiseA, float noiseB, float minB, float maxA, float t)
    {
        if (maxA - minB == 0)
            return noiseA;
        
        float nt = (Clamp(t, minB, maxA) - minB) / (maxA - minB);
        return Lerp(noiseA, noiseB, nt);
    }

    public static float Clamp(float v, float min, float max)
    {
        return Mathf.Clamp(v, min, max);
    }

    public static float Lerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }
}