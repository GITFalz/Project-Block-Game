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
}
