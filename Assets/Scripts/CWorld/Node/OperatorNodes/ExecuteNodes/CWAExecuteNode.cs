using UnityEngine;

public abstract class CWAExecuteNode : CWAOperatorNode
{
    public abstract float GetNoise();
    public abstract int GetBlock(int x, int y, int z);
    public abstract Block[] GetBlocks(Vector3Int position, Block[] blocks, CWorldHandler handler);
}