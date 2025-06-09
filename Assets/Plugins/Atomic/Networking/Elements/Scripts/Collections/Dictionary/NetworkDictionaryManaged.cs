using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Elements;
using UnityEngine;

// ReSharper disable RedundantAssignment
// ReSharper disable StaticMemberInGenericType
// ReSharper disable JoinNullCheckWithUsage

/**
 * Author Maxim Steshenko — 09.2024
 */

//TODO: SNAPSHOTS
namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw> :
        IDisposable,
        IReactiveDictionary<K, V>,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where KRaw : unmanaged
        where VRaw : unmanaged
    {
        private struct Entry
        {
            public int hashCode; // Lower 31 bits of hash code, -1 if unused
            public int next; // Index of next entry, -1 if last
            public KRaw key;
            public VRaw value;

            public override string ToString() =>
                $"{nameof(key)}: {key}, {nameof(value)}: {value}, {nameof(next)}: {next}";
        }

        internal const int UNDEFINED = -1;
        internal const string MAXED_CAPACITY_ERROR_TEXT = "Can't add entry! Networked dictionary is at capacity!";

        private static readonly IEqualityComparer<KRaw> KEY_COMPARER = EqualityComparer<KRaw>.Default;
        private static readonly IEqualityComparer<VRaw> VALUE_COMPARER = EqualityComparer<VRaw>.Default;

        public event StateChangedHandler OnStateChanged;
        public event SetItemHandler<K, V> OnItemChanged;
        public event AddItemHandler<K, V> OnItemAdded;
        public event RemoveItemHandler<K, V> OnItemRemoved;

        public ICollection<K> Keys => this.m_keys;
        public ICollection<V> Values => this.m_values;
        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Values;
        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Keys;

        public int Capacity => this.m_capacity;
        public int Count => this.m_agent.IsActive ? this.entry_count() : this.m_localDictionary.Count;
        public bool IsLocal => !this.m_agent.IsActive;
        public bool IsReadOnly => false;

        private readonly INetworkObject m_agent;
        private readonly INetworkSerializer<K, KRaw> m_keySerializer;
        private readonly INetworkSerializer<V, VRaw> m_valueSerializer;

        private readonly int m_capacity;
        private readonly int m_bucketsPtr;
        private readonly int m_entryArrayPtr;
        private readonly int m_entryCountPtr;
        private readonly int m_freeListPtr;
        private readonly int m_freeCountPtr;

        private readonly int m_eventCapacity;
        private readonly int m_eventRequiredPtr;
        private readonly int m_eventQueuePtr;
        private int m_eventConsumed;

        private readonly int m_versionPtr;
        private int m_versionConsumed;

        private readonly Dictionary<K, V> m_localDictionary;
        private readonly ValueCollection m_values;
        private readonly KeyCollection m_keys;

        public NetworkDictionaryManaged(
            in INetworkObject agent,
            in INetworkSerializer<K, KRaw> keySerializer,
            in INetworkSerializer<V, VRaw> valueSerializer,
            in int capacity,
            in int eventCapacity = 4,
            params KeyValuePair<K, V>[] initialEntries
        ) : this(agent, keySerializer, valueSerializer, capacity, eventCapacity)
        {
            if (initialEntries == null)
                return;

            for (int i = 0, count = Math.Min(capacity, initialEntries.Length); i < count; i++)
                this.Add(initialEntries[i].Key, initialEntries[i].Value);
        }

        public NetworkDictionaryManaged(
            in INetworkObject agent,
            in INetworkSerializer<K, KRaw> keySerializer,
            in INetworkSerializer<V, VRaw> valueSerializer,
            in int capacity = 2,
            in int eventCapacity = 4
        )
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException($"Entry capacity must be positive! Actual: {capacity}");

            m_keySerializer = keySerializer ?? throw new ArgumentNullException(nameof(keySerializer));
            m_valueSerializer = valueSerializer ?? throw new ArgumentNullException(nameof(valueSerializer));

            m_capacity = capacity;
            m_entryCountPtr = agent.AllocState<int>();
            m_entryArrayPtr = agent.AllocState<Entry>(capacity);
            m_bucketsPtr = agent.AllocState<int>(capacity);
            m_freeListPtr = agent.AllocState<int>();

            m_eventCapacity = eventCapacity;
            m_eventQueuePtr = agent.AllocState<Event>(eventCapacity);
            m_eventRequiredPtr = agent.AllocState<int>();

            m_versionPtr = agent.AllocState<int>();

            m_localDictionary = new Dictionary<K, V>(capacity);
            m_values = new ValueCollection(this);
            m_keys = new KeyCollection(this);

            m_agent = agent;
            m_agent.AddListener(this);
        }

        public void Dispose()
        {
            //TODO: FREE STATE
            m_agent.RemoveListener(this);
            this.UnsubscribeAll();
        }
        
        public V this[K key]
        {
            get => this.GetValueInternal(key);
            set => this.SetValueInternal(key, value);
        }
        
        void INetworkObject.ISpawned.OnSpawned()
        {
            this.entry_count() = 0;
            this.free_list() = UNDEFINED;
            this.free_count() = 0;

            this.FreeBuckets();
            this.FreeEntries();
            this.SetupItems();
            this.m_versionConsumed = this.version();
        }

        private void SetupItems()
        {
            foreach ((K key, V value) in this.m_localDictionary)
                this.TryInsertInternal(key, value, true);

            this.version() = 0; //Reset version after adding pair from a local dictionary
        }

        private void FreeBuckets()
        {
            for (int i = 0; i < this.m_capacity; i++)
                this.bucket(i) = UNDEFINED;
        }

        private void FreeEntries()
        {
            for (int i = 0; i < this.m_capacity; i++)
                this.entry(i).hashCode = UNDEFINED;
        }

        private ref int entry_count() =>
            ref this.m_agent.GetState<int>(m_entryCountPtr);

        private static int Hashcode(in KRaw key)
            => KEY_COMPARER.GetHashCode(key) & 0x7FFFFFFF;

        private ref Entry entry(in int index) =>
            ref this.m_agent.GetState<Entry>(this.m_entryArrayPtr + m_agent.SizeOf<Entry>(index));

        private ref int bucket(in int index) =>
            ref this.m_agent.GetState<int>(this.m_bucketsPtr + m_agent.SizeOf<int>(index));

        private ref Event event_arg(in int index) =>
            ref this.m_agent.GetState<Event>(this.m_eventQueuePtr + m_agent.SizeOf<Event>(index));

        private ref int free_list() =>
            ref this.m_agent.GetState<int>(this.m_freeListPtr);

        private ref int free_count() =>
            ref this.m_agent.GetState<int>(this.m_freeCountPtr);

        private ref int version() =>
            ref this.m_agent.GetState<int>(this.m_versionPtr);

        private ref int event_required() =>
            ref this.m_agent.GetState<int>(this.m_eventRequiredPtr);
    }
}