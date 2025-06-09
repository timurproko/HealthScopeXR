using System;

namespace Atomic.Networking
{
    [Serializable]
    public sealed class SnapshotArray<T>
    {
        public int Tick
        {
            get => _tick;
            set => _tick = value;
        }
        
        public int Count
        {
            get => _count;
            set => _count = value;
        }
        
        private int _tick;
        private int _count;
        private readonly T[] _data;
    
        public T this[in int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    
        public SnapshotArray(int capacity)
        {
            _tick = 0;
            _data = new T[capacity];
        }
    }
}