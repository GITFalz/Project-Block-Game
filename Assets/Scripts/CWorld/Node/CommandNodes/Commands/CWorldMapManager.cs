using System;
using System.Collections.Generic;
using UnityEngine;

public class CWorldMapManager
{
    public static CWorldMapManager instance;
    public CWorldMapNode MapNode;

    public CWorldMapManager()
    {
        if (instance == null) instance = this;
    }
    
    public void SetMap(CWorldMapNode map)
    {
        MapNode = map;
    }
    
    public Dictionary<string, Func<WMWriter, int>> settings = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "biomeRange", (w) => w.On_Settings(w.writerManager.worldMapManager.biomeRanges)},
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "set", (w) => w.On_SetBiomeRange() },
        { "}", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> setRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "sample", (w) => w.On_SetSampleRange() },
        { "}", (w) => w.Increment(1, 1) },
    };
}