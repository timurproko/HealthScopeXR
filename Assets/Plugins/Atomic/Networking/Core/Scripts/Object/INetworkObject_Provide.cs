namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        T Provide<T>();

        bool TryProvide<T>(out T value);
    }
}