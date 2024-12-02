using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class CWorldNodesManager
{
    public static Dictionary<string, Func<Task<int>>> Types = new Dictionary<string, Func<Task<int>>>()
    {
        { "Save", async () => await On_Save() },
        { "Link", async () => await On_Link() },
        { "Use", async () => await On_Use() },
        { "Sample", async () => await On_Sample() },
        { "Biome", async () => await On_Biome() },
        { "Block", async () => await On_Block() },
        { "Modifier", async () => await On_Modifier() },
        { "Map", async () => await On_Map() },
        { "Foliage", async () => await On_Foliage() },
    };
    
    private static int Index => CWorldCommandManager.Index;
    private static string[] Args => CWorldCommandManager.Args;
    private static int ArgsLength => CWorldCommandManager.ArgsLength;
    private static string Arg => CWorldCommandManager.Arg;
    private static string CurrentPath => CWorldCommandManager.CurrentPath;
    private static WriterManager WriterManager => CWorldCommandManager._writerManager;
    
    
    
    public static Task<int> On_Name(ref string name)
    {
        Increment();
        if (!Arg.Equals("=")) return Error("No '=' found");
        Increment();
        if (Arg.Equals(")")) return Error("'name' expected but ) found");
        name = Arg;
        Increment();
        return Task.FromResult(2);
    }
    
    public static async Task<int> On_Settings(Dictionary<string, Func<Task<int>>> commands)
    {
        Increment();
        return await CommandsTest(commands) == -1 ? await Error("Problem with the settings") : 0;
    }
    
    public static Task<int> On_Display()
    {
        Increment();
        ChunkGenerationNodes.sampleDisplayName = CWorldSampleManager.name;
        return Task.FromResult(0);
    }
    
    
    
    private static async Task<int> On_Save()
    {
        Increment();
        
        try
        {
            await CWorldEditorManager.Instance.SaveFileAsync();
        }
        catch (Exception e)
        {
            Console.Log("Saving file wont work");
        }
        
        return 1;
    }

    private static async Task<int> On_Use()
    {
        Increment();
        string[] path = CWorldCommandManager.Arg.Split(new[] { '/', '\'', '|', '>' }, StringSplitOptions.RemoveEmptyEntries);
        string mainPath = FileManager.EditorFolderPath;

        string consolePath = "";
        
        foreach (string p in path)
        {
            consolePath += $" {p}";
            mainPath = Path.Combine(mainPath, p);
        }
        
        Console.Log("Using: " + consolePath);
        
        mainPath += ".cworld";

        if (mainPath.Equals(CurrentPath))
            return await Error($"Infinite loop is occuring in {CurrentPath}");

        return await CWorldCommandManager.Load(mainPath);
    }
    
    private static async Task<int> On_Sample()
    {
        Increment();

        if (await GenerateSampleAsync() == -1) return -1;
        if (!ChunkGenerationNodes.localLoad)
        {
            if (!await ChunkGenerationNodes.AddSamples(CWorldSampleManager.name))
                return await Error($"An error occured creating the {CWorldSampleManager.name} sample node");
        }
        else 
        {
            CWorldDataHandler dataHandler = ChunkGenerationNodes.localDataHandler;
            if (dataHandler == null)
                return await Console.LogErrorAsync("Can't load locally without a data handler");
            
            string name = CWorldSampleManager.name;
            
            CWorldSampleNode sampleNode = new CWorldSampleNode(name);
            ChunkGenerationNodes.GenerateSampleNode(sampleNode, dataHandler);
            
            dataHandler.sampleNodes.Add(name, sampleNode);
        }
        
        CWorldSampleManager.Reset();
        
        return 0;
    }

    private static async Task<int> GenerateSampleAsync()
    {
        if (await CommandsTest(CWorldSampleManager.labels) == -1) return await Error("Problem in the label found");
        if (await CommandsTest(CWorldSampleManager.settings) == -1) return await Error("Problem in the sample settings found");
        return 0;
    }

    private static async Task<int> On_Biome()
    {
        Increment();
        
        if (await CommandsTest(CWorldBiomeManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddBiomes(CWorldBiomeManager.name))
                return await Error("name is used twice");
        
        if (await CommandsTest(CWorldBiomeManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    private static async Task<int> On_Block()
    {
        Increment();
        
        CWorldBlockManager.SetBlock(new CWorldBlock());
        
        if (await CommandsTest(CWorldBlockManager.labels) == -1) return await Error("Problem in the label found");

        CWorldBlockManager.BlockNode.blockName = CWorldBlockManager.name;
        
        if (await CommandsTest(CWorldBlockManager.settings) == -1) return await Error("Problem in the biome settings found");
        return 0;
    }
    
    private static async Task<int> On_Foliage()
    {
        Increment();
        
        if (await CommandsTest(CWorldFoliageManager.Labels) == -1) return await Error("Problem in the foliage label found");
        if (await CommandsTest(CWorldFoliageManager.Settings) == -1) return await Error("Problem in the foliage settings found");
        if (!await ChunkGenerationNodes.AddFoliage(CWorldFoliageManager.name))
            return await Error("An error occured creating the foliage node");
        
        return 0;
    }
    
    private static async Task<int> On_Map()
    {
        Increment();

        if (!await ChunkGenerationNodes.AddMap(WriterManager.NextLine()))
            return await Error("Watch out!, a map node already exists in the system. To replace the node write 'Map Force'");

        if (WriterManager.NextLine(1).Equals("Force"))
            Increment();
        
        Increment();
        
        if (await CommandsTest(CWorldMapManager.settings) == -1) return await Error("Problem in the map settings found");
        return 0;
    }
    
    private static async Task<int> On_Modifier()
    {
        Increment();
        
        if (await CommandsTest(CWorldModifierManager.labels) == -1) return await Error("Problem in the label found");
        if (!await ChunkGenerationNodes.AddModifier(CWorldModifierManager.name))
                return await Error("name is used twice");
        
        if (await CommandsTest(CWorldModifierManager.settings) == -1) return await Error("Problem in the modifier settings found");
        return 0;
    }
    
    private static async Task<int> On_Link()
    {
        Increment();
        
        if (await CommandsTest(CWorldLinkManager.labels) == -1) return await Error("Couldn't assign label to link");
        if (!await ChunkGenerationNodes.AddLink(CWorldLinkManager.name))
                return await Error("name is used twice");
        
        if (await CommandsTest(CWorldLinkManager.settings) == -1) return await Error("Problem in the link settings found");
        return 0;
    }
    
    
    public static async Task<int> On_BlockSetTextures()
    {
        int i = 0;
        while (true)
        {
            if (await CWorldCommandManager.GetNextInt(out int t) == -1)
                return await Error("A problem occured when trying to get the texture indexes, check if they are indeed integers");

            if (!BlockManager.SetUv(CWorldBlockManager.BlockNode.index, i, t))
                return await Error("A problem occured when trying set the uv texture index, too many indices could be the problem");

            i++;
            if (Arg.Equals(",")) continue;
            return 0;
        }
    }

    public static async Task<int> On_BlockSetPriority()
    {
        if (await CWorldCommandManager.GetNextInt(out int t) == -1)
            return await Error("A problem occured when trying to get the priority value, make sure it's an integer");

        if (!BlockManager.SetPriority(CWorldBlockManager.BlockNode.index, t))
            return await Error("Couldn't find block to set priority");

        return 0;
    }
    
    public static async Task<int> Error(string message)
    {
        return await Console.LogErrorAsync(message);
    }
    
    
    
    private static async Task<int> CommandsTest(Dictionary<string, Func<Task<int>>> commands)
    {
        return await CWorldCommandManager.CommandsTest(commands);
    }
    
    private static void Increment(int i = 1)
    {
        CWorldCommandManager.Increment(i);
    }
    
    private static void Increment(int i, int result)
    {
        CWorldCommandManager.Increment(i, result);
    }
}