using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureGeneration : MonoBehaviour
{
    public Texture2D noiseTexture;
    public RawImage image;

    public float scaleX = 20f;
    public float scaleY = 20f;

    public int textureSize = 512;

    private void Start()
    {
        UpdateTexture(new CWorldMask("", new CWorldNoise()));
    }
    

    public void UpdateTexture(CWorldMask mask)
    {
        noiseTexture = new Texture2D(textureSize, textureSize);
        GenerateNoise(mask);
        image.texture = noiseTexture;
    }
    
    private void GenerateNoise()
    {
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                float x = (float)((float)i / scaleX + 0.001f);
                float y = (float)((float)j / scaleY + 0.001f);

                float height = Mathf.Clamp01(Mathf.PerlinNoise(x, y));
                
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }
        
        noiseTexture.Apply();
    }

    private void GenerateNoise(CWorldMask mask)
    {
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                float height = mask.GetNoiseValue(i, j);
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }

        noiseTexture.Apply();
    }
}
