using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    //TODO: SNAPSHOTS
    public class NetworkSet<T> : NetworkSetManaged<T, T> where T : unmanaged
    {
        public NetworkSet(
            in INetworkObject agent,
            in int capacity,
            in int eventCapacity = 4
        ) : base(in agent, NetworkSerializer<T>.Instance, in capacity, in eventCapacity)
        {
        }

        public NetworkSet(
            in INetworkObject agent,
            in int capacity,
            in IEnumerable<T> initialItems,
            in int eventCapacity = 4
        ) : base(in agent, NetworkSerializer<T>.Instance, in capacity, in initialItems, in eventCapacity)
        {
        }
    }
}