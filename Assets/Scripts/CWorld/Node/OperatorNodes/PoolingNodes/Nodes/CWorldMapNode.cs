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

    public int GetBlockPillar(Vector3Int chunkPosition, Block[] blocks, int x, int z, CWorldHandler handler)
    {
        int i = 0;
        float noise = 0;

        List<BiomePool> biomes = new List<BiomePool>();
        
        foreach (var biome in biomePool)
        {
            if (biome.InRange())
            {
                biomes.Add(biome);
            }
        }

        if (biomes.Count == 1)
        {
            return biomes[0].GetBiomePillar(chunkPosition, blocks, x, z);
        }
        if (biomes.Count == 2)
        {
            foreach (var sample in biomePool[0].samples)
            {
                if (biomePool[1].samples.TryGetValue(sample.Key, out var s))
                {
                    noise = NoiseLerpSample(sample.Value, s, biomePool[0].biome.sample.noiseValue, biomePool[1].biome.sample.noiseValue, s.sample.noiseValue);
                    return biomes[0].GetBiomePillar(chunkPosition, blocks, x, z, noise);
                }
            }
        }

        return 0;
    }
    
    public int GetBlockPillar(Vector3Int chunkPosition, Block[] blocks, int x, int z, CWorldDataHandler handler)
    {
        int i = 0;
        float noise = 0;

        List<BiomePool> biomes = new List<BiomePool>();
        
        foreach (var biome in biomePool)
        {
            if (biome.InRange())
            {
                biomes.Add(biome);
            }
        }

        if (biomes.Count == 1)
        {
            return biomes[0].GetBiomePillar(chunkPosition, blocks, x, z);
        }
        
        if (biomes.Count == 2)
        {
            foreach (var sample in biomePool[1].samples)
            {
                if (biomePool[0].samples.TryGetValue(sample.Key, out var s))
                {
                    noise = NoiseLerpSample(sample.Value, s, biomePool[0].biome.sample.noiseValue, biomePool[1].biome.sample.noiseValue, s.sample.noiseValue);
                    return biomes[0].GetBiomePillar(chunkPosition, blocks, x, z, noise);
                }
            }
        }

        return 0;
    }

    public uint GetPillar(int x, int y, int z, Vector3Int chunkPosition, CWorldHandler handler)
    {
        sampleHandler.Init(x, y, z);

        return 1;
    }
    
    public float NoiseLerpSample(BiomePoolSample a, BiomePoolSample b, float noiseA, float noiseB, float t)
    {
        if (a.max > b.min)
        {
            float noise = Mathp.NoiseLerp(noiseA, noiseB, a.min, b.max, t);
            //Debug.Log("noiseA: " + noiseA + " noiseB: " + noiseB + " minB: " + b.min + " maxA: " + a.max + " t: " + t + " gave noise: " + noise);
            return noise;
        }
        else
        {
            float noise = Mathp.NoiseLerp(noiseB, noiseA, b.min, a.max, t);
            //Debug.Log("noiseA: " + noiseB + " noiseB: " + noiseA + " minB: " + a.min + " maxA: " + b.max + " t: " + t + " gave noise: " + noise);
            return noise;
        }
    }
}

public class BiomePool
{
    public CWorldBiomeNode biome;
    public Dictionary<string, BiomePoolSample> samples;

    public BiomePool(CWorldBiomeNode newBiome)
    {
        biome = newBiome;
        samples = new Dictionary<string, BiomePoolSample>();
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

    public int GetBiomePillar(Vector3Int chunkPosition, Block[] blocks, int x, int z)
    {
        return biome.GetBlockPillar(chunkPosition, blocks, x, z);
    }
    
    public int GetBiomePillar(Vector3Int chunkPosition, Block[] blocks, int x, int z, float noise)
    {
        return biome.GetBlockPillar(chunkPosition, blocks, x, z, noise);
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

    public BiomePoolSample Copy(CWorldDataHandler handler)
    {
        BiomePoolSample biomePoolSample = new BiomePoolSample(sample.Copy(handler))
        {
            min = min,
            max = max,
        };
        return biomePoolSample;
    }
}