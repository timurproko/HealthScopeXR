namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        private bool TryInsertInternal(in K key, in V value, in bool onlyAdd)
        {
            ref int count = ref entry_count();
            return TryInsertInternal(in key, in value, ref count, in onlyAdd);
        }

        private bool TryInsertInternal(in K key, in V value, ref int count, in bool onlyAdd)
        {
            KRaw rawKey = m_keySerializer.Serialize(key);
            int hashcode = Hashcode(in rawKey);

            int bucketIndex = hashcode % this.m_capacity;
            ref int bucket = ref this.bucket(in bucketIndex);
            int next;

            for (int i = bucket; i != UNDEFINED; i = next)
            {
                ref Entry entry = ref this.entry(in i);
                next = entry.next;

                if (entry.hashCode != hashcode)
                    continue;

                if (!KEY_COMPARER.Equals(rawKey, entry.key))
                    continue;

                if (onlyAdd)
                    return false;

                entry.value = m_valueSerializer.Serialize(in value);
                this.version()++;
                return true;
            }

            if (count == this.m_capacity)
                return false;

            ref int freeList = ref this.free_list();
            int index;

            if (freeList > 0)
            {
                index = freeList;
                freeList = this.entry(in index).next;
            }
            else
            {
                index = count;
            }

            this.entry(in index) = new Entry
            {
                hashCode = hashcode,
                key = rawKey,
                value = m_valueSerializer.Serialize(in value),
                next = bucket
            };

            bucket = index;
            count++;
            this.version()++;
            return true;
        }
    }
}