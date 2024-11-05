using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public static class ChunkGenerationNodes
{
    public static List<Task> tasks;
    public static List<CWorldDataHandler> dataHandlers;

    public static string currentSampleName = "";
    public static string currentBiomeName = "";
    public static string sampleDisplayName = "";
    public static string currentModifierName = "";

    public static int threadCount = 4;
    
    
    public static void Set()
    {
        tasks = new List<Task>();
        dataHandlers = new List<CWorldDataHandler>();
        
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(null);
            dataHandlers.Add(new CWorldDataHandler());
        }
    }

    public static void Clear()
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes.Clear();
            dataHandlers[i].biomeNodes.Clear();
        }
    }

    public static void SetupSamplePool(string sampleName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].SetupSamplePool(sampleName);
        }
    }

    public static bool AddSamples(string name)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryAdd(name, new CWorldSampleNode(name)))
                return false;
        }

        currentSampleName = name;

        return true;
    }
    
    public static bool AddModifier(string name)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].modifierNodes.TryAdd(name, new CWorldModifierNode(name)))
                return false;
        }

        currentModifierName = name;

        return true;
    }
    
    public static bool AddBiomes(string name)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].biomeNodes.TryAdd(name, new CWorldBiomeNode(name)))
                return false;
        }

        currentBiomeName = name;

        return true;
    }

    public static bool AddMap(string nextLine)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (nextLine.Equals("Force") || dataHandlers[i].MapNode == null)
            {
                dataHandlers[i].MapNode = new CWorldMapNode();
            }
            else
                return false;
        }
        
        return true;
    }

    public static bool AddSampleOverrideAdd(string sampleToAdd)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                return false;
            
            dataHandlers[i].sampleNodes[currentSampleName].overrideNode.add.Add(sampleNode);
        }

        return true;
    }
    
    public static bool AddSampleOverrideMultiply(string sampleToAdd)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                return false;
            
            dataHandlers[i].sampleNodes[currentSampleName].overrideNode.multiply.Add(sampleNode);
        }

        return true;
    }
    
    public static bool AddSampleOverrideSubtract(string sampleToAdd)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                return false;
            
            dataHandlers[i].sampleNodes[currentSampleName].overrideNode.subtract.Add(sampleNode);
        }

        return true;
    }


    public static bool AddSampleNoiseParameter(string type, Vector2 floats)
    {
        switch (type)
        {
            case "clamp":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                        .Add(new CWOPClampNode(floats.x, floats.y));
                }
                break;
            }
            
            case "lerp":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                        .Add(new CWOPLerpNode(floats.x, floats.y));
                }
                break;
            }
            
            case "slide":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                        .Add(new CWOPSlideNode(floats.x, floats.y));
                }
                break;
            }
            
            case "smooth":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                        .Add(new CWOPSmoothNode(floats.x, floats.y));
                }
                break;
            }
            
            case "ignore":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                        .Add(new CWOPIgnoreNode(floats.x, floats.y));
                }
                break;
            }
            
            default:
                return false;
        }

        return true;
    }

    public static bool AddSampleOverrideParameter(string type, Vector2 floats)
    {
        switch (type)
        {
            case "clamp":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                        .Add(new CWOPClampNode(floats.x, floats.y));
                }
                break;
            }
            
            case "lerp":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                        .Add(new CWOPLerpNode(floats.x, floats.y));
                }
                break;
            }
            
            case "slide":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                        .Add(new CWOPSlideNode(floats.x, floats.y));
                }
                break;
            }
            
            case "smooth":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                        .Add(new CWOPSmoothNode(floats.x, floats.y));
                }
                break;
            }
            
            case "ignore":
            {
                for (int i = 0; i < threadCount; i++)
                {
                    dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                        .Add(new CWOPIgnoreNode(floats.x, floats.y));
                }
                break;
            }
            
            default:
                return false;
        }

        return true;
    }
    
    public static void SetSampleNoiseSize(Vector2 floats)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
            noiseNode.sizeX = floats.x;
            noiseNode.sizeY = floats.y;
        }
    }
    
    public static void SetSampleNoiseOffset(Vector2 floats)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
            noiseNode.offsetX = floats.x;
            noiseNode.offsetY = floats.y;
        }
    }

    public static void SetSampleNoiseAmplitude(float value)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes[currentSampleName].noiseNode.amplitude = value;
        }
    }

    public static void SetSampleNoiseInvert(bool value = true)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes[currentSampleName].noiseNode.invert = value;
        }
    }

    public static void SetBiomeSampleRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldSampleNode sampleNode = dataHandlers[i].sampleNodes[currentSampleName];
            sampleNode.min_height = ints.x;
            sampleNode.max_height = ints.y;
        }
    }
    
    public static void SetSampleFlip(bool value = true)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes[currentSampleName].flip = value;
        }
    }
    
    public static void SetSampleOverrideInvert(bool value = true)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes[currentSampleName].overrideNode.invert = value;
        }
    }



    public static bool SetModifierSample(string sampleName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                return false;

            dataHandlers[i].modifierNodes[currentModifierName].sample = sampleNode;
        }

        return true;
    }
    
    public static void SetModifierRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldModifierNode modifierNode = dataHandlers[i].modifierNodes[currentModifierName];
            modifierNode.range.min = ints.x;
            modifierNode.range.max = ints.y;
        }
    }
    
    public static void SetModifierIgnore(Vector2 floats)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldModifierNode modifierNode = dataHandlers[i].modifierNodes[currentModifierName];
            modifierNode.ignore = new FloatRangeNode();
            modifierNode.ignore.min = floats.x;
            modifierNode.ignore.max = floats.y;
        }
    }

    public static void SetModifierInvert(bool value = false)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].modifierNodes[currentModifierName].invert = value;
        }
    }

    public static void AddModifierGen()
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].modifierNodes[currentModifierName].gen.Add(new CWorldModifierGenNode());
        }
    }

    public static bool SetModifierGenSample(string sampleName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                return false;

            dataHandlers[i].modifierNodes[currentModifierName].gen[^1].sample = sampleNode;
        }

        return true;
    }

    public static void SetModifierGenRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldModifierGenNode genNode = dataHandlers[i].modifierNodes[currentModifierName].gen[^1];
            genNode.range = new IntRangeNode();
            genNode.range.min = ints.x;
            genNode.range.max = ints.y;
        }
    }

    public static void SetModifierGenOffset(int value)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].modifierNodes[currentModifierName].gen[^1].offset = value;
        }
    }

    public static void SetModifierGenFlip(bool value = false)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].modifierNodes[currentModifierName].gen[^1].flip = value;
        }
    }
    
    
    

    public static void SetBiomeSequence(CWOCSequenceNode sequenceNode)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].biomeNodes[currentBiomeName].SequenceNodes.Add(sequenceNode);
        }
    }

    public static bool SetBiomeSample(string sampleName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                return false;

            dataHandlers[i].biomeNodes[currentBiomeName].sample = sampleNode;
        }

        return true;
    }

    public static bool SetBiomeModifier(string modifierName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].modifierNodes.TryGetValue(modifierName, out var modifierNode))
                return false;
            
            dataHandlers[i].biomeNodes[currentBiomeName].modifier = modifierNode;
        }

        return true;
    }

    public static void SetBiomeRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            CWorldBiomeNode biomeNode = dataHandlers[i].biomeNodes[currentBiomeName];
            biomeNode.sampleRange.min = ints.x;
            biomeNode.sampleRange.max = ints.y;
        }
    }

    public static bool SetMapBiomeRange(string biomeName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].biomeNodes.TryGetValue(biomeName, out var biomeNode))
                return false;
            
            dataHandlers[i].MapNode.biomePool.Add(new BiomePool(biomeNode));
        }

        return true;
    }

    public static bool SetMapSampleRange(string value, Vector2 floats)
    {
        for (int i = 0; i < threadCount; i++)
        {
            if (!dataHandlers[i].sampleNodes.TryGetValue(value, out var sampleNode) || dataHandlers[i].MapNode == null)
                return false;
            
            dataHandlers[i].MapNode.biomePool[^1].samples.Add(sampleNode.name, new BiomePoolSample(sampleNode));
                
            dataHandlers[i].MapNode.biomePool[^1].samples[sampleNode.name].min = floats.x;
            dataHandlers[i].MapNode.biomePool[^1].samples[sampleNode.name].max = floats.y;
        }

        return true;
    }
}