public abstract class CWAExecuteNode : CWAOperatorNode
{
    public abstract float GetNoise();
    public abstract Block GetBlock(int x, int y, int z);
}