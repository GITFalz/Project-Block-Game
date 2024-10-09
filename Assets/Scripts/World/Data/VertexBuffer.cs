using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VertexBuffer
{
    public Dictionary<float3, int> _vertexBuffer;

    public VertexBuffer()
    {
        _vertexBuffer = new Dictionary<float3, int>();
    }

    public void Add(float3 position, int index)
    {
        _vertexBuffer.TryAdd(position, index);
    }

    public bool TryAdd(float3 position, out int newIndex)
    {
        newIndex = -1;

        if (_vertexBuffer.TryAdd(position, Count()))
        {
            return true;
        }
        newIndex = GetIndex(position);
        return false;            
    }

    public bool Contains(float3 position)
    {
        float3 pos1 = position + .01f;
        float3 pos2 = position - .01f;

        foreach(var v in _vertexBuffer.Keys) 
        { 
            if (v.x < pos1.x && v.y < pos1.y && v.z < pos1.z && v.x > pos2.x && v.y > pos2.y && v.z > pos2.z)
            {
                return true;
            }
        }
        return false;
    }

    public int GetIndex(float3 position)
    {
        return _vertexBuffer[position];
    }

    public int Count()
    {
        return _vertexBuffer.Count;
    }

    public void Clear()
    {
        _vertexBuffer.Clear();
    }
}
