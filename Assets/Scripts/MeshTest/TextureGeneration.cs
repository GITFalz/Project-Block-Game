using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureGeneration : MonoBehaviour
{
    public Texture2D noiseTexture;
    public RawImage image;
    
    public CWorldHandler handler;

    public float scaleX = 20f;
    public float scaleY = 20f;

    public int textureSize = 512;

    private void Start()
    {
        
    }
    

    public void UpdateTexture(CSampleNode sample)
    {
        noiseTexture = new Texture2D(textureSize, textureSize);
        GenerateNoise(sample);
        image.texture = noiseTexture;
    }
    
    public void UpdateTexture(string sampleName)
    {
        noiseTexture = new Texture2D(textureSize, textureSize);
        GenerateNoise(sampleName);
        image.texture = noiseTexture;
    }
    
    public void UpdateTexture()
    {
        noiseTexture = new Texture2D(textureSize, textureSize);
        GenerateNoise();
        image.texture = noiseTexture;
    }

    private void GenerateNoise()
    {
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                float height = handler.GetTextureNoise(i, j);
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }

        noiseTexture.Apply();
    }
    
    private void GenerateNoise(string sampleName)
    {
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                float height = handler.GetSampleNoise(i, j, sampleName);
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }

        noiseTexture.Apply();
    }
    
    private void GenerateNoise(CSampleNode sample)
    {
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                handler.Init(i, j);
                float height = sample.GetNoise();
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }

        noiseTexture.Apply();
    }
}
