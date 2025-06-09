namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        bool TryGetSnapshots<T>(
            in int ptr,
            out Snapshot<T> previous,
            out Snapshot<T> current,
            out float alpha
        ) where T : unmanaged;
        
        public bool TryGetSnapshots<T>(
            in int ptr,
            ref SnapshotSpan<T> previous,
            ref SnapshotSpan<T> current,
            out float alpha
        ) where T : unmanaged;
    }
}