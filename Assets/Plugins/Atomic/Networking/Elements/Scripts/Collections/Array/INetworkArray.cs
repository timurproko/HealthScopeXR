using Atomic.Elements;

namespace Atomic.Networking.Elements
{
    public interface INetworkArray<T> : IReactiveArray<T>
    {
        bool TryGetSnapshots(in int index, out Snapshot<T> previous, out Snapshot<T> current, out float alpha);
        
        bool TryGetSnapshots(ref SnapshotSpan<T> previous, ref SnapshotSpan<T> current, out float alpha);
        
        bool TryGetSnapshots(in SnapshotArray<T> previous, in SnapshotArray<T> current, out float alpha);
    }
}