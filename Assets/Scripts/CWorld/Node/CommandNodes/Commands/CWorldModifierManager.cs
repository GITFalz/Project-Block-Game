using System;
using System.Collections.Generic;

public class CWorldModifierManager
{
    public static CWorldModifierManager instance;
    public CWorldMapNode MapNode;

    public CWorldModifierManager()
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
        { "sample", (w) => },
        { "range", (w) => },
        { "ignore", (w) => },
        { "invert", (w) => },
        { "gen", (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, int>> biomeRanges = new Dictionary<string, Func<WMWriter, int>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "sample", (w) =>  },
        { "offset", (w) =>  },
        { "flip", (w) =>  },
        { "}", (w) => w.Increment(1, 1) },
    };
}