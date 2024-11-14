using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CWorldLinkManager
{
    public static CWorldLinkManager instance;
    public CWorldBlock BlockNode;

    public int test = 0;

    public CWorldLinkManager()
    {
        if (instance == null) instance = this;
    }
    
    private CWOCSequenceNode _sequenceNode;

    public void SetBlock(CWorldBlock block)
    {
        BlockNode = block;
    }
    
    public Dictionary<string, Func<WMWriter, Task<int>>> labels = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "(", (w) => w.Increment(1, 0) },
        { "name", (w) => w.On_Name(ref w.writerManager.currentLinkName) },
        { ")", (w) => w.Increment(1, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> settings = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "A", async (w) => },
        { "B", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> A = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "start", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> B = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        { "A", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
}