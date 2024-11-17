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
    public static string currentLinkName = "";
    public static string currentTreeName = "";

    public static int threadCount = 4;
    public static bool set = true;
    
    
    public static void Set()
    {
        if (!set) return;
        
        tasks = new List<Task>();
        dataHandlers = new List<CWorldDataHandler>();
        
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(null);
            dataHandlers.Add(new CWorldDataHandler());
        }

        set = false;
    }

    public static void Clear()
    {
        Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes.Clear();
                dataHandlers[i].biomeNodes.Clear();
            }

            currentSampleName = "";
            currentBiomeName = "";
            sampleDisplayName = "";
            currentModifierName = "";
        });
    }

    public static void SetupSamplePool(string sampleName)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].SetupSamplePool(sampleName);
        }
    }

    public static Task<bool> AddSamples(string name)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryAdd(name, new CWorldSampleNode(name)))
                    return false;
            }

            currentSampleName = name;

            return true;
        });
    }
    
    public static Task<bool> AddModifier(string name)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].modifierNodes.TryAdd(name, new CWorldModifierNode(name)))
                    return false;
            }

            currentModifierName = name;
            return true;
        });
    }
    
    public static Task<bool> AddLink(string name)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].linkNodes.TryAdd(name, new CWorldLinkNode(name)))
                    return false;
            }

            currentLinkName = name;
            return true;
        });
    }

    public static Task<bool> AddBiomes(string name)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].biomeNodes.TryAdd(name, new CWorldBiomeNode(name)))
                    return false;
            }

            currentBiomeName = name;
            return true;
        });
    }
    
    public static Task<bool> AddTree(string name)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].treeNodes.TryAdd(name, new CWorldTreeNode(name)))
                    return false;
            }

            currentTreeName = name;
            return true;
        });
    }

    public static Task<bool> AddMap(string nextLine)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (nextLine.Trim().Equals("Force") || dataHandlers[i].MapNode == null)
                {
                    dataHandlers[i].MapNode = new CWorldMapNode();
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        });
    }

    public static Task<bool> AddSampleOverrideAdd(string sampleToAdd)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                    return false;

                dataHandlers[i].sampleNodes[currentSampleName].overrideNode.add.Add(sampleNode);
            }

            return true;
        });
    }

    public static Task<bool> AddSampleOverrideMultiply(string sampleToAdd)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                    return false;

                dataHandlers[i].sampleNodes[currentSampleName].overrideNode.multiply.Add(sampleNode);
            }

            return true;
        });
    }

    public static Task<bool> AddSampleOverrideSubtract(string sampleToAdd)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleToAdd, out var sampleNode))
                    return false;

                dataHandlers[i].sampleNodes[currentSampleName].overrideNode.subtract.Add(sampleNode);
            }

            return true;
        });
    }

    public static Task<bool> AddSampleNoiseParameter(string type, Vector2 floats)
    {
        return Task.Run(() =>
        {
            switch (type)
            {
                case "clamp":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                            .Add(new CWOPClampNode(floats.x, floats.y));
                    }
                    break;

                case "lerp":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                            .Add(new CWOPLerpNode(floats.x, floats.y));
                    }
                    break;

                case "slide":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                            .Add(new CWOPSlideNode(floats.x, floats.y));
                    }
                    break;

                case "smooth":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                            .Add(new CWOPSmoothNode(floats.x, floats.y));
                    }
                    break;

                case "ignore":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].noiseNode.parameters
                            .Add(new CWOPIgnoreNode(floats.x, floats.y));
                    }
                    break;

                default:
                    return false;
            }

            return true;
        });
    }


        public static Task<bool> AddSampleOverrideParameter(string type, Vector2 floats)
    {
        return Task.Run(() =>
        {
            switch (type)
            {
                case "clamp":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                            .Add(new CWOPClampNode(floats.x, floats.y));
                    }
                    break;
                case "lerp":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                            .Add(new CWOPLerpNode(floats.x, floats.y));
                    }
                    break;
                case "slide":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                            .Add(new CWOPSlideNode(floats.x, floats.y));
                    }
                    break;
                case "smooth":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                            .Add(new CWOPSmoothNode(floats.x, floats.y));
                    }
                    break;
                case "ignore":
                    for (int i = 0; i < threadCount; i++)
                    {
                        dataHandlers[i].sampleNodes[currentSampleName].overrideNode.parameters
                            .Add(new CWOPIgnoreNode(floats.x, floats.y));
                    }
                    break;
                default:
                    return false;
            }

            return true;
        });
    }

    public static Task SetSampleNoiseSize(Vector2 floats)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
                noiseNode.sizeX = floats.x;
                noiseNode.sizeY = floats.y;
            }
        });
    }

    public static Task SetSampleNoiseOffset(Vector2 floats)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
                noiseNode.offsetX = floats.x;
                noiseNode.offsetY = floats.y;
            }
        });
    }

    public static Task SetSampleNoiseAmplitude(float value)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].noiseNode.amplitude = value;
            }
        });
    }

    public static Task SetSampleNoiseInvert(bool value = true)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].noiseNode.invert = value;
            }
        });
    }

    public static Task SetBiomeSampleRange(Vector2Int ints)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldSampleNode sampleNode = dataHandlers[i].sampleNodes[currentSampleName];
                sampleNode.min_height = ints.x;
                sampleNode.max_height = ints.y;
            }
        });
    }

    public static Task SetSampleFlip(bool value = true)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].flip = value;
            }
        });
    }

    public static Task SetSampleOverrideInvert(bool value = true)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].overrideNode.invert = value;
            }
        });
    }




    public static Task<bool> SetModifierSample(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].modifierNodes[currentModifierName].sample = sampleNode;
            }

            return true;
        });
    }
    
    public static Task SetModifierRange(Vector2Int ints)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldModifierNode modifierNode = dataHandlers[i].modifierNodes[currentModifierName];
                modifierNode.range.min = ints.x;
                modifierNode.range.max = ints.y;
            }
        });
    }
    
    public static Task SetModifierIgnore(Vector2 floats)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldModifierNode modifierNode = dataHandlers[i].modifierNodes[currentModifierName];
                modifierNode.ignore = new FloatRangeNode();
                modifierNode.ignore.min = floats.x;
                modifierNode.ignore.max = floats.y;
            }
        });
    }

    public static Task SetModifierInvert(bool value = false)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].invert = value;
            }
        });
    }

    public static Task AddModifierGen()
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen.Add(new CWorldModifierGenNode());
            }
        });
    }

    public static Task<bool> SetModifierGenSample(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].modifierNodes[currentModifierName].gen[^1].sample = sampleNode;
            }

            return true;
        });
    }

    public static Task SetModifierGenRange(Vector2Int ints)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldModifierGenNode genNode = dataHandlers[i].modifierNodes[currentModifierName].gen[^1];
                genNode.range = new IntRangeNode();
                genNode.range.min = ints.x;
                genNode.range.max = ints.y;
            }
        });
    }

    public static Task SetModifierGenOffset(int value)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen[^1].offset = value;
            }
        });
    }

    public static Task SetModifierGenFlip(bool value = false)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen[^1].flip = value;
            }
        });
    }
    
    
    

    public static Task SetBiomeSequence(CWOCSequenceNode sequenceNode)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].biomeNodes[currentBiomeName].SequenceNodes.Add(sequenceNode);
            }
        });
    }

    public static Task<bool> SetBiomeSample(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].biomeNodes[currentBiomeName].sample = sampleNode;
            }

            return true;
        });
    }

    public static Task<bool> SetBiomeModifier(string modifierName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].modifierNodes.TryGetValue(modifierName, out var modifierNode))
                    return false;

                dataHandlers[i].biomeNodes[currentBiomeName].modifier = modifierNode;
            }

            return true;
        });
    }

    public static Task<bool> SetBiomeTree(string treeName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].treeNodes.TryGetValue(treeName, out var treeNode))
                    return false;

                dataHandlers[i].biomeNodes[currentBiomeName].treeNode = treeNode;
            }

            return true;
        });
    }
    
    public static Task<bool> SetBiomeTreeSample(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].biomeNodes[currentBiomeName].treeSampleNode = sampleNode;
            }

            return true;
        });
    }
    
    public static Task SetBiomeTreeSampleRange(Vector2 value)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].biomeNodes[currentBiomeName].treeRange = new FloatRangeNode(value.x, value.y);
            }
        });
    }

    public static Task SetBiomeRange(Vector2Int ints)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldBiomeNode biomeNode = dataHandlers[i].biomeNodes[currentBiomeName];
                biomeNode.sampleRange.min = ints.x;
                biomeNode.sampleRange.max = ints.y;
            }
        });
    }

    public static Task<bool> SetMapBiomeRange(string biomeName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].biomeNodes.TryGetValue(biomeName, out var biomeNode))
                    return false;

                dataHandlers[i].MapNode.biomePool.Add(new BiomePool(biomeNode));
            }

            return true;
        });
    }

    public static Task<bool> SetMapSampleRange(string value, Vector2 floats)
    {
        return Task.Run(() =>
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
        });
    }



    public static void SetLinkAPosition(Vector3Int position, bool overwrite = false)
    {
        for (int i = 0; i < threadCount; i++)
        {
            SetLinkPointPosition(dataHandlers[i].linkNodes[currentLinkName].A, position, overwrite);
        }
    }
    
    public static void SetLinkBPosition(Vector3Int position, bool overwrite = false)
    {
        for (int i = 0; i < threadCount; i++)
        {
            SetLinkPointPosition(dataHandlers[i].linkNodes[currentLinkName].B, position, overwrite);
        }
    }

    public static void SetLinkPointPosition(CWorldLinkPoint point, Vector3Int position, bool overwrite = false)
    {
        if (overwrite)
            point.position = position;
        else
        {
            point.position.x = position.x != 0 ? position.x : point.position.x;
            point.position.y = position.y != 0 ? position.y : point.position.y;
            point.position.z = position.z != 0 ? position.z : point.position.z;
        }
    }
    
    
    
    
    public static Task<bool> SetTreeSample(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].treeNodes[currentTreeName].sampler = new TreeSample { sampleNode = sampleNode };
            }

            Console.Log("Sample was found");
            return true;
        });
    }
    
    public static Task<bool> SetTreeModifier(string sampleName)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].modifierNodes.TryGetValue(sampleName, out var sampleNode))
                    return false;

                dataHandlers[i].treeNodes[currentTreeName].sampler = new TreeModifier { modifierNode = sampleNode };
            }

            Console.Log("Modifier was found");
            return true;
        });
    }
    
    public static Task SetTreeBasic()
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].treeNodes[currentTreeName].sampler = new TreeBasic();
            }

            return true;
        });
    }

    public static Task SetTreeRange(Vector2Int ints)
    {
        return Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].treeNodes[currentTreeName].range = new IntRangeNode(ints.x, ints.y);
            }

            return true;
        });
    }
}