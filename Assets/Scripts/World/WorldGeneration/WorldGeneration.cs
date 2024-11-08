using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public Transform player;
    public int renderDistance = 8;
    
    public bool generate = false;
    
    public WorldRenderer worldRenderer;

    public static ConcurrentQueue<Vector3Int> chunksToGenerate;
    public static ConcurrentQueue<ChunkData> chunksToRender;
    public static ConcurrentQueue<Vector3Int> chunksToRemove;

    public Vector3Int oldRelativePosition = new Vector3Int(0, 0, 0);
    public Vector3Int currentRelativePosition = new Vector3Int(0, 0, 0);
    
    public Func<Vector2> moveInput;
    
    public int numberOfActiveChunks = 0;
    public int numberOfExistingChunks = 0;
    public int numberOfChunksToIgnore = 0;

    private void Start()
    {
        chunksToGenerate = new ConcurrentQueue<Vector3Int>();
        chunksToRender = new ConcurrentQueue<ChunkData>();
        chunksToRemove = new ConcurrentQueue<Vector3Int>();
        
        moveInput = PlayerInput.Instance.MoveInput;
        
        GenerateChunkPositions();
    }

    private void Update()
    {
        Vector2 input = moveInput();
        if (!generate)
            return;

        if (!input.Equals(Vector2.zero))
        {
            currentRelativePosition.x = RelativeAxis(player.position.x);
            currentRelativePosition.y = RelativeAxis(player.position.y);
            currentRelativePosition.z = RelativeAxis(player.position.z);
        }
        
        if (!currentRelativePosition.Equals(oldRelativePosition))
        {
            GenerateChunkPositions();
        }
        
        GenerateChunk();
        RenderChunk();
        DeleteChunk();
        
        oldRelativePosition = currentRelativePosition;
        
        numberOfActiveChunks = WorldChunks.activeChunks.Count;
        numberOfExistingChunks = WorldChunks.existingChunks.Count;
        numberOfChunksToIgnore = WorldChunks.chunksToIgnore.Count;
    }

    public void SetGenerate(bool b)
    {
        generate = b;
    }

    private int RelativeAxis(float axis)
    {
        return Mathf.FloorToInt(axis / 32);
    }

    public async void GenerateChunk()
    {
        for (int i = 0; i < ChunkGenerationNodes.threadCount; i++)
        {
            if (ChunkGenerationNodes.tasks[i] == null)
            {
                if (chunksToGenerate.TryDequeue(out var result))
                {
                    if (WorldChunks.chunksToIgnore.Remove(result))
                        continue;
                    
                    WorldChunks.existingChunks.Add(result);
                    
                    ChunkGenerationNodes.tasks[i] = Chunk.CreateMapChunk(new ChunkData(result), result, ChunkGenerationNodes.dataHandlers[i], 0);
                    await ChunkGenerationNodes.tasks[i];
                    ChunkGenerationNodes.tasks[i] = null;
                }
            }
        }
    }

    public void RenderChunk()
    {
        if (chunksToRender.TryDequeue(out var result))
        {
            ChunkRenderer chunkRenderer = worldRenderer.RenderChunk(result.position, result);
            if (!WorldChunks.activeChunks.TryAdd(result.position, chunkRenderer))
                worldRenderer.RemoveChunk(chunkRenderer);
        }
    }

    public void DeleteChunk()
    {
        if (chunksToRemove.TryDequeue(out var result))
        {
            if (WorldChunks.activeChunks.Remove(result, out var chunkRenderer))
            {
                WorldChunks.chunksToIgnore.Remove(result);
                worldRenderer.RemoveChunk(chunkRenderer);
            }
        }
    }

    public void GenerateChunkPositions()
    {
        int loop, dir, maxSteps;
        int x = currentRelativePosition.x;
        int z = currentRelativePosition.z;
        
        HashSet<Vector3Int> chunksToDelete = new HashSet<Vector3Int>();
        foreach (var chunk in WorldChunks.existingChunks)
            chunksToDelete.Add(chunk);
            
        for (int d = 0; d < renderDistance; d++)
        {
            dir = 0;
            loop = 1 + d * 2;
            maxSteps = 4 + d * 8;
                
            for (int step = 0; step < maxSteps; step++)
            {
                for (int y = 0; y < WorldInfo.worldChunkHeight; y++)
                {
                    Vector3Int position = new Vector3Int(x * 32, y * 32, z * 32);

                    chunksToDelete.Remove(position);
                    
                    if (!chunksToGenerate.Contains(position) && !WorldChunks.existingChunks.Contains(position))
                    {
                        chunksToGenerate.Enqueue(position);
                    }
                }

                x += SpacialData.chunkDirections[dir, 0];
                z += SpacialData.chunkDirections[dir, 1];

                loop--;

                if (loop == 0)
                {
                    dir++;
                    loop = 1 + d * 2;
                }
            }

            x--; z--;
        }
        
        foreach (var chunk in chunksToDelete)
        {
            if (WorldChunks.chunksToIgnore.Add(chunk))
                chunksToRemove.Enqueue(chunk);
        }
    }
}