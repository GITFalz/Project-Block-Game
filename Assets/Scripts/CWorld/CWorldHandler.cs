using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public static Dictionary<string, CWorldSampleNode> sampleNodes;
    public static Dictionary<string, CWorldBiomeNode> biomeNodes;
    public static CWorldMapNode MapNode;

    public CWorldSampleHandler SampleHandler;

    private bool test = true;

    public CWorldHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
    }

    public void Init()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
    }

    public float GetTextureNoise(int x, int y, int z)
    {
        foreach (var i in sampleNodes.Values)
        {
            i.Init(x, y, z);
        }

        float height = 0;

        foreach (CWorldBiomeNode e in biomeNodes.Values)
        {
            float noise = e.GetNoise();
            if (noise > height)
                height = noise;
        }

        return height;
    }
    
    public float GetSampleNoise(int x, int y, int z, CWorldSampleNode i)
    {
        Init(x, y, z);
        return i.GetNoise();
    }

    public void AddSample(CWorldSampleNode sample, CWorldSampleHandler sampleHandler)
    {
        sampleHandler.sampleNodes.TryAdd(sample.name, sample);
        
        foreach (var s in sample.overrideNode.add)
            AddSample(s, sampleHandler);
        foreach (var s in sample.overrideNode.multiply)
            AddSample(s, sampleHandler);
        foreach (var s in sample.overrideNode.subtract)
            AddSample(s, sampleHandler);
    }

    public Block[] GenerateBiome(Vector3Int position, Block[] blocks, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlocks(position, blocks, this);
        }

        return blocks;
    }

    public void Init(int x, int y, int z)
    {
        foreach (var i in ChunkGenerationNodes.dataHandlers[0].sampleNodes.Values)
        {
            i.Init(x, y, z);
        }
        
        foreach (var i in ChunkGenerationNodes.dataHandlers[0].sampleNodes.Values)
        {
            i.ApplyOverride();
        }
    }
}

public class CWorldSampleHandler
{
    public Dictionary<string, CWorldSampleNode> sampleNodes;

    public CWorldSampleHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
    }
    
    public void Init(int x, int y, int z)
    {
        foreach (var i in sampleNodes.Values)
        {
            i.Init(x, y, z);
        }
        
        foreach (var i in sampleNodes.Values)
        {
            i.ApplyOverride();
        }
    }
}

public class CWorldDataHandler
{
    public Dictionary<string, CWorldSampleNode> sampleNodes;
    public Dictionary<string, CWorldBiomeNode> biomeNodes;
    public CWorldSampleHandler SampleHandler;
    public CWorldSampleNode mainPoolSample;
    public CWorldMapNode MapNode;

    public CWorldDataHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
        SampleHandler = new CWorldSampleHandler();
        MapNode = null;
    }

    public void Init(int x, int y, int z)
    {
        foreach (var i in sampleNodes.Values)
        {
            i.Init(x, y, z);
        }
        
        foreach (var i in sampleNodes.Values)
        {
            i.ApplyOverride();
        }
    }
    
    public void SetupSamplePool(string sampleName)
    {
        if (sampleNodes.TryGetValue(sampleName, out var sample))
        {
            mainPoolSample = sample;
            AddSample(sample, SampleHandler);
        }
    }
    
    public static void AddSample(CWorldSampleNode sample, CWorldSampleHandler sampleHandler)
    {
        sampleHandler.sampleNodes.TryAdd(sample.name, sample);
        
        foreach (var s in sample.overrideNode.add)
            AddSample(s, sampleHandler);
        foreach (var s in sample.overrideNode.multiply)
            AddSample(s, sampleHandler);
        foreach (var s in sample.overrideNode.subtract)
            AddSample(s, sampleHandler);
    }
    
    public float SampleNoise(int x, int y, int z, CWorldSampleNode sample)
    {
        SampleHandler.Init(x, y, z);
        return sample.GetNoise();
    }

    public Block[] GenerateBiome(Vector3Int position, Block[] blocks, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlocks(position, blocks, this);
        }

        return blocks;
    }

    public int GenerateBiomePillar(Vector3Int position, Block[] blocks, int x, int z, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlockPillar(position, blocks, x, z);
        }

        return 0;
    }
    
    public int GenerateMapPillar(Vector3Int position, Block[] blocks, int x, int z)
    {
        return MapNode.GetBlockPillar(position, blocks, x, z, this);
    }
}