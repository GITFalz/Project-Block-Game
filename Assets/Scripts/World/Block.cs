
public class Block
{
    public short blockData;
    public byte state;
    public byte occlusion;

    public Block(short blockData, byte state)
    {
        this.blockData = blockData;
        this.state = state;
        occlusion = 0;
    }

    public override string ToString()
    {
        return $"BlockData: {blockData}, State: {state}";
    }
}
