namespace Core.StatSystem
{
    public interface IStatContainer<TStat>
    {
        float GetValue(TStat stat);
        bool TryGetValue(TStat stat, out float value);
    }
}
