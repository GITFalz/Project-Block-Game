public class SideUpdate
{
    public ChunkData mainChunk;
    public ChunkData sideChunk;

    public SideUpdate(ChunkData mainChunk, ChunkData sideChunk)
    {
        this.mainChunk = mainChunk;
        this.sideChunk = sideChunk;
    }

    public void Clear()
    {
        mainChunk.Clear();
        sideChunk.Clear();
    }

    public override string ToString()
    {
        return $"Main Chunk: {mainChunk}\n\n" +
               $"Side Chunk: {sideChunk}";
    }
}
