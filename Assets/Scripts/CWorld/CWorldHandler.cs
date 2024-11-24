using System.Collections.Generic;
using UnityEngine;

public class CWorldHandler : MonoBehaviour
{
    public static CWorldMapNode MapNode;

    private bool test = true;
    
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