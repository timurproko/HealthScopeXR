using System;
using System.Collections;
using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        private sealed class KeyCollection : ICollection<K>, ICollection, IReadOnlyCollection<K>
        {
            private readonly NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary;

            public KeyCollection(NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary) =>
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            public int Count => this.dictionary.Count;
            bool ICollection<K>.IsReadOnly => true;
            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => this.dictionary;

            IEnumerator IEnumerable.GetEnumerator() =>
                this.dictionary.GetKeyEnumerator();

            IEnumerator<K> IEnumerable<K>.GetEnumerator() =>
                this.dictionary.GetKeyEnumerator();

            void ICollection<K>.Add(K key) =>
                throw new NotSupportedException();

            bool ICollection<K>.Remove(K key) =>
                throw new NotSupportedException();

            void ICollection<K>.Clear() =>
                throw new NotSupportedException();

            bool ICollection<K>.Contains(K key) =>
                key != null && this.dictionary.ContainsKey(key);

            void ICollection.CopyTo(Array array, int index) =>
                this.dictionary.CopyKeysTo(in array, index);

            public void CopyTo(K[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                this.dictionary.CopyTo(array, index);
            }
        }

        private sealed class ValueCollection : ICollection<V>, ICollection, IReadOnlyCollection<V>
        {
            private readonly NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary;

            public ValueCollection(NetworkDictionaryManaged<K, V, KRaw, VRaw> dictionary) =>
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            public int Count => dictionary.Count;
            bool ICollection<V>.IsReadOnly => true;
            bool ICollection.IsSynchronized => false;
            object ICollection.SyncRoot => this.dictionary;

            IEnumerator IEnumerable.GetEnumerator() =>
                this.dictionary.GetValueEnumerator();

            IEnumerator<V> IEnumerable<V>.GetEnumerator() =>
                this.dictionary.GetValueEnumerator();

            void ICollection<V>.Add(V value) =>
                throw new NotSupportedException();

            bool ICollection<V>.Remove(V value) =>
                throw new NotSupportedException();

            void ICollection<V>.Clear() =>
                throw new NotSupportedException();

            bool ICollection<V>.Contains(V value) =>
                this.dictionary.ContainsValue(value);

            void ICollection.CopyTo(Array array, int index) =>
                this.dictionary.CopyValuesTo(in array, index);

            public void CopyTo(V[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));

                this.dictionary.CopyTo(array, index);
            }
        }
    }
}