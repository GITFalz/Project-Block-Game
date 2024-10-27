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

    public bool _move = false;
    private Vector3 _direction;
    private string _sampleName;

    private float _timer;

    private void Start()
    {
        _direction = Vector3.zero;
        _sampleName = "";

        _timer = 0;
    }

    private void Update()
    {
        if (_move)
        {
            if (_timer <= 0)
            {
                _timer = 0.2f;
                MoveTexture();
            }
            else
            {
                _timer -= Time.deltaTime;
            }
        }
    }

    public void MoveTexture()
    {
        Vector2 moveInput = InputManager.MoveInput();
        
        if (!moveInput.Equals(Vector2.zero))
        {
            int speed = 20;

            if (InputManager.ControlInput())
                speed = 100;
            
            _direction.y += moveInput.y * speed;
            _direction.x += moveInput.x * speed;
            
            GenerateNoise(_sampleName);
        }
    }
    
    public void UpdateTexture(string sampleName)
    {
        noiseTexture = new Texture2D(textureSize, textureSize);
        _sampleName = sampleName;
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
                float height = handler.GetTextureNoise(i + (int)_direction.x, 0, j + (int)_direction.y);
                noiseTexture.SetPixel(i, j, new Color(height, height, height));
            }
        }

        noiseTexture.Apply();
    }
    
    private void GenerateNoise(string sampleName)
    {
        if (handler.initializers.TryGetValue(sampleName, out CWAInitializerNode init))
        {
            for (int i = 0; i < textureSize; i++)
            {
                for (int j = 0; j < textureSize; j++)
                {
                    float height = handler.GetSampleNoise(i + (int)_direction.x, 0, j + (int)_direction.y, init);
                    noiseTexture.SetPixel(i, j, new Color(height, height, height));
                }
            }
            
            noiseTexture.Apply();
        }
    }

    public void SetMove(bool move)
    {
        _move = move;
    }
}
