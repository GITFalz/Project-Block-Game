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
    public static string currentFoliageName = "";

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

    public static async Task<bool> AddSamples(string name)
    {
        return await Task.Run(() =>
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
    
    public static async Task<bool> AddModifier(string name)
    {
        return await Task.Run(() =>
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
    
    public static async Task<bool> AddLink(string name)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> AddBiomes(string name)
    {
        return await Task.Run(() =>
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
    
    public static async Task<bool> AddFoliage(string name)
    {
        return await Task.Run(() =>
        {
            currentFoliageName = name;
            
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].foliageNodes.TryAdd(name, new CWorldFoliageNode(name)))
                {
                    Console.Log("Failed to add foliage node");
                    return false;
                }
                
                if (dataHandlers[i].sampleNodes.TryGetValue(CWorldFoliageManager.samplerName, out var sampleNode))
                    dataHandlers[i].foliageNodes[currentFoliageName].sampler = new TreeSample { sampleNode = sampleNode };
                else
                    if (dataHandlers[i].modifierNodes.TryGetValue(CWorldFoliageManager.samplerName, out var modifierNode))
                        dataHandlers[i].foliageNodes[currentFoliageName].sampler = new TreeModifier { modifierNode = modifierNode };
                    else
                        dataHandlers[i].foliageNodes[currentFoliageName].sampler = new TreeBasic();

                
                string trunkName = CWorldFoliageManager.trunkName;
                if (!dataHandlers[i].linkNodes.TryGetValue(trunkName, out var linkNode))
                {
                    Console.Log("The link node does not exist, creating a new one");
                    dataHandlers[i].linkNodes.Add(trunkName, new CWorldLinkNode(trunkName));
                    
                    if (!dataHandlers[i].linkNodes.TryGetValue(trunkName, out linkNode))
                    {
                        Console.Log("Failed to create link node");
                        return false;
                    }
                    
                    dataHandlers[i].foliageNodes[currentFoliageName].trunk = linkNode;
                }
                
                foreach (var direction in CWorldFoliageManager.directions)
                {
                    switch (direction.direction)
                    {
                        case "forward":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new ForwardRotate { angle = new AngleRange(direction.values) });
                            break;
                        case "backward":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new BackwardsRotate { angle = new AngleRange(direction.values) });
                            break;
                        case "right":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new RightRotate { angle = new AngleRange(direction.values) });
                            break;
                        case "left":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new LeftRotate { angle = new AngleRange(direction.values) });
                            break;
                        case "up":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new UpRotate { angle = new AngleRange(direction.values) });
                            break;
                        case "down":
                            dataHandlers[i].foliageNodes[currentFoliageName].directions.Add(new DownRotate { angle = new AngleRange(direction.values) });
                            break;
                    }
                }
                
                dataHandlers[i].foliageNodes[currentFoliageName].lengthRange = new IntRangeNode(CWorldFoliageManager.lengthRange);
                dataHandlers[i].foliageNodes[currentFoliageName].branchAmount = new IntRangeNode(CWorldFoliageManager.branchAmount);
                dataHandlers[i].foliageNodes[currentFoliageName].branchLengthRange = new IntRangeNode(CWorldFoliageManager.branchLengthRange);
                dataHandlers[i].foliageNodes[currentFoliageName].angleRange = new FloatRangeNode(CWorldFoliageManager.angleRange);
                dataHandlers[i].foliageNodes[currentFoliageName].thresholdRange = new FloatRangeNode(CWorldFoliageManager.thresholdRange);
                dataHandlers[i].foliageNodes[currentFoliageName].verticalAngle = new FloatRangeNode(CWorldFoliageManager.verticalAngle);
            }
            
            return true;
        });
    }

    public static async Task<bool> AddMap(string nextLine)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> AddSampleOverrideAdd(string sampleToAdd)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> AddSampleOverrideMultiply(string sampleToAdd)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> AddSampleOverrideSubtract(string sampleToAdd)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> AddSampleNoiseParameter(string type, Vector2 floats)
    {
        return await Task.Run(() =>
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


        public static async Task<bool> AddSampleOverrideParameter(string type, Vector2 floats)
    {
        return await Task.Run(() =>
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

    public static async Task SetSampleNoiseSize(Vector2 floats)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
                noiseNode.sizeX = floats.x;
                noiseNode.sizeY = floats.y;
            }
        });
    }

    public static async Task SetSampleNoiseOffset(Vector2 floats)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldNoiseNode noiseNode = dataHandlers[i].sampleNodes[currentSampleName].noiseNode;
                noiseNode.offsetX = floats.x;
                noiseNode.offsetY = floats.y;
            }
        });
    }

    public static async Task SetSampleNoiseAmplitude(float value)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].noiseNode.amplitude = value;
            }
        });
    }

    public static async Task SetSampleNoiseInvert(bool value = true)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].noiseNode.invert = value;
            }
        });
    }

    public static async Task SetBiomeSampleRange(Vector2Int ints)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldSampleNode sampleNode = dataHandlers[i].sampleNodes[currentSampleName];
                sampleNode.min_height = ints.x;
                sampleNode.max_height = ints.y;
            }
        });
    }

    public static async Task SetSampleFlip(bool value = true)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].flip = value;
            }
        });
    }

    public static async Task SetSampleOverrideInvert(bool value = true)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].sampleNodes[currentSampleName].overrideNode.invert = value;
            }
        });
    }




    public static async Task<bool> SetModifierSample(string sampleName)
    {
        return await Task.Run(() =>
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
    
    public static async Task SetModifierRange(Vector2Int ints)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldModifierNode modifierNode = dataHandlers[i].modifierNodes[currentModifierName];
                modifierNode.range.min = ints.x;
                modifierNode.range.max = ints.y;
            }
        });
    }
    
    public static async Task SetModifierIgnore(Vector2 floats)
    {
        await Task.Run(() =>
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

    public static async Task SetModifierInvert(bool value = false)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].invert = value;
            }
        });
    }

    public static async Task AddModifierGen()
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen.Add(new CWorldModifierGenNode());
            }
        });
    }

    public static async Task<bool> SetModifierGenSample(string sampleName)
    {
        return await Task.Run(() =>
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

    public static async Task SetModifierGenRange(Vector2Int ints)
    {
        await Task.Run(() =>
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

    public static async Task SetModifierGenOffset(int value)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen[^1].offset = value;
            }
        });
    }

    public static async Task SetModifierGenFlip(bool value = false)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].modifierNodes[currentModifierName].gen[^1].flip = value;
            }
        });
    }
    
    
    

    public static async Task SetBiomeSequence(CWOCSequenceNode sequenceNode)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].biomeNodes[currentBiomeName].SequenceNodes.Add(sequenceNode);
            }
        });
    }

    public static async Task<bool> SetBiomeSample(string sampleName)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> SetBiomeModifier(string modifierName)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> SetBiomeFoliage(string treeName)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].foliageNodes.TryGetValue(treeName, out var foliageNode))
                    return false;

                dataHandlers[i].biomeNodes[currentBiomeName].foliageNode = foliageNode;
            }

            return true;
        });
    }
    
    public static async Task<bool> SetBiomeTreeSample(string sampleName)
    {
        return await Task.Run(() =>
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
    
    public static async Task SetBiomeTreeSampleRange(Vector2 value)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].biomeNodes[currentBiomeName].treeRange = new FloatRangeNode(value.x, value.y);
            }
        });
    }

    public static async Task SetBiomeRange(Vector2Int ints)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                CWorldBiomeNode biomeNode = dataHandlers[i].biomeNodes[currentBiomeName];
                biomeNode.sampleRange.min = ints.x;
                biomeNode.sampleRange.max = ints.y;
            }
        });
    }

    public static async Task<bool> SetMapBiomeRange(string biomeName)
    {
        return await Task.Run(() =>
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

    public static async Task<bool> SetMapSampleRange(string value, Vector2 floats)
    {
        return await Task.Run(() =>
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
        /*
        for (int i = 0; i < threadCount; i++)
        {
            SetLinkPointPosition(dataHandlers[i].linkNodes[currentLinkName].A, position, overwrite);
        }
        */
    }
    
    public static void SetLinkAxRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].A.X = SetLinkPointRange(ints);
        }
    }
    
    public static void SetLinkAyRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].A.Y = SetLinkPointRange(ints);
        }
    }
    
    public static void SetLinkAzRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].A.Z = SetLinkPointRange(ints);
        }
    }
    
    public static void SetLinkBxRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].B.X = SetLinkPointRange(ints);
        }
    }
    
    public static void SetLinkByRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].B.Y = SetLinkPointRange(ints);
        }
    }
    
    public static void SetLinkBzRange(Vector2Int ints)
    {
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].linkNodes[currentLinkName].B.Z = SetLinkPointRange(ints);
        }
    }

    public static IPoint SetLinkPointRange(Vector2Int ints)
    {
        return new PointRange { range = new IntRangeNode(ints.x, ints.y) };
    }
    
    public static void SetLinkBPosition(Vector3Int position, bool overwrite = false)
    {
        /*
        for (int i = 0; i < threadCount; i++)
        {
            SetLinkPointPosition(dataHandlers[i].linkNodes[currentLinkName].B, position, overwrite);
        }
        */
    }

    /*
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
    */
    
    public static async Task<bool> SetLinkLink(string linkName)
    {
        return await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].linkNodes.TryGetValue(linkName, out var linkNode))
                    return false;

                linkNode.spikes.Add(dataHandlers[i].linkNodes[currentLinkName]);
            }
            
            return true;
        });
    }

    public static async Task SetLinkThreshold(float value)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < threadCount; i++)
            {
                dataHandlers[i].linkNodes[currentLinkName].threshold = value;
            }
        });
    }
}