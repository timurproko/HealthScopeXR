using Atomic.Elements;

namespace Atomic.Networking.Elements
{
    public interface INetworkVariable<T> : IReactiveVariable<T>
    {
        bool TryGetSnapshots(out Snapshot<T> previous, out Snapshot<T> current, out float alpha);
    }
}