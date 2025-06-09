using System;

namespace Atomic.Networking
{
    [Serializable]
    public ref struct SnapshotSpan<T>
    {
        public int Tick
        {
            get => _tick;
            set => _tick = value;
        }

        public int Length => _span.Length;
        
        private int _tick;
        private Span<T> _span;

        public SnapshotSpan(int tick, Span<T> span)
        {
            _tick = tick;
            _span = span;
        }
        
        public SnapshotSpan(Span<T> span)
        {
            _tick = -1;
            _span = span;
        }

        public ref T this[in int index] => ref _span[index];

        public static implicit operator SnapshotSpan<T>(Span<T> span) => new(span);
    }
}