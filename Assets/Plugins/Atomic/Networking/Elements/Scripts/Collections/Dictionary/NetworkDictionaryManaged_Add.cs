using System;
using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public void Add(KeyValuePair<K, V> item) =>
            this.Add(item.Key, item.Value);
        
        public void Add(K key, V value)
        {
            if (this.IsLocal)
            {
                if (this.m_localDictionary.Count == m_capacity)
                    throw new Exception(MAXED_CAPACITY_ERROR_TEXT);

                this.m_localDictionary.Add(key, value);
                return;
            }

            ref int count = ref this.entry_count();
            if (count == m_capacity)
                throw new Exception(MAXED_CAPACITY_ERROR_TEXT);

            if (!this.TryInsertInternal(in key, in value, ref count, true))
                throw new ArgumentException(nameof(key));

            this.SendEvent(EventType.ADDED, in key, in value);
        }
    }    
}
