using System.Collections.Generic;
using UnityEngine;

public class CWorldMapNode : CWAPoolingNode
{
    public CWorldSampleHandler sampleHandler;
    public List<BiomePool> biomePool;

    public CWorldMapNode()
    {
        sampleHandler = new CWorldSampleHandler();
        biomePool = new List<BiomePool>();
    }

    public Block[] GetBlocks(Vector3Int chunkPosition, Block[] blocks, CWorldHandler handler)
    {
        
        foreach (var biome in biomePool)
        {

        }
        
        return blocks;
    }

    public uint GetPillar(int x, int y, int z, Vector3Int chunkPosition, CWorldHandler handler)
    {
        sampleHandler.Init(x, y, z);

        return 1;
    }
}

public class BiomePool
{
    public CWorldBiomeNode biome;
    public List<BiomePoolSample> samples;

    public BiomePool(CWorldBiomeNode newBiome)
    {
        biome = newBiome;
        samples = new List<BiomePoolSample>();
    }
}

public class BiomePoolSample
{
    public CWorldSampleNode sample;
    public float min;
    public float max;
    
    public BiomePoolSample(CWorldSampleNode sample)
    {
        this.sample = sample;
        min = 0;
        max = 1;
    }

    public bool InRange()
    {
        float noise = sample.noiseValue;
        return min <= noise && noise <= max;
    }
}