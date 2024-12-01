using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CWorldMapNode : CWAPoolingNode
{
    public List<BiomePool> biomePool;
    
    private int _poolIndex = 0;
    private Dictionary<string, int> _sampleNames = new Dictionary<string, int>();

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
        
        if (biomeNode == null || biomes.Count == 0)
            return 0;
        
        if (biomes.Count == 1)
            return biomeNode.GetBlockPillar(chunkPosition, blocks, x, y, z);

        int count = 0;
        float total = 0;
        
        List<float> noiseValues = new List<float>();

        for (int i = 0; i < biomes.Count - 1; i++)
        {
            for (int j = i + 1; j < biomes.Count; j++)
            {
                BiomePool a = biomes[i];
                BiomePool b = biomes[j];
                
                noiseValues.Add(GetBiomeAvg(a, b));
            }
        }
        
        return biomeNode.GetBlockPillar(chunkPosition, blocks, x, y, z, MathUtils.Avg(noiseValues));
    }

    public float GetHeight01(int x, int z)
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

        return 0;
    }
    
    
    
    public float GetBiomeAvg(BiomePool a, BiomePool b)
    {
        //Get noise values for easy reading
        float noiseA = a.GetNoise();
        float noiseB = b.GetNoise();
        
        Dictionary<int, BiomePoolSample> inB = new Dictionary<int, BiomePoolSample>();
        List<float> noiseValues = new List<float>();

        //Add all samples from b to a dictionary for easy access
        foreach (var sample in b.samples)
            inB.Add(sample.index, sample);

        //If a and b have the same sample, lerp the noise values and remove the current sample from inB, else just add the noise value of a
        foreach (var sample in a.samples)
        {
            if (inB.TryGetValue(sample.index, out var s))
            {
                noiseValues.Add(NoiseLerpSample(sample, s, noiseA, noiseB, s.sample.noiseValue));
                inB.Remove(sample.index);
            }
            else
            {
                noiseValues.Add(noiseA);
            }
        }
        
        //Add the noise of b inB.Count times to the noiseValues list
        MathUtils.AddCount(noiseValues, inB.Count, noiseB);
        
        return MathUtils.Avg(noiseValues);
    }
    
    public float NoiseLerpSample(BiomePoolSample a, BiomePoolSample b, float noiseA, float noiseB, float t)
    {
        if (a.range.max <= b.range.max)
            return MathUtils.NoiseLerp(noiseA, noiseB, b.range.min, a.range.max, t);
            
        return MathUtils.NoiseLerp(noiseB, noiseA, a.range.min, b.range.max, t);
    }
    
    public void AddBiomeSample(CWorldSampleNode sampleNode, FloatRangeNode range)
    {
        if (_sampleNames.TryGetValue(sampleNode.name, out int index))
        {
            biomePool.Last().samples.Add(new BiomePoolSample(sampleNode, range, index));
        }
        else
        {
            _sampleNames.Add(sampleNode.name, _poolIndex);
            biomePool.Last().samples.Add(new BiomePoolSample(sampleNode, range, _poolIndex));
            _poolIndex++;
        }
    }
}

public class BiomePool
{
    public CWorldBiomeNode biome;
    public List<BiomePoolSample> samples;
    public int priority;

    public BiomePool(CWorldBiomeNode newBiome)
    {
        biome = newBiome;
        samples = new List<BiomePoolSample>();
        priority = 0;
    }

    public bool InRange()
    {
        foreach (var sample in samples)
        {
            if (!sample.InRange())
                return false;
        }

        return true;
    }

    public float GetNoise()
    {
        return biome.sample.noiseValue;
    }
}

public class BiomePoolSample
{
    public CWorldSampleNode sample;
    public FloatRangeNode range;

    public int index = 0;
    
    public BiomePoolSample(CWorldSampleNode sample, FloatRangeNode range, int index)
    {
        this.sample = sample;
        this.range = range;
        this.index = index;
    }

    public bool InRange()
    {
        float noise = sample.noiseValue;
        return range.min <= noise && noise <= range.max;
    }
    
    public string GetName()
    {
        return sample.name;
    }
}