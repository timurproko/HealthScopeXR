using System;
using System.Collections;
using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        //TODO: Unified validator for the array argument
        //TODO: Try to use enumerators instead of for cycles
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (this.IsLocal)
            {
                foreach (KeyValuePair<K, V> pair in this.m_localDictionary)
                    array[arrayIndex++] = pair;

                return;
            }

            if (array == null)
                throw new NullReferenceException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new IndexOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < Count)
                throw new ArgumentException(nameof(arrayIndex));

            int count = this.entry_count();
            for (int i = 0; i < count; i++)
            {
                Entry entry = this.entry(in i);
                if (entry.hashCode < 0)
                    continue;

                K key = m_keySerializer.Deserialize(in entry.key);
                V value = m_valueSerializer.Deserialize(in entry.value);
                array[arrayIndex++] = new KeyValuePair<K, V>(key, value);
            }
        }
        
        private void CopyTo(in V[] array, int arrayIndex)
        {
            if (this.IsLocal)
            {
                this.m_localDictionary.Values.CopyTo(array, arrayIndex);
                return;
            }

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException(nameof(arrayIndex));

            int count = this.entry_count();
            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref this.entry(in i);
                if (entry.hashCode < 0)
                    continue;

                V value = m_valueSerializer.Deserialize(in entry.value);
                array[arrayIndex++] = value;
            }
        }

        private void CopyTo(in K[] array, int arrayIndex)
        {
            if (this.IsLocal)
            {
                this.m_localDictionary.Keys.CopyTo(array, arrayIndex);
                return;
            }

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException(nameof(arrayIndex));

            int count = this.entry_count();
            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref this.entry(in i);
                if (entry.hashCode < 0)
                    continue;

                K key = m_keySerializer.Deserialize(in entry.key);
                array[arrayIndex++] = key;
            }
        }

        private void CopyKeysTo(in Array array, int arrayIndex)
        {
            if (this.IsLocal)
            {
                ((ICollection) this.m_localDictionary.Keys).CopyTo(array, arrayIndex);
                return;
            }

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException(nameof(array.Rank));

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException();

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < this.entry_count())
                throw new ArgumentException();

            if (array is K[] keys)
                this.CopyTo(in keys, arrayIndex);

            else
            {
                if (array is not object[] objects)
                    throw new ArgumentException();

                try
                {
                    int count = this.entry_count();
                    for (int i = 0; i < count; i++)
                    {
                        Entry entry = this.entry(in i);
                        if (entry.hashCode < 0)
                            continue;

                        K key = m_keySerializer.Deserialize(in entry.key);
                        objects[arrayIndex++] = key;
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
        }

        private void CopyValuesTo(in Array array, int arrayIndex)
        {
            if (this.IsLocal)
            {
                ((ICollection) m_localDictionary.Values).CopyTo(array, arrayIndex);
                return;
            }

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException(nameof(array.Rank));

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException();

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < this.entry_count())
                throw new ArgumentException();

            if (array is V[] values)
                this.CopyTo(in values, arrayIndex);

            else
            {
                if (array is not object[] objects)
                    throw new ArgumentException();

                try
                {
                    int count = this.entry_count();
                    for (int i = 0; i < count; i++)
                    {
                        Entry entry = this.entry(in i);
                        if (entry.hashCode < 0)
                            continue;

                        V value = m_valueSerializer.Deserialize(in entry.value);
                        objects[arrayIndex++] = value;
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}