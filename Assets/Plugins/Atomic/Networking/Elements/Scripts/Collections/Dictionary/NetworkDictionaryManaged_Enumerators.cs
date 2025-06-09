using System.Collections;
using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => IsLocal
            ? this.m_localDictionary.GetEnumerator()
            : new PairEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        private IEnumerator<K> GetKeyEnumerator() => IsLocal
            ? this.m_localDictionary.Keys.GetEnumerator()
            : new KeyEnumerator(this);

        private IEnumerator<V> GetValueEnumerator() => IsLocal
            ? this.m_localDictionary.Values.GetEnumerator()
            : new ValueEnumerator(this);
        
        //TODO: Track version to see when the collection was modified
        public struct PairEnumerator : IEnumerator<KeyValuePair<K, V>>
        {
            object IEnumerator.Current => this.currentPair;

            public KeyValuePair<K, V> Current => this.currentPair;

            private readonly NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary;
            private KeyValuePair<K, V> currentPair;
            private int index;

            public PairEnumerator(in NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary)
            {
                this.dictionary = dictionary;
                this.index = 0;
                this.currentPair = default;
            }

            public bool MoveNext()
            {
                while (this.index < this.dictionary.m_capacity)
                {
                    ref Entry entry = ref this.dictionary.entry(this.index++);
                    if (entry.hashCode < 0)
                        continue;

                    K key = this.dictionary.m_keySerializer.Deserialize(in entry.key);
                    V value = this.dictionary.m_valueSerializer.Deserialize(in entry.value);
                    this.currentPair = new KeyValuePair<K, V>(key, value);
                    return true;
                }

                this.currentPair = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.currentPair = default;
            }

            public void Dispose()
            {
                //Do nothing...
            }
        }

        public struct ValueEnumerator : IEnumerator<V>
        {
            private readonly NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary;
            private V currentValue;
            private int index;

            internal ValueEnumerator(in NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary)
            {
                this.dictionary = dictionary;
                this.index = 0;
                this.currentValue = default;
            }

            object IEnumerator.Current => this.Current;

            public V Current => this.currentValue;

            public bool MoveNext()
            {
                while (this.index < this.dictionary.Count)
                {
                    ref Entry entry = ref this.dictionary.entry(this.index++);
                    if (entry.hashCode < 0)
                        continue;

                    V value = this.dictionary.m_valueSerializer.Deserialize(in entry.value);
                    this.currentValue = value;
                    return true;
                }

                this.currentValue = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.currentValue = default;
            }

            public void Dispose()
            {
                //Do nothing...
            }
        }

        public struct KeyEnumerator : IEnumerator<K>
        {
            private readonly NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary;
            private int index;
            private K currentKey;

            internal KeyEnumerator(in NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary)
            {
                this.dictionary = dictionary;
                this.index = 0;
                this.currentKey = default;
            }

            public K Current => this.currentKey;

            object IEnumerator.Current => this.currentKey;

            public bool MoveNext()
            {
                while (this.index < this.dictionary.Count)
                {
                    ref Entry entry = ref this.dictionary.entry(this.index++);
                    if (entry.hashCode < 0)
                        continue;

                    K key = this.dictionary.m_keySerializer.Deserialize(in entry.key);
                    this.currentKey = key;
                    return true;
                }

                this.currentKey = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.currentKey = default;
            }

            public void Dispose()
            {
                //Do nothing...
            }
        }
    }    
}

