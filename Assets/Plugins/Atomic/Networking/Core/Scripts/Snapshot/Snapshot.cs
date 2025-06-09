using System;

namespace Atomic.Networking
{
    [Serializable]
    public struct Snapshot<T>
    {
        public int tick;
        public T data;

        public Snapshot(int tick, T data)
        {
            this.tick = tick;
            this.data = data;
        }
    }
}


