using System.Collections.Generic;
using UnityEngine;

public class CWorldMapNode : CWAPoolingNode
{
    public List<BiomePool> biomePool;

    public CWorldMapNode()
    {
        biomePool = new List<BiomePool>();
    }
    
    public uint GetBlockPillar(Vector3Int chunkPosition, Block[] blocks, int x, int y, int z, CWorldDataHandler handler)
    {
        int priority = -1;
        CWorldBiomeNode biomeNode = null;
        List<BiomePool> biomes = new List<BiomePool>();
        
        foreach (var biome in biomePool)
        {
            if (biome.InRange())
            {
                biomes.Add(biome);
                if (biome.priority > priority)
                {
                    priority = biome.priority;
                    biomeNode = biome.biome;
                }
            }
        }
        
        if (biomeNode == null)
            return 0;
        
        switch (biomes.Count)
        {
            case 0:
                return 0;
            case 1:
                return biomeNode.GetBlockPillar(chunkPosition, blocks, x, y, z);
        }

        int count = 0;
        float total = 0;

        for (int i = 0; i < biomes.Count - 1; i++)
        {
            for (int j = i + 1; j < biomes.Count; j++)
            {
                int index = 0;
                float avg = 0;
                
                foreach (var sample in biomes[i].samples)
                {
                    if (biomes[j].samples.TryGetValue(sample.Key, out var s))
                    {
                        index++;
                        avg += NoiseLerpSample(sample.Value, s, biomes[i].biome.sample.noiseValue, biomes[j].biome.sample.noiseValue, s.sample.noiseValue);
                    }
                }
                
                count++;
                total += index == 0 ? 0 : avg / index;
            }
        }
        
        return biomeNode.GetBlockPillar(chunkPosition, blocks, x, y, z, total / count);
    }
    
    public float NoiseLerpSample(BiomePoolSample a, BiomePoolSample b, float noiseA, float noiseB, float t)
    {
        return a.max > b.min ? Mathp.NoiseLerp(noiseA, noiseB, a.min, b.max, t) : Mathp.NoiseLerp(noiseB, noiseA, b.min, a.max, t);
    }
}

public class BiomePool
{
    public CWorldBiomeNode biome;
    public Dictionary<string, BiomePoolSample> samples;
    public int priority;

    public BiomePool(CWorldBiomeNode newBiome)
    {
        biome = newBiome;
        samples = new Dictionary<string, BiomePoolSample>();
        priority = 0;
    }
    
    public void SetPriority(int p)
    {
        priority = p < 0 ? 0 : p;
    }

    public bool InRange()
    {
        foreach (var sample in samples.Values)
        {
            if (!sample.InRange())
                return false;
        }

        return true;
    }

    public uint GetBiomePillar(Vector3Int chunkPosition, Block[] blocks, int x, int y, int z)
    {
        return biome.GetBlockPillar(chunkPosition, blocks, x, y, z);
    }
    
    public uint GetBiomePillar(Vector3Int chunkPosition, Block[] blocks, int x, int y, int z, float noise)
    {
        return biome.GetBlockPillar(chunkPosition, blocks, x, y, z, noise);
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