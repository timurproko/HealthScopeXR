using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        private int IndexOfInternal(in K key) =>
            this.IndexOfInternal(in key, out _);

        private int IndexOfInternal(in KeyValuePair<K, V> pair)
        {
            int index = this.IndexOfInternal(pair.Key, out Entry entry);
            VRaw rawValue = m_valueSerializer.Serialize(pair.Value);
            return index >= 0 && VALUE_COMPARER.Equals(rawValue, entry.value)
                ? index
                : UNDEFINED;
        }

        private int IndexOfInternal(in K key, out Entry entry)
        {
            KRaw rawKey = m_keySerializer.Serialize(key);
            int hashcode = Hashcode(rawKey);
            int index = this.bucket(hashcode % this.m_capacity);

            entry = default;
            while (index >= 0)
            {
                entry = this.entry(index);
                if (entry.hashCode >= 0 && KEY_COMPARER.Equals(rawKey, entry.key))
                    return index;

                index = entry.next;
            }

            return UNDEFINED;
        }
    }
}