using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public bool Contains(KeyValuePair<K, V> item)
        {
            return this.IsLocal
                ? this.m_localDictionary.ContainsKey(item.Key) && this.m_localDictionary.ContainsValue(item.Value)
                : this.ContainsInternal(in item);
        }

        public bool ContainsKey(K key) => this.IsLocal
            ? this.m_localDictionary.ContainsKey(key)
            : this.ContainsKeyInternal(in key);

        public bool ContainsValue(V value) => this.IsLocal
            ? this.m_localDictionary.ContainsValue(value)
            : this.ContainsValueInternal(in value);

        private bool ContainsInternal(in KeyValuePair<K, V> pair) =>
            this.entry_count() > 0 && this.IndexOfInternal(in pair) >= 0;

        private bool ContainsKeyInternal(in K key) =>
            this.entry_count() > 0 && this.IndexOfInternal(in key) >= 0;

        private bool ContainsValueInternal(in V value)
        {
            VRaw rawValue = m_valueSerializer.Serialize(value);
            for (int i = 0; i < this.entry_count(); i++)
            {
                ref Entry entry = ref this.entry(i);
                if (entry.hashCode > 0 && VALUE_COMPARER.Equals(rawValue, entry.value))
                    return true;
            }

            return false;
        }
    }
}