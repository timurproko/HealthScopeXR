using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public sealed class NetworkList<T> : NetworkListManaged<T, T> where T : unmanaged
    {
        public NetworkList(
            in INetworkObject agent,
            in int capacity,
            in int eventCapacity = 4
        ) : base(in agent, NetworkSerializer<T>.Instance, in capacity, in eventCapacity)
        {
        }

        public NetworkList(
            in INetworkObject agent,
            in int capacity,
            in IEnumerable<T> initialItems,
            in int eventCapacity = 4 
        ) : base(in agent, NetworkSerializer<T>.Instance, in capacity, in initialItems, in eventCapacity)
        {
        }
    }
}