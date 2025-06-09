using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        //TODO: Consider removing only if values match...
        public bool Remove(KeyValuePair<K, V> item) => this.Remove(item.Key);

        public bool Remove(K key)
        {
            if (this.IsLocal)
                return this.m_localDictionary.Remove(key);

            if (!this.TryRemoveInternal(in key, out V value))
                return false;

            this.SendEvent(EventType.REMOVED, in key, in value);
            return true;
        }
        
        private bool TryRemoveInternal(in K key, out V value)
        {
            value = default;
            ref int count = ref this.entry_count();
            return TryRemoveInternal(in key, ref value, ref count, false);
        }

        private bool TryRemoveInternal(in K key, ref V value, ref int count, bool match)
        {
            if (count <= 0)
                return false;

            KRaw rawKey = m_keySerializer.Serialize(in key);
            VRaw rawValue = m_valueSerializer.Serialize(in value);

            int hashcode = Hashcode(in rawKey);
            int bucketIndex = hashcode % this.m_capacity;
            ref int bucket = ref this.bucket(in bucketIndex);

            int index = bucket;
            int last = UNDEFINED;

            while (index >= 0)
            {
                ref Entry entry = ref this.entry(in index);
                if (KEY_COMPARER.Equals(rawKey, entry.key))
                {
                    if (match && !VALUE_COMPARER.Equals(rawValue, entry.value))
                        return false;

                    if (last < 0)
                        bucket = entry.next;
                    else
                        this.entry(last).next = entry.next;

                    ref int freeList = ref this.free_list();

                    entry.next = freeList;
                    entry.hashCode = -1;
                    value = m_valueSerializer.Deserialize(in entry.value);

                    count--;
                    freeList = count == 0 ? UNDEFINED : index;
                    this.version()++;
                    return true;
                }

                last = index;
                index = entry.next;
            }

            return false;
        }
    }
}