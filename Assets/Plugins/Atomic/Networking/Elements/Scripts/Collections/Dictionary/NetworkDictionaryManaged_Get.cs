using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public bool TryGetValue(K key, out V value)
        {
            if (this.IsLocal)
                return this.m_localDictionary.TryGetValue(key, out value);

            int i = this.IndexOfInternal(in key, out Entry entry);
            bool hasValue = i >= 0;

            value = hasValue
                ? m_valueSerializer.Deserialize(in entry.value)
                : default;

            return hasValue;
        }

        private V GetValueInternal(K key)
        {
            if (this.IsLocal)
                return m_localDictionary[key];

            int i = this.IndexOfInternal(in key, out Entry entry);
            return i >= 0
                ? m_valueSerializer.Deserialize(in entry.value)
                : throw new KeyNotFoundException(nameof(key));
        }
    }
}