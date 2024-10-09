using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
    {
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }

    public static float RemapValue01(float value, float outputMin, float outputMax)
    {
        return outputMin + value * (outputMax - outputMin) / 1;
    }

    public static int RemapValue01ToInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValue01(value, outputMin, outputMax);  
    }

    public static float Redistribution(float noise, NoiseSettings settings)
    {
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0;

        for (int i  = 0; i < settings.octaves; i++)
        {
            total += Mathf.PerlinNoise((settings.offset.x + settings.worldOffset.x + x) * frequency, (settings.offset.y + settings.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= settings.persistance;
            frequency *= 2;
        }

        return total / amplitudeSum;
    }

    public static float TerrainNoise(int x, int y, int z, float frequency)
    {
        return Get3DNoise(x, y, z, frequency);
    }

    public static float HeightMapNoise(float x, float z, float width, float height)
    {
        return Mathf.PerlinNoise((float)x / width + .001f, (float)z / height + .001f);
    }

    public static float Get3DNoise(float x, float y, float z, float frequency)
    {
        x *= frequency; y *= frequency; z *= frequency;

        int X0 = Mathf.FloorToInt(x);
        int Y0 = Mathf.FloorToInt(y);
        int Z0 = Mathf.FloorToInt(z);

        float Dx0 = x - X0;
        float Dy0 = y - Y0;
        float Dz0 = z - Z0;

        float Dx1 = Dx0 - 1;
        float Dy1 = Dy0 - 1;
        float Dz1 = Dz0 - 1;

        X0 &= 255;
        Y0 &= 255;
        Z0 &= 255;

        int X1 = X0 + 1;
        int Y1 = Y0 + 1;
        int Z1 = Z0 + 1;

        int Px0 = perm[X0];
        int Px1 = perm[X0];

        int Py00 = perm[Px0 + Y0];
        int Py10 = perm[Px1 + Y0];
        int Py01 = perm[Px0 + Y1];
        int Py11 = perm[Px1 + Y1];

        Vector3 D000 = dir[perm[Py00 + Z0] & 15];
        Vector3 D100 = dir[perm[Py10 + Z0] & 15];
        Vector3 D010 = dir[perm[Py01 + Z0] & 15];
        Vector3 D110 = dir[perm[Py11 + Z0] & 15];
        Vector3 D001 = dir[perm[Py00 + Z1] & 15];
        Vector3 D101 = dir[perm[Py10 + Z1] & 15];
        Vector3 D011 = dir[perm[Py01 + Z1] & 15];
        Vector3 D111 = dir[perm[Py11 + Z1] & 15];

        float V000 = scalar(D000, new Vector3(Dx0, Dy0, Dz0));
        float V100 = scalar(D100, new Vector3(Dx1, Dy0, Dz0));
        float V010 = scalar(D010, new Vector3(Dx0, Dy1, Dz0));
        float V110 = scalar(D110, new Vector3(Dx1, Dy1, Dz0));
        float V001 = scalar(D001, new Vector3(Dx0, Dy0, Dz1));
        float V101 = scalar(D101, new Vector3(Dx1, Dy0, Dz1));
        float V011 = scalar(D011, new Vector3(Dx0, Dy1, Dz1));
        float V111 = scalar(D111, new Vector3(Dx1, Dy1, Dz1));

        float SdX = smoothDistance(Dx0);
        float SdY = smoothDistance(Dy0);
        float SdZ = smoothDistance(Dz0);

        return Mathf.Lerp(
            Mathf.Lerp(Mathf.Lerp(V000, V100, SdX), Mathf.Lerp(V010, V110, SdX), SdY),
            Mathf.Lerp(Mathf.Lerp(V001, V101, SdX), Mathf.Lerp(V011, V111, SdX), SdY),
            SdZ
        );
    }

    private static Vector3[] dir = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };

    private static int[] perm = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151,

        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151
    };

    private static float scalar(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    private static float smoothDistance(float d)
    {
        return d * d * d * (d * (d * 6f - 15f) + 10f);
    }
}
