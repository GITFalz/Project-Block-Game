using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class World : MonoBehaviour
{
    public Transform player;
    public GameObject chunkPrefab;
    private bool isGeneratingChunk = false;
    public bool generate;

    public int renderDistance = 3;

    public Chunk chunkGeneratorScript;
    public WorldRenderer worldRenderer;

    public int chunkCount = 0;

    private static ConcurrentQueue<Data> chunkGenerationQueue;
    private static ConcurrentQueue<Vector3Int> chunkPositionQueue;
    private static ConcurrentQueue<SideUpdate> chunkUpdateQueue;
    private static ConcurrentQueue<MeshDataUpdate> chunkRerenderQueue;

    public WorldData worldData;

    private Task currentChunkCreationTask = null;
    private Task currentChunkUpdateTask = null;
    
    private bool updating = false;

    

    private void Start()
    {
        worldData = new WorldData();

        worldData.activeChunks = new Dictionary<Vector3Int, ChunkRenderer>();
        worldData.activeChunkData = new ConcurrentDictionary<Vector3Int, ChunkData>();
        worldData.canUpdate = new ConcurrentDictionary<Vector3Int, ChunkData>();
        
        chunkGenerationQueue = new ConcurrentQueue<Data>();
        chunkPositionQueue = new ConcurrentQueue<Vector3Int>();
        chunkUpdateQueue = new ConcurrentQueue<SideUpdate>();
        chunkRerenderQueue = new ConcurrentQueue<MeshDataUpdate>();

        worldData.chunksToRemove = new HashSet<Vector3Int>();

        chunkCount = (renderDistance * 2 - 1) * (renderDistance * 2 - 1) * 5;
    }

    private void Update()
    {
        if (generate)
            RenderDistance();

        //frustumCulling.Cull(worldData);
    }
    
    
    public ChunkData GetChunk(Vector3Int chunkPos)
    {
        return worldData.activeChunkData.TryGetValue(chunkPos, out ChunkData chunk) ? chunk : null;
    }

    public bool AddChunk(Vector3Int chunkPos, ChunkData chunk)
    {
        return worldData.activeChunkData.TryAdd(chunkPos, chunk);
    }

    private void ManageGeneration()
    {
        GenerateWorld();
        UpdateWorld();
        InstantiateChunks();
        RerenderChunk();
    }

    private async void GenerateWorld()
    {
        if (currentChunkCreationTask == null)
        {
            currentChunkCreationTask = CreateChunk();
            await currentChunkCreationTask;
            currentChunkCreationTask = null;
        }
    }

    #region Multithread
    private async Task CreateChunk()
    {
        while (chunkPositionQueue.TryDequeue(out Vector3Int newChunkPosition))
        {
            ChunkData newChunkData = new ChunkData(newChunkPosition);
            worldData.activeChunkData.TryAdd(newChunkPosition, newChunkData);
            newChunkData = await CreateChunkData(newChunkData, newChunkPosition);
            worldData.activeChunkData[newChunkPosition] = newChunkData;
            chunkGenerationQueue.Enqueue(new Data(newChunkPosition, newChunkData));
            
            AddUpdateChunks(newChunkData);
        }
    }
    
    private Task<ChunkData> CreateChunkData(ChunkData chunkData, Vector3Int chunkPosition)
    {
        return Task.Run(() =>
        {
            ChunkData newChunkData = chunkGeneratorScript.CreateChunk(chunkData, chunkPosition);
            return newChunkData;
        });
    }

    public void AddUpdateChunks(ChunkData chunkData)
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3Int sidePos = chunkData.position + sideChunks[i];
            if (worldData.activeChunkData.TryGetValue(sidePos, out ChunkData sideChunkData))
            {
                sideChunkData.sideChunks[SpacialData.oppositeFace[i]] = chunkData;
                worldData.activeChunkData[sidePos] = sideChunkData;
                
                chunkData.sideChunks[i] = sideChunkData;
                worldData.activeChunkData[chunkData.position] = chunkData;
                
                SideUpdate sideUpdate = new SideUpdate(chunkData, sideChunkData, 0);
                chunkUpdateQueue.Enqueue(sideUpdate);
            }
            else
            {
                chunkData.sideChunks[i] = null;
            }
        }
    }
    
    public async void UpdateWorld()
    {
        if (currentChunkUpdateTask == null)
        {
            currentChunkUpdateTask = UpdateChunks();
            await currentChunkUpdateTask;
            currentChunkUpdateTask = null;
        }
    }

    private async Task UpdateChunks()
    {
        if (chunkUpdateQueue.TryDequeue(out SideUpdate sideUpdate))
        {
            if (CheckUpdate(sideUpdate))
            {
                try
                {
                    await UpdateChunkData(sideUpdate);
                
                    worldData.activeChunkData[sideUpdate.mainChunk.position] = sideUpdate.mainChunk;
                    worldData.activeChunkData[sideUpdate.sideChunk.position] = sideUpdate.sideChunk;
                
                    chunkRerenderQueue.Enqueue(new MeshDataUpdate(sideUpdate.mainChunk.position, sideUpdate.mainChunk.meshData));
                    chunkRerenderQueue.Enqueue(new MeshDataUpdate(sideUpdate.sideChunk.position, sideUpdate.sideChunk.meshData));
                
                    //Debug.Log(sideUpdate);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error updating chunk data: {ex.Message}");
                }
            }
        }
    }

    private bool CheckUpdate(SideUpdate sideUpdate)
    {
        bool noMain = false;
        bool noSide = false;

        if (!worldData.activeChunkData.ContainsKey(sideUpdate.mainChunk.position))
            noMain = true;
        
        if (!worldData.activeChunkData.ContainsKey(sideUpdate.sideChunk.position))
            noSide = true;

        if (noMain && noSide)
        {
            RemoveUpdate(sideUpdate);
            return false;
        }
        if (noMain)
        {
            sideUpdate.mainChunk.Clear();
            AddUpdateChunks(sideUpdate.sideChunk);
            return false;
        }
        if (noSide)
        {
            sideUpdate.sideChunk.Clear();
            AddUpdateChunks(sideUpdate.mainChunk);
            return false;
        }

        if (!worldData.canUpdate.ContainsKey(sideUpdate.mainChunk.position) ||
            !worldData.canUpdate.ContainsKey(sideUpdate.sideChunk.position))
        {
            chunkUpdateQueue.Enqueue(sideUpdate);
            return false;
        }
            
        return true;

    }

    private void RemoveUpdate(SideUpdate sideUpdate)
    {
        sideUpdate.Clear();
    }
    
    private Task<SideUpdate> UpdateChunkData(SideUpdate sideUpdate)
    {
        return Task.Run(() =>
        {
            SideUpdate newSideUpdate = chunkGeneratorScript.UpdateChunkFromSide(sideUpdate);
            return newSideUpdate;
        });
    }
    
    #endregion

    #region Chunk GameObject renderer
    private void InstantiateChunks()
    {
        if (chunkGenerationQueue.TryDequeue(out Data data))
        {
            ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(data.position, data.chunkData);
            worldData.activeChunks.Add(data.position, chunkRenderer);
            worldData.canUpdate.TryAdd(data.position, data.chunkData);
            data.Clear();
        }
    }
    #endregion

    public void RerenderChunk()
    {
        if (chunkRerenderQueue.TryDequeue(out MeshDataUpdate update))
        {
            if (CheckRerender(update))
            {
                if (worldData.activeChunks.TryGetValue(update.position, out ChunkRenderer mainChunkRenderer))
                {
                    mainChunkRenderer.RenderChunk(update.meshData);
                }
            }
        }
    }

    public bool CheckRerender(MeshDataUpdate update)
    {
        if (!worldData.activeChunkData.ContainsKey(update.position))
        {
            update.Clear();
            return false;
        }
        if (!worldData.canUpdate.ContainsKey(update.position))
        {
            chunkRerenderQueue.Enqueue(update);
            return false;
        }

        return true;
    }
    
    
    
    #region Chunk removal from chunk pool
    private void RemoveChunks()
    {
        foreach (var position in worldData.chunksToRemove)
        {    
            RemoveChunk(position);
        }
    }

    private void RemoveChunk(Vector3Int position)
    {
        ChunkRenderer chunkRenderer;
        if (worldData.activeChunks.TryGetValue(position, out chunkRenderer))
        {
            worldRenderer.RemoveChunk(chunkRenderer);
            worldData.activeChunks.Remove(position);
            RemoveSideDataFromChunks(worldData, position);
            worldData.activeChunkData.Remove(position, out var chunkData);
            chunkData.Clear();
            worldData.canUpdate.Remove(position, out var data);
        }
    }

    private static void RemoveSideDataFromChunks(WorldData worldData, Vector3Int position)
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3Int offset = position + sideChunks[i];
            
            if (worldData.activeChunkData.ContainsKey(offset))
            {
                ChunkData chunkData = worldData.activeChunkData[offset];
                chunkData.sideChunks[SpacialData.oppositeFace[i]] = null;
            }
        }
    }
    #endregion
    

    private void RenderDistance()
    {
        List<Vector3Int> chunksToGenerate = new List<Vector3Int>();
        
        int posX = (int)player.position.x + (player.position.x >= 0 ? 16 : -16);
        int posZ = (int)player.position.z + (player.position.z >= 0 ? 16 : -16);
        
        Vector2Int playerChunkPos = new Vector2Int(posX >> 5, posZ >> 5);
        Vector3Int chunkPosition = new Vector3Int(playerChunkPos.x, 0, playerChunkPos.y);
        
        foreach (var chunk in worldData.activeChunkData)
        {
            worldData.chunksToRemove.Add(chunk.Key);
        }

        for (int render = 0; render < renderDistance; render++)
        {
            for (int cycle = 0; cycle < 4; cycle++)
            {
                int rowWidth = render * 2 + 1;
                for (int row = 0; row < rowWidth; row++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        chunkPosition.y = y;

                        Vector3Int chunkPos = chunkPosition * 32;

                        if (!worldData.activeChunkData.ContainsKey(chunkPos) && !chunksToGenerate.Contains(chunkPos))
                        {
                            chunksToGenerate.Add(chunkPos);
                        }

                        worldData.chunksToRemove.Remove(chunkPos);
                    }
                    
                    chunkPosition.x += chunkPosOffset[cycle].x;
                    chunkPosition.z += chunkPosOffset[cycle].y;
                }
            }
            
            chunkPosition.x++;
            chunkPosition.z++;
        }

        chunkPositionQueue.Clear();
        
        foreach (Vector3Int position in chunksToGenerate)
        {
            chunkPositionQueue.Enqueue(position);
        }
        
        chunksToGenerate.Clear();
        
        RemoveChunks();
        ManageGeneration();      

        worldData.chunksToRemove.Clear();
    }

    public static int2[] chunkPosOffset = 
    {
        new int2(-1, 0),
        new int2( 0,-1),
        new int2( 1, 0),
        new int2( 0, 1),
    };
    
    
    
    
    public static void WriteChunkData(ChunkData chunkData, StreamWriter writer)
    {
        if (chunkData == null)
            writer.WriteLine("no chunk");
            
        writer.WriteLine($"\n\n--------------- New Chunk: {chunkData.position} ---------------");
        for (int i = 0; i < 32768; i++)
        {
            if (chunkData.blocks[i] == null)
                writer.WriteLine($" Block at {i} is: null");
            else
                writer.WriteLine($" Block at {i} is: {chunkData.blocks[i]}");
        }
        writer.WriteLine($"\n--------------- End of Chunk ---------------");
    }

    private int GetLODlevel(int distance)
    {
        int LODlevel = 0;
        int LODdistance = WorldInfo.DropOfLOD;

        while (LODlevel < WorldInfo.MaxLODlevel && LODdistance < distance)
        {
            LODlevel++;
            LODdistance += WorldInfo.DropOfLOD;
        }

        return (int)Mathf.Pow(2, LODlevel);
    }
    
    public static Vector3Int[] sideChunks = new Vector3Int[6]
    {
        new Vector3Int( 0 , 0 ,-32),
        new Vector3Int( 32, 0 , 0 ),
        new Vector3Int( 0 , 32, 0 ),
        new Vector3Int(-32, 0 , 0 ),
        new Vector3Int( 0 ,-32, 0 ),
        new Vector3Int( 0 , 0 , 32),
    };

    public void Clear()
    {
        worldData.activeChunks.Clear();
        worldData.activeChunkData.Clear();

        worldData.chunksToRemove.Clear();

        chunkGenerationQueue.Clear();
        chunkPositionQueue.Clear();

        Debug.Log("Cleared");
    }

    public class Data
    {
        public Vector3Int position;
        public ChunkData chunkData;

        public Data(Vector3Int position, ChunkData chunkData)
        {
            this.position = position;
            this.chunkData = chunkData;
        }

        public void Clear()
        {
            chunkData.meshData.Clear();
        }
    }

    public class MeshDataUpdate
    {
        public Vector3Int position;
        public MeshData meshData;

        public MeshDataUpdate(Vector3Int position, MeshData meshData)
        {
            this.position = position;
            this.meshData = new MeshData(meshData);
        }

        public void Clear()
        {
            meshData.Clear();
        }
    }
}

public struct WorldData
{
    public Dictionary<Vector3Int, ChunkRenderer> activeChunks;
    public ConcurrentDictionary<Vector3Int, ChunkData> activeChunkData;
    public ConcurrentDictionary<Vector3Int, ChunkData> canUpdate;

    public HashSet<Vector3Int> chunksToRemove;
}
