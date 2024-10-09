using UnityEngine;

public class ChunkData
{
    public Vector3Int position;
    public Block[] blocks;
    public MeshData meshData;
    public ChunkData[] sideChunks;

    public ChunkData(Vector3Int position)
    {
        this.position = position;
        sideChunks = new ChunkData[6] { null, null, null, null, null, null };
    }

    public void SetBlocks(Block[] newBlocks)
    {
        blocks ??= new Block[newBlocks.Length];
                
        for (int i = 0; i < newBlocks.Length; i++)
        {
            blocks[i] = newBlocks[i];
        }
    }

    public void Clear()
    {
        blocks = null;
        meshData.Clear();
        sideChunks = null;
    }

    public override string ToString()
    {
        return $"Chunk position: {position} \n" +
               $"Blocks: {ChunkInfo.GetBlockCount(this)} \n" +
               $"Mesh: {MeshInfo.GetMeshVertexCount(this)} \n" +
               $"SideChunks: {ChunkInfo.GetSideChunks(sideChunks)}\n";
    }
}
