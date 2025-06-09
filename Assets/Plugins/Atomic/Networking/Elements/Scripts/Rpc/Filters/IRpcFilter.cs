namespace Atomic.Networking
{
    public interface IRpcFilter
    {
        bool Matches(int player);
    }
}