using System.Collections.Generic;
using UnityEngine;

public class CWorldDataHandler
{
    public Dictionary<string, CWorldSampleNode> sampleNodes;
    public Dictionary<string, CWorldBiomeNode> biomeNodes;
    public Dictionary<string, CWorldModifierNode> modifierNodes;
    public Dictionary<string, CWorldLinkNode> linkNodes;
    public Dictionary<string, CWorldFoliageNode> foliageNodes;
    public CWorldSampleHandler SampleHandler;
    public CWorldSampleNode mainPoolSample;
    public CWorldMapNode MapNode;

    public CWorldDataHandler()
    {
        sampleNodes = new Dictionary<string, CWorldSampleNode>();
        biomeNodes = new Dictionary<string, CWorldBiomeNode>();
        modifierNodes = new Dictionary<string, CWorldModifierNode>();
        linkNodes = new Dictionary<string, CWorldLinkNode>();
        foliageNodes = new Dictionary<string, CWorldFoliageNode>();
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
    
    public async void SetupSamplePool(string sampleName)
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
        
        foreach (var s in sample.overrideNode.modifiers)
            AddSample(s.GetSample(), sampleHandler);
    }
    
    public float SampleNoise(int x, int y, int z, CWorldSampleNode sample)
    {
        SampleHandler.Init(x, y, z);
        return sample.GetNoise();
    }

    public uint GenerateBiomePillar(Vector3Int position, Block[] blocks, int x, int y, int z, string biomeName)
    {
        if (biomeNodes.TryGetValue(biomeName, out var node))
        {
            return node.GetBlockPillar(position, blocks, x, y, z);
        }

        return 0;
    }
    
    public uint GenerateMapPillar(Vector3Int position, Block[] blocks, int x, int y, int z)
    {
        return MapNode.GetBlockPillar(position, blocks, x, y, z, this);
    }
}