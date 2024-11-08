using System.Collections.Generic;
using UnityEngine;

public class WorldRenderer : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Transform parentChunk;
    public Queue<ChunkRenderer> chunkPool = new Queue<ChunkRenderer>();

    public void Clear(WorldData worldData)
    {
        foreach (var chunk in worldData.activeChunks.Values)
        {
            Destroy(chunk.gameObject);
        }
        chunkPool.Clear();
    }

    internal ChunkRenderer RenderChunk(Vector3Int position, ChunkData chunkData)
    {
        ChunkRenderer renderer;
        
        if (chunkPool.Count > 0)
        {
            renderer = chunkPool.Dequeue();
            renderer.transform.position = chunkData.position;
            renderer.ClearMesh();
            renderer.gameObject.SetActive(true);
        }
        else
        {
            GameObject newChunk = Instantiate(chunkPrefab, position, Quaternion.identity);
            renderer = newChunk.GetComponent<ChunkRenderer>();
            renderer.RenderChunk(chunkData);
        }
        
        return renderer;
    }

    public void RemoveChunk(ChunkRenderer renderer)
    {
        renderer.gameObject.SetActive(false);
        renderer.ClearMesh();
        chunkPool.Enqueue(renderer);
    }
}
