using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public sealed class NetworkDictionary<K, V> : NetworkDictionaryManaged<K, V, K, V>
        where K : unmanaged
        where V : unmanaged
    {
        public NetworkDictionary(
            in INetworkObject agent,
            in int capacity,
            in int eventCapacity = 4
        ) : base(in agent, NetworkSerializer<K>.Instance, NetworkSerializer<V>.Instance, in capacity, in eventCapacity)
        {
        }

        public NetworkDictionary(
            in INetworkObject agent,
            in int capacity,
            in int eventCapacity = 4,
            params KeyValuePair<K, V>[] initialEntries
        ) : base(in agent, NetworkSerializer<K>.Instance, NetworkSerializer<V>.Instance, in capacity, in eventCapacity,
            initialEntries)
        {
        }
    }
}
