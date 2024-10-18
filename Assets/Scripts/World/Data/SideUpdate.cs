public class SideUpdate
{
    public ChunkData mainChunk;
    public ChunkData sideChunk;
    public int mainFace;

    public SideUpdate(ChunkData mainChunk, ChunkData sideChunk, int mainFace)
    {
        this.mainChunk = mainChunk;
        this.sideChunk = sideChunk;
        this.mainFace = mainFace;
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
