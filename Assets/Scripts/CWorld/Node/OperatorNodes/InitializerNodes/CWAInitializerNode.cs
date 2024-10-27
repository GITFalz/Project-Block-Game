public abstract class CWAInitializerNode : CWAOperatorNode
{
    public abstract void Init(int x, int y, int z);
    public abstract float GetNoise();
    public abstract uint GetPillar(int x, int y, int z);
}