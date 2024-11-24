using UnityEngine;

public static class NoiseUtils
{
    private static int[] angleArray = 
    {
        13, 325, 277, 232, 121, 106, 52, 3, 37, 290, 170, 
        207, 35, 160, 153, 271, 34, 172, 301, 146,
        268, 24, 192, 240, 132, 12, 241, 157, 38, 305, 
        133, 81, 75, 218, 253, 0, 187, 42, 254, 65, 59,
        169, 173, 92, 285, 28, 264, 84, 141, 184, 319, 
        128, 287, 117, 225, 270, 189, 68, 344, 54, 91,
        289, 340, 19, 114, 298, 244, 70, 129, 118, 103, 
        166, 96, 339, 299, 120, 318, 125, 233, 122, 261,
        20, 85, 292, 148, 97, 195, 61, 107, 220, 262, 
        178, 151, 164, 200, 330, 322, 43, 265, 140, 257, 210,
        321, 312, 123, 158, 130, 260, 159, 137, 310, 
        40, 22, 49, 171, 247, 29, 86, 82, 335, 6, 208, 231,
        327, 44, 272, 168, 194, 224, 14, 278, 26, 235, 
        190, 186, 147, 47, 27, 95, 179, 320, 259, 230,
        217, 199, 78, 258, 110, 300, 119, 291, 209, 315,
        294, 348, 101, 73, 302, 80, 144, 79, 198, 138,
        18, 152, 2, 102, 295, 45, 58, 55, 98, 56, 313, 
        286, 185, 7, 100, 57, 206, 99, 354, 196, 246,
        307, 323, 357, 228, 226, 165, 238, 105, 334, 
        314, 88, 337, 273, 1, 139, 11, 53, 251, 15,
        39, 124, 23, 250, 212, 66, 63, 342, 256, 4, 
        236, 239, 25, 324, 329, 202, 9, 345, 216, 284,
        288, 326, 33, 180, 297, 89, 93, 205, 193, 115, 
        263, 347, 221, 183, 266, 350, 332, 252, 333, 249,
        306, 116, 108, 8, 134, 296, 213, 161, 5, 248, 
        145, 163, 275, 136, 303, 104, 234, 311, 293, 358,
        21, 279, 243, 267, 219, 111, 135, 150, 276, 
        31, 309, 51, 10, 60, 245, 269, 155, 87, 126, 16, 64,
        176, 211, 222, 127, 156, 341, 229, 62, 281, 
        154, 283, 356, 203, 46, 90, 162, 149, 71, 69, 346,
        67, 177, 353, 77, 72, 343, 175, 328, 32, 215,
        113, 331, 338, 204, 197, 76, 142, 83, 351, 317,
        227, 188, 50, 94, 181, 255, 316, 17, 131, 308, 
        74, 109, 223, 174, 36, 355, 242, 304, 191, 41,
        143, 182, 237, 280, 30, 201, 167, 48, 336, 352,
        349, 214, 112, 274, 359, 282
    };

    private static float[] variations =
    {
        0.53f, 0.25f, 0.75f, 0.27f, 0.56f, 0.86f, 0.68f, 0.22f, 0.89f, 0.58f, 
        0.41f, 0.29f, 0.45f, 0.91f, 0.28f, 0.15f, 0.32f, 0.94f, 0.01f, 0.46f, 
        0.61f, 0.14f, 0.44f, 0.57f, 0.13f, 0.55f, 0.96f, 0.88f, 0.33f, 0.26f, 
        0.95f, 0.37f, 0.16f, 0.04f, 0.63f, 0.38f, 0.23f, 0.93f, 0.64f, 0.02f, 
        0.49f, 0.73f, 0.39f, 0.82f, 0.62f, 0.12f, 0.31f, 0.65f, 0.2f, 0.74f, 
        0.17f, 0.67f, 0.4f, 0.18f, 0.81f, 0.54f, 0.07f, 0.72f, 0.11f, 0.42f, 
        0.71f, 0.66f, 0.7f, 0.85f, 0.78f, 0.36f, 0.34f, 0.08f, 0.47f, 0.83f, 
        0.69f, 0.76f, 0.9f, 0.06f, 0.03f, 0.43f, 0.98f, 0.59f, 0.87f, 0.21f, 
        0.09f, 0.92f, 0.77f, 0.5f, 0.97f, 0.84f, 0.24f, 0.35f, 0.8f, 0.99f, 
        0.79f, 1.0f, 0.05f, 0.48f, 0.1f, 0.19f, 0.52f, 0.6f, 0.51f, 0.0f, 0.3f,
    };

    private static int[] offsetArray1 = GetRandomOffset(1, 5);
    private static int[] offsetArray2 = GetRandomOffset(0, 9);

    private static int angleIndex = 0;
    private static int indexA = 0;
    private static int indexB = 0;
    
    private static int variationIndex = 0;
    private static int vA = 0;
    private static int vB = 0;
    
    public static int GetRandomAngle(int offset = 1)
    {
        indexA = (indexA+offset) % 256;
        indexB += offsetArray1[indexA];
        indexB %= 256;
        
        angleIndex += offsetArray1[(indexA + offsetArray2[indexB]) % 256];
        angleIndex %= 360;
        
        return angleArray[angleIndex];
    }
    
    public static float GetRandomRange(float a, float b)
    {
        vA = (vA + 1) % 256;
        vB += offsetArray1[vA];
        vB %= 256;
        
        variationIndex += offsetArray1[(vA + offsetArray2[vB]) % 256];
        variationIndex %= 101;
        
        return Mathf.Lerp(a, b, variations[variationIndex]);
    }
    
    public static float GetRandomRange(float a, float b, float x, float y, float z)
    {
        float noise = Mathf.PerlinNoise((float)((float)x / 10 + y + 0.001f) , (float)((float)z / 10 + y + 0.001f));
        return Mathf.Lerp(a, b, noise);
    }
    
    private static int[] GetRandomOffset(int a, int b)
    {
        int[] array = new int[256];
        
        for (int i = 0; i < 256; i++)
        {
            array[i] = (int)Mathf.Lerp(a, b, Mathf.PerlinNoise(0.001f + (float)i, 0.001f + (float)i));
        }

        return array;
    }
    
    public static float GetNoiseAtPos(float x, float y, float z)
    {
        float X = (float)((float)x + (float)y + 0.001f);
        float Y = (float)((float)z + (float)y + 0.001f);
        
        return Mathf.PerlinNoise(X / 10, Y / 10);
    }
    
    public static float LerpRange(FloatRangeNode range, float noise)
    {
        return Mathf.Lerp(range.min, range.max, noise);
    }
    
    public static int LerpRange(IntRangeNode range, float noise)
    {
        return (int)Mathf.Lerp(range.min, range.max, noise);
    }
    
    #region Noise functions

    public static float Noise(float x)
    {
        var X = Mathf.FloorToInt(x) & 0xff;
        x -= Mathf.Floor(x);
        var u = Fade(x);
        return Lerp(u, Grad(perm[X], x), Grad(perm[X+1], x-1)) * 2;
    }

    public static float Noise(float x, float y)
    {
        var X = Mathf.FloorToInt(x) & 0xff;
        var Y = Mathf.FloorToInt(y) & 0xff;
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        var u = Fade(x);
        var v = Fade(y);
        var A = (perm[X  ] + Y) & 0xff;
        var B = (perm[X+1] + Y) & 0xff;
        return Lerp(v, Lerp(u, Grad(perm[A  ], x, y  ), Grad(perm[B  ], x-1, y  )),
                       Lerp(u, Grad(perm[A+1], x, y-1), Grad(perm[B+1], x-1, y-1)));
    }

    public static float Noise(Vector2 coord)
    {
        return Noise(coord.x, coord.y);
    }

    public static float Noise(float x, float y, float z)
    {
        var X = Mathf.FloorToInt(x) & 0xff;
        var Y = Mathf.FloorToInt(y) & 0xff;
        var Z = Mathf.FloorToInt(z) & 0xff;
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        z -= Mathf.Floor(z);
        var u = Fade(x);
        var v = Fade(y);
        var w = Fade(z);
        var A  = (perm[X  ] + Y) & 0xff;
        var B  = (perm[X+1] + Y) & 0xff;
        var AA = (perm[A  ] + Z) & 0xff;
        var BA = (perm[B  ] + Z) & 0xff;
        var AB = (perm[A+1] + Z) & 0xff;
        var BB = (perm[B+1] + Z) & 0xff;
        return Lerp(w, Lerp(v, Lerp(u, Grad(perm[AA  ], x, y  , z  ), Grad(perm[BA  ], x-1, y  , z  )),
                               Lerp(u, Grad(perm[AB  ], x, y-1, z  ), Grad(perm[BB  ], x-1, y-1, z  ))),
                       Lerp(v, Lerp(u, Grad(perm[AA+1], x, y  , z-1), Grad(perm[BA+1], x-1, y  , z-1)),
                               Lerp(u, Grad(perm[AB+1], x, y-1, z-1), Grad(perm[BB+1], x-1, y-1, z-1))));
    }

    public static float Noise(Vector3 coord)
    {
        return Noise(coord.x, coord.y, coord.z);
    }

    #endregion

    #region fBm functions

    public static float Fbm(float x, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise(x);
            x *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }

    public static float Fbm(Vector2 coord, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise(coord);
            coord *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }

    public static float Fbm(float x, float y, int octave)
    {
        return Fbm(new Vector2(x, y), octave);
    }

    public static float Fbm(Vector3 coord, int octave)
    {
        var f = 0.0f;
        var w = 0.5f;
        for (var i = 0; i < octave; i++) {
            f += w * Noise(coord);
            coord *= 2.0f;
            w *= 0.5f;
        }
        return f;
    }

    public static float Fbm(float x, float y, float z, int octave)
    {
        return Fbm(new Vector3(x, y, z), octave);
    }

    #endregion

    #region Private functions

    static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    static float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    static float Grad(int hash, float x)
    {
        return (hash & 1) == 0 ? x : -x;
    }

    static float Grad(int hash, float x, float y)
    {
        return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
    }

    static float Grad(int hash, float x, float y, float z)
    {
        var h = hash & 15;
        var u = h < 8 ? x : y;
        var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    static int[] perm = {
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

    #endregion
}
