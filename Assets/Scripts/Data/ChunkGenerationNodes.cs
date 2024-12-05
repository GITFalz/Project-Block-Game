using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public static class ChunkGenerationNodes
{
    public static List<Task> tasks;
    public static List<CWorldDataHandler> dataHandlers;

    public static bool localLoad = false;
    public static CWorldDataHandler localDataHandler;

    public static string currentSampleName = "";
    public static string currentBiomeName = "";
    public static string sampleDisplayName = "";
    public static string currentModifierName = "";
    public static string currentLinkName = "";
    public static string currentFoliageName = "";

    public static int threadCount = 6;
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
        if (dataHandlers == null || dataHandlers.Count == 0) 
            return;
        
        for (int i = 0; i < threadCount; i++)
        {
            dataHandlers[i].sampleNodes.Clear();
            dataHandlers[i].biomeNodes.Clear();
            dataHandlers[i].modifierNodes.Clear();
            dataHandlers[i].linkNodes.Clear();
            dataHandlers[i].foliageNodes.Clear();
            dataHandlers[i].MapNode = null;
        }

        currentSampleName = "";
        currentBiomeName = "";
        sampleDisplayName = "";
        currentModifierName = "";
        currentLinkName = "";
        currentFoliageName = "";
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
            currentSampleName = name;
            
            for (int i = 0; i < threadCount; i++)
            {
                if (!dataHandlers[i].sampleNodes.TryAdd(name, new CWorldSampleNode(name)))
                {
                    Console.Log("Failed to add sample node");
                    return false;
                }

                GenerateSampleNode(dataHandlers[i].sampleNodes[name], dataHandlers[i]);
            }
            
            return true;
        });
    }

    public static CWorldSampleNode GenerateSampleNode(CWorldSampleNode sampleNode, CWorldDataHandler dataHandler)
    {
        sampleNode.overrideNode = new CWOSOverrideNode();
        sampleNode.noiseNode = new CWorldNoiseNode();

        sampleNode.flip = CWorldSampleManager.flip;
        sampleNode.min_height = CWorldSampleManager.min_height;
        sampleNode.max_height = CWorldSampleManager.max_height;
        
        foreach (var data in CWorldSampleManager.noiseSampleData)
        {
            switch (data.type)
            {
                case "clamp":
                    sampleNode.noiseNode.parameters.Add(new CWOPClampNode(data.floats.x, data.floats.y));
                    break;
                case "lerp":
                    sampleNode.noiseNode.parameters.Add(new CWOPLerpNode(data.floats.x, data.floats.y));
                    break;
                case "slide":
                    sampleNode.noiseNode.parameters.Add(new CWOPSlideNode(data.floats.x, data.floats.y));
                    break;
                case "smooth":
                    sampleNode.noiseNode.parameters.Add(new CWOPSmoothNode(data.floats.x, data.floats.y));
                    break;
                case "ignore":
                    sampleNode.noiseNode.parameters.Add(new CWOPIgnoreNode(data.floats.x, data.floats.y));
                    break;
                default:
                    continue;
            }
        }

        sampleNode.noiseNode.sizeX = CWorldSampleManager.noiseSize.x;
        sampleNode.noiseNode.sizeY = CWorldSampleManager.noiseSize.y;

        sampleNode.noiseNode.offsetX = CWorldSampleManager.noiseOffset.x;
        sampleNode.noiseNode.offsetY = CWorldSampleManager.noiseOffset.y;

        sampleNode.noiseNode.amplitude = CWorldSampleManager.noiseAmplitude;
        sampleNode.noiseNode.invert = CWorldSampleManager.noiseInvert;

        foreach (var data in CWorldSampleManager.sampleOverrideData)
        {
            switch (data.type)
            {
                case OverrideType.Add:
                    sampleNode.overrideNode.modifiers.Add(new AddModifier { sample = dataHandler.sampleNodes[data.name] });
                    break;
                case OverrideType.Mul:
                    sampleNode.overrideNode.modifiers.Add(new MultiplyModifier { sample = dataHandler.sampleNodes[data.name] });
                    break;
                case OverrideType.Sub:
                    sampleNode.overrideNode.modifiers.Add(new SubtractModifier { sample = dataHandler.sampleNodes[data.name] });
                    break;
                default:
                    continue;
            }
        }
        
        foreach (var data in CWorldSampleManager.overrideSampleData)
        {
            switch (data.type)
            {
                case "clamp":
                    sampleNode.overrideNode.parameters.Add(new CWOPClampNode(data.floats.x, data.floats.y));
                    break;
                case "lerp":
                    sampleNode.overrideNode.parameters.Add(new CWOPLerpNode(data.floats.x, data.floats.y));
                    break;
                case "slide":
                    sampleNode.overrideNode.parameters.Add(new CWOPSlideNode(data.floats.x, data.floats.y));
                    break;
                case "smooth":
                    sampleNode.overrideNode.parameters.Add(new CWOPSmoothNode(data.floats.x, data.floats.y));
                    break;
                case "ignore":
                    sampleNode.overrideNode.parameters.Add(new CWOPIgnoreNode(data.floats.x, data.floats.y));
                    break;
                default:
                    continue;
            }
        }

        sampleNode.overrideNode.invert = CWorldSampleManager.overrideInvert;

        return sampleNode;
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
                
                CWorldFoliageNode foliageNode = dataHandlers[i].foliageNodes[name];
                
                if (dataHandlers[i].sampleNodes.TryGetValue(CWorldFoliageManager.samplerName, out var sampleNode))
                    foliageNode.sampler = new TreeSample { sampleNode = sampleNode };
                else
                    if (dataHandlers[i].modifierNodes.TryGetValue(CWorldFoliageManager.samplerName, out var modifierNode))
                        foliageNode.sampler = new TreeModifier { modifierNode = modifierNode };
                    else
                        foliageNode.sampler = new TreeBasic();

                
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
                    
                    foliageNode.trunk = linkNode;
                }
                
                foreach (var direction in CWorldFoliageManager.directions)
                {
                    IDirection dir;
                    
                    switch (direction.direction)
                    {
                        case "forward":
                            dir = new ForwardRotate { angle = new AngleRange(direction.values) };
                            break;
                        case "backward":
                            dir = new BackwardsRotate { angle = new AngleRange(direction.values) };
                            break;
                        case "right":
                            dir = new RightRotate { angle = new AngleRange(direction.values) };
                            break;
                        case "left":
                            dir = new LeftRotate { angle = new AngleRange(direction.values) };
                            break;
                        case "up":
                            dir = new UpRotate { angle = new AngleRange(direction.values) };
                            break;
                        case "down":
                            dir = new DownRotate { angle = new AngleRange(direction.values) };
                            break;
                        default:
                            continue;
                    }

                    foliageNode.directions.Add(dir);
                }
                
                foliageNode.lengthRange = new IntRangeNode(CWorldFoliageManager.lengthRange);
                foliageNode.branchAmount = new IntRangeNode(CWorldFoliageManager.branchAmount);
                foliageNode.branchLengthRange = new IntRangeNode(CWorldFoliageManager.branchLengthRange);
                foliageNode.angleRange = new FloatRangeNode(CWorldFoliageManager.angleRange);
                foliageNode.thresholdRange = new FloatRangeNode(CWorldFoliageManager.thresholdRange);
                foliageNode.verticalAngle = new FloatRangeNode(CWorldFoliageManager.verticalAngle);
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
                
                dataHandlers[i].MapNode.AddBiomeSample(sampleNode, new FloatRangeNode(floats));
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