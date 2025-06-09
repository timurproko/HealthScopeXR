using System;

namespace Atomic.Networking.Elements
{
    public readonly struct NetworkPointer
    {
        private readonly INetworkObject _obj;
        private readonly int _ptr;

        public NetworkPointer(in INetworkObject obj, in int ptr)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _ptr = ptr;
        }

        public void Free(in int size = 1) => _obj.FreeState(_ptr, size);

        public ref T Ref<T>(in int index = 0) where T : unmanaged =>
            ref _obj.GetState<T>(_ptr + _obj.SizeOf<T>(index));

        public bool TryGetSnapshots<T>(
            out Snapshot<T> previous,
            out Snapshot<T> current,
            out float alpha
        ) where T : unmanaged => _obj.TryGetSnapshots(_ptr, out previous, out current, out alpha);

        public bool TryGetSnapshots<T>(
            ref SnapshotSpan<T> previous,
            ref SnapshotSpan<T> current,
            out float alpha
        ) where T : unmanaged => _obj.TryGetSnapshots<T>(_ptr, ref previous, ref current, out alpha);
        
        public static NetworkPointer Allocate(in INetworkObject obj, in int size = 1) =>
            new(in obj, obj.AllocState(size));
    }

    public readonly struct NetworkPointer<T> where T : unmanaged
    {
        private readonly INetworkObject _obj;
        private readonly int _ptr;

        public T Value
        {
            get => _obj.GetState<T>(_ptr);
            set => _obj.GetState<T>(_ptr) = value;
        }

        public NetworkPointer(in INetworkObject obj, in int ptr)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _ptr = ptr;
        }

        public void Free(in int size = 1) => _obj.FreeState<T>(_ptr, size);

        public ref T Ref(in int index = 0) => ref _obj.GetState<T>(_ptr + _obj.SizeOf<T>(index));

        public bool TryGetSnapshots(
            out Snapshot<T> previous,
            out Snapshot<T> current,
            out float alpha
        ) => _obj.TryGetSnapshots(_ptr, out previous, out current, out alpha);

        public bool TryGetSnapshots(
            ref SnapshotSpan<T> previous,
            ref SnapshotSpan<T> current,
            out float alpha
        ) => _obj.TryGetSnapshots(_ptr, ref previous, ref current, out alpha);

        public static NetworkPointer<T> Allocate(in INetworkObject obj, in int size = 1) =>
            new(in obj, obj.AllocState<T>(size));

        public static implicit operator T(in NetworkPointer<T> ptr) => ptr.Value;
    }
}