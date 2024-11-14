
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CWorldTreeManager
{
    public static CWorldTreeManager instance;

    public CWorldTreeManager()
    {
        if (instance == null) instance = this;
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
        //{ "link", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
    
    public Dictionary<string, Func<WMWriter, Task<int>>> link = new Dictionary<string, Func<WMWriter, Task<int>>>()
    {
        { "{", (w) => w.Increment(1, 0) },
        //{ "sample", async (w) => },
        //{ "link", async (w) => },
        //{ "range", async (w) => },
        { "}", (w) => w.Increment(0, 1) },
    };
}