public abstract class CWAConfigurationNode
{
    public abstract Block GetBlock(int airDistance, int height);
    public abstract bool Match(int height);
}