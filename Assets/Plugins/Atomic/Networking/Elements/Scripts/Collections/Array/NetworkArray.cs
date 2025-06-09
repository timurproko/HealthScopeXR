using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public class NetworkArray<T> : NetworkArrayManaged<T, T> where T : unmanaged
    {
        public NetworkArray(in INetworkObject obj, in int capacity) :
            base(in obj, NetworkSerializer<T>.Instance, in capacity)
        {
        }

        public NetworkArray(
            in INetworkObject obj,
            in int capacity,
            in IEnumerable<T> initial
        ) : base(in obj, NetworkSerializer<T>.Instance, in capacity, in initial)
        {
        }
    }
}
