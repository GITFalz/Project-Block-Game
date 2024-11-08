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
    }
    
    public float GetSampleNoise(int x, int y, int z, CWorldSampleNode i)
    {
        Init(x, y, z);
        return i.GetNoise();
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