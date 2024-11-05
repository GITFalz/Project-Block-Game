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
    //public Dictionary<string, CWAInitializerNode> initializers;
    //public Dictionary<string, CWAExecuteNode> executes;

    public CWorldSampleHandler SampleHandler;

    private bool test = true;

    public CWorldHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
        //MapNode = null;
    }

    public void Init()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
        //initializers = new Dictionary<string, CWAInitializerNode>();
        //executes = new Dictionary<string, CWAExecuteNode>();
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

    public CWorldSampleNode SetupSamplePool(string sampleName)
    {
        SampleHandler = new CWorldSampleHandler();

        if (sampleNodes.TryGetValue(sampleName, out var sample))
        {
            AddSample(sample, SampleHandler);
            return sample;
        }

        return null;
    }

    public float SampleNoise(int x, int y, int z, CWorldSampleNode sample)
    {
        SampleHandler.Init(x, y, z);
        return sample.GetNoise();
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

    public uint GenerateBiomePillar(Vector3Int position, Block[] blocks, int x, int z, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlockPillar(position, blocks, x, z);
        }

        return 0;
    }
    
    public uint GenerateMapPillar(Vector3Int position, Block[] blocks, int x, int z)
    {
        return MapNode.GetBlockPillar(position, blocks, x, z, this);
    }
    
    public uint GetBlockMapPillar(int x, int y, int z, string sampleName)
    {
        if (sampleNodes.TryGetValue(sampleName, out CWorldSampleNode i))
        {
            return i.GetPillar(x, y, z);
        }
        return 0;
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
    
    public Block GetBlock(int x, int y, int z)
    {
        Block block = null;
        foreach (CWorldBiomeNode execute in biomeNodes.Values)
        {
            //Block b = execute.GetBlock(x, y, z);
            //if (b != null) block = b;
        }

        return block;
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
    public Dictionary<string, CWorldModifierNode> modifierNodes;
    public CWorldSampleHandler SampleHandler;
    public CWorldSampleNode mainPoolSample;
    public CWorldMapNode MapNode;

    public CWorldDataHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
        modifierNodes = new Dictionary<string, CWorldModifierNode>();
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

    public uint GenerateBiomePillar(Vector3Int position, Block[] blocks, int x, int z, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName.Trim(), out var node))
        {
            return node.GetBlockPillar(position, blocks, x, z);
        }

        return 0;
    }
    
    public uint GenerateMapPillar(Vector3Int position, Block[] blocks, int x, int z)
    {
        return MapNode.GetBlockPillar(position, blocks, x, z, this);
    }
}