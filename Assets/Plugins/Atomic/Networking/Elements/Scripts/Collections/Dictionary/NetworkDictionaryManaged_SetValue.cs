using System;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        private void SetValueInternal(K key, V value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (this.IsLocal)
            {
                if (this.m_localDictionary.Count == m_capacity && !m_localDictionary.ContainsKey(key))
                    throw new Exception(MAXED_CAPACITY_ERROR_TEXT);

                this.m_localDictionary[key] = value;
                return;
            }

            int previousCount = this.entry_count();
            if (!this.TryInsertInternal(in key, in value, false))
                throw new Exception(MAXED_CAPACITY_ERROR_TEXT);

            EventType eventType = previousCount == this.entry_count()
                ? EventType.CHANGED
                : EventType.ADDED;

            this.SendEvent(in eventType, in key, in value);
        }
    }
}