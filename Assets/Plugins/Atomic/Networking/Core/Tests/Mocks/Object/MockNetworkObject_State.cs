namespace Atomic.Networking
{
    public sealed unsafe partial class MockNetworkObject
    {
        public bool IsActive => _stateBuffer != null;

        public bool TryGetSnapshotsReturnsTrue { get; set; } = true;

        private int _stateLength;
        private int[] _stateBuffer;

        public bool TryGetPreviousSnapshot<T>(in int ptr, in int count, out SnapshotArray<T> previous, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public int SizeOf<T>(int count = 1) where T : unmanaged => sizeof(T) * count;

        public ref T GetState<T>(int ptr = 0) where T : unmanaged
        {
            fixed (int* p = &_stateBuffer[ptr])
                return ref *(T*) p;
        }

        public int AllocState(int size)
        {
            int pointer = _stateLength;
            _stateLength += size;
            return pointer;
        }

        public int AllocState<T>(int size = 1) where T : unmanaged
        {
            int pointer = _stateLength;
            _stateLength += sizeof(T) * size;
            return pointer;
        }

        public void FreeState(int ptr, int size)
        {
        }

        public void FreeState<T>(int ptr, int size = 1) where T : unmanaged
        {
            this.FreeState(ptr, sizeof(T) * size);
        }

        public bool TryGetSnapshots<T>(in int ptr, out Snapshot<T> previous, out Snapshot<T> current, out float alpha)
            where T : unmanaged
        {
            if (this.TryGetSnapshotsReturnsTrue)
            {
                previous = new Snapshot<T>(5, default);
                current = new Snapshot<T>(6, default);
                alpha = 0.5f;
                return true;
            }

            previous = default;
            current = default;
            alpha = default;
            return false;
        }

        public bool TryGetSnapshots<T>(in int ptr, in int length, SnapshotArray<T> previous, SnapshotArray<T> current, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetSnapshots<T>(in int ptr, in int length, out SnapshotArray<T> previous, out SnapshotArray<T> current,
            out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPreviousSnapshot<T>(in int ptr, out Snapshot<T> previous, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPreviousSnapshot<T>(in int ptr, in int count, ref SnapshotSpan<T> previous, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPreviousSnapshot<T>(in int ptr, in int count, SnapshotArray<T> previous, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetSnapshots<T>(in int ptr, in int length, ref SnapshotSpan<T> previous, ref SnapshotSpan<T> current, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }

        private void AllocStateBuffer()
        {
            _stateBuffer = new int[_stateLength];
        }
        
        public void ReplicateToAll(bool replicate)
        {
            throw new System.NotImplementedException();
        }

        public void ReplicateTo(in int player, bool replicate)
        {
            throw new System.NotImplementedException();
        }

        public void CopyStateFrom(INetworkObject source)
        {
            throw new System.NotImplementedException();
        }

        public void ResetState()
        {
            throw new System.NotImplementedException();
        }
    }
}