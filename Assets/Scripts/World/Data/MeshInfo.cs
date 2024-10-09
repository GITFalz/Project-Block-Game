public static class MeshInfo
{
    public static int GetMeshVertexCount(ChunkData chunkData)
    {
        return chunkData.meshData.verts.Count;
    }
}