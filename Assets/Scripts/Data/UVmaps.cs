

[System.Serializable]
public struct UVmaps
{
    public int[] textureIndices;

    public UVmaps(int[] textureIndices)
    {
        this.textureIndices = textureIndices;
    }

    public static UVmaps DefaultIndexUVmap => new UVmaps(new int[] {0, 0, 0, 0, 0, 0});
}
