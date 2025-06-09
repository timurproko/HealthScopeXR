using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Elements;

// ReSharper disable JoinNullCheckWithUsage

namespace Atomic.Networking.Elements
{
    public class NetworkSetManaged<T, TRaw> : IReactiveSet<T>, IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw : unmanaged
    {
        internal const int UNDEFINED = -1;

        private static readonly IEqualityComparer<TRaw> COMPARER = EqualityComparer<TRaw>.Default;

        public event StateChangedHandler OnStateChanged;
        public event AddItemHandler<T> OnItemAdded;
        public event RemoveItemHandler<T> OnItemRemoved;

        public int Capacity => m_itemCapacity;
        public int Count => m_agent.IsActive ? item_count() : m_localSet.Count;

        public bool IsReadOnly => false;

        private readonly INetworkObject m_agent;
        private readonly INetworkSerializer<T, TRaw> m_serializer;

        //Items:
        private readonly int m_itemCapacity;
        private readonly int m_itemCountPtr;
        private readonly int m_itemArrayPtr;
        //TODO: Add last index

        //Buckets:
        private readonly int m_bucketsPtr;

        //Recycled:
        private readonly int m_freeListPtr;

        //Events:
        private readonly int m_eventCapacity;
        private readonly int m_eventRequiredPtr;
        private readonly int m_eventQueuePtr;
        private int m_eventConsumed;

        //Version:
        private readonly int m_versionPtr;
        private int m_versionConsumed;

        //Local:
        private readonly HashSet<T> m_localSet;

        public NetworkSetManaged(
            in INetworkObject agent,
            in INetworkSerializer<T, TRaw> serializer,
            in int capacity,
            in IEnumerable<T> initialItems,
            in int eventCapacity = 4
        ) : this(agent, serializer, capacity, eventCapacity)
        {
            if (initialItems == null)
                return;

            int i = 0;
            foreach (T item in initialItems)
            {
                if (i >= capacity)
                    break;

                this.Add(item);
                i++;
            }
        }

        public NetworkSetManaged(
            in INetworkObject agent,
            in INetworkSerializer<T, TRaw> serializer,
            in int capacity,
            in int eventCapacity = 4
        )
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException($"Item capacity must be more than zero! Actual: {capacity}");

            if (eventCapacity < 0)
                throw new ArgumentOutOfRangeException($"Event capacity must be non negative! Actual: {eventCapacity}");

            m_itemCapacity = capacity;
            m_itemCountPtr = agent.AllocState<int>();
            m_itemArrayPtr = agent.AllocState<Node>(capacity);
            m_serializer = serializer;

            m_bucketsPtr = agent.AllocState<int>(capacity);
            m_freeListPtr = agent.AllocState<int>();

            m_eventCapacity = eventCapacity;
            m_eventQueuePtr = agent.AllocState<Event>(eventCapacity);
            m_eventRequiredPtr = agent.AllocState<int>();

            m_versionPtr = agent.AllocState<int>();
            m_localSet = new HashSet<T>(capacity);

            m_agent = agent;
            m_agent.AddListener(this);
        }

        public void Dispose()
        {
            m_agent.RemoveListener(this);
            this.UnsubscribeAll();
        }

        void INetworkObject.ISpawned.OnSpawned()
        {
            this.item_count() = m_localSet.Count;
            this.free_list() = UNDEFINED;

            this.FreeBuckets();
            this.SetupItems();

            this.m_eventConsumed = this.event_required();
            this.m_versionConsumed = this.version();
        }

        void INetworkObject.IRender.OnRender()
        {
            this.ConsumeEvents();
            this.ConsumeVersion();
        }

        public void UnsubscribeAll()
        {
            if (this.OnItemAdded != null)
            {
                Delegate[] delegates = this.OnItemAdded.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemAdded -= (AddItemHandler<T>) delegates[i];

                this.OnItemAdded = null;
            }

            if (this.OnItemRemoved != null)
            {
                Delegate[] delegates = this.OnItemRemoved.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemRemoved -= (RemoveItemHandler<T>) delegates[i];

                this.OnItemRemoved = null;
            }

            if (this.OnStateChanged != null)
            {
                Delegate[] delegates = this.OnStateChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnStateChanged -= (StateChangedHandler) delegates[i];

                this.OnStateChanged = null;
            }
        }

        public bool Contains(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return !m_agent.IsActive
                ? m_localSet.Contains(item)
                : this.item_count() > 0 && this.ContainsInternal(m_serializer.Serialize(item));
        }

        public bool IsEmpty()
        {
            return m_agent.IsActive ? this.item_count() == 0 : m_localSet.Count == 0;
        }

        public bool IsNotEmpty()
        {
            return m_agent.IsActive
                ? this.item_count() > 0
                : m_localSet.Count > 0;
        }

        public bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!m_agent.IsActive)
            {
                return m_localSet.Count == m_itemCapacity
                    ? throw new Exception("Can't add item! Neworked set is full!")
                    : m_localSet.Add(item);
            }

            ref int count = ref this.item_count();
            if (count == m_itemCapacity)
                throw new Exception("Can't add item! Networked set is full!");

            TRaw data = m_serializer.Serialize(item);

            if (!this.AddIfNotPresentInternal(data, ref count, out _))
                return false;

            this.version()++;
            this.SendEvent(EventType.ADDED, data);

            return true;
        }

        void ICollection<T>.Add(T item)
        {
            if (item != null) this.Add(item);
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            if (!m_agent.IsActive)
                return m_localSet.Remove(item);

            ref int count = ref this.item_count();
            if (count == 0)
                return false;

            TRaw data = m_serializer.Serialize(item);

            if (!this.RemoveInternal(data, ref count))
                return false;

            this.version()++;
            this.SendEvent(EventType.REMOVED, data);
            return true;
        }

        public void Clear()
        {
            if (!m_agent.IsActive)
            {
                m_localSet.Clear();
                return;
            }

            ref int count = ref this.item_count();
            if (count == 0)
                return;

            this.ClearInternal(ref count);
            this.version()++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_agent.IsActive
                ? new Enumerator(this)
                : m_localSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.UnionWith(other);
                return;
            }

            ref int count = ref this.item_count();
            int prevCount = count;

            this.AddIfNotPresentInternal(other, ref count);

            if (prevCount < count)
                this.version()++;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.ExceptWith(other);
                return;
            }

            ref int count = ref this.item_count();
            if (count == 0)
                return;

            if (Equals(other, this))
            {
                this.ClearInternal(ref count);
                this.version()++;
                return;
            }

            int prevCount = count;

            foreach (T item in other)
                this.RemoveInternal(m_serializer.Serialize(item), ref count);

            if (prevCount > count)
                this.version()++;
        }

        public unsafe void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.IntersectWith(other);
                return;
            }

            ref int count = ref this.item_count();
            if (count == 0)
                return;

            bool* intersectedItems = stackalloc bool[m_itemCapacity];
            foreach (T item in other)
            {
                int index = this.IndexOfInternal(m_serializer.Serialize(item));
                intersectedItems[index] = index >= 0;
            }

            int previousCount = count;

            for (int i = 0; i < m_itemCapacity; i++)
            {
                Node node = this.item(i);
                if (node.exists && !intersectedItems[i])
                {
                    this.RemoveInternal(node.value, ref count);
                }
            }

            if (previousCount > count)
                this.version()++;
        }

        public unsafe void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.SymmetricExceptWith(other);
                return;
            }

            ref int count = ref this.item_count();
            int prevCount = count;

            if (count == 0)
            {
                this.AddIfNotPresentInternal(other, ref count);
                if (prevCount < count) this.version()++;
                return;
            }

            if (Equals(other, this))
            {
                this.ClearInternal(ref count);
                this.version()++;
                return;
            }

            bool* itemsToRemove = stackalloc bool[m_itemCapacity];
            bool* itemsAddedFromOther = stackalloc bool[m_itemCapacity];

            foreach (T item in other)
            {
                bool added = this.AddIfNotPresentInternal(m_serializer.Serialize(item), ref count, out int index);
                if (added)
                {
                    itemsAddedFromOther[index] = true;
                }
                else if (!itemsAddedFromOther[index])
                {
                    itemsToRemove[index] = true;
                }
            }

            for (int i = 0; i < m_itemCapacity; i++)
            {
                if (itemsToRemove[i])
                {
                    Node node = this.item(i);
                    if (node.exists)
                    {
                        this.RemoveInternal(node.value, ref count);
                    }
                }
            }

            this.version()++;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            foreach (T item in this)
            {
                if (arrayIndex == array.Length)
                {
                    return;
                }

                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public void CopyTo(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int i = 0;

            foreach (T item in this)
            {
                if (i == array.Length)
                {
                    return;
                }

                array[i] = item;
                i++;
            }
        }

        public void ReplaceTo(params T[] other)
        {
            this.ReplaceTo((IEnumerable<T>) other);
        }

        public void ReplaceTo(IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.Clear();
                m_localSet.UnionWith(other);
                return;
            }

            ref int count = ref this.item_count();
            if (count > 0)
            {
                this.ClearInternal(ref count);
            }

            this.AddIfNotPresentInternal(other, ref count);
            this.version()++;
        }

        public void ReplaceTo(T other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (!m_agent.IsActive)
            {
                m_localSet.Clear();
                m_localSet.Add(other);
                return;
            }

            ref int count = ref this.item_count();
            if (count > 0)
            {
                this.ClearInternal(ref count);
            }

            this.AddIfNotPresentInternal(m_serializer.Serialize(other), ref count, out _);
            this.version()++;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (m_agent.IsActive)
                this.SyncLocalSet();

            return m_localSet.SetEquals(other);
        }

        #region Internal

        internal struct Node
        {
            public TRaw value;
            public int next;
            public bool exists;

            public override string ToString()
            {
                return $"{nameof(value)}: {value}, {nameof(next)}: {next}, {nameof(exists)}: {exists}";
            }
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly NetworkSetManaged<T, TRaw> set;
            private int index;
            private TRaw current;

            public T Current => this.set.m_serializer.Deserialize(this.current);
            object IEnumerator.Current => this.set.m_serializer.Deserialize(this.current);

            internal Enumerator(NetworkSetManaged<T, TRaw> set)
            {
                this.set = set;
                this.index = 0;
                this.current = default;
            }

            public bool MoveNext()
            {
                while (this.index < this.set.m_itemCapacity)
                {
                    ref Node node = ref this.set.item(this.index++);
                    if (node.exists)
                    {
                        this.current = node.value;
                        return true;
                    }
                }

                this.index = this.set.m_itemCapacity;
                this.current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.current = default;
            }

            public void Dispose()
            {
                //Do nothing...
            }
        }

        private bool ContainsInternal(TRaw item)
        {
            int index = this.bucket(item);
            while (index >= 0)
            {
                Node node = this.item(index);
                if (node.exists && COMPARER.Equals(node.value, item))
                    return true;

                index = node.next;
            }

            return false;
        }

        private void AddIfNotPresentInternal(IEnumerable<T> other, ref int count)
        {
            foreach (T item in other)
            {
                this.AddIfNotPresentInternal(m_serializer.Serialize(item), ref count, out _);
            }
        }

        private bool AddIfNotPresentInternal(TRaw item, ref int count, out int index)
        {
            index = this.IndexOfInternal(item);
            if (index >= 0)
                return false;

            ref int bucket = ref this.bucket(item);
            ref int freeList = ref free_list();

            if (freeList >= 0)
            {
                index = freeList;
                freeList = this.item(freeList).next;
            }
            else
            {
                index = count;
            }

            this.item(index) = new Node
            {
                value = item,
                next = bucket,
                exists = true
            };

            bucket = index;
            count++;

            return true;
        }

        private bool RemoveInternal(TRaw item, ref int count)
        {
            ref int bucket = ref this.bucket(item);

            int index = bucket;
            int last = UNDEFINED;

            while (index >= 0)
            {
                ref Node node = ref this.item(index);

                if (COMPARER.Equals(node.value, item))
                {
                    if (last < 0)
                        bucket = node.next;
                    else
                        this.item(last).next = node.next;

                    ref int freeList = ref this.free_list();

                    node.next = freeList;
                    node.exists = false;

                    count--;
                    freeList = count == 0 ? UNDEFINED : index;
                    return true;
                }

                last = index;
                index = node.next;
            }

            return false;
        }

        // ReSharper disable once RedundantAssignment
        private void ClearInternal(ref int count)
        {
            count = 0;
            this.FreeBuckets();
            this.FreeItems();
            this.free_list() = UNDEFINED;
        }

        private int IndexOfInternal(TRaw item)
        {
            int index = this.bucket(item);
            while (index >= 0)
            {
                Node node = this.item(index);
                if (node.exists && COMPARER.Equals(node.value, item))
                    return index;

                index = node.next;
            }

            return UNDEFINED;
        }

        private void SyncLocalSet()
        {
            m_localSet.Clear();
            m_localSet.UnionWith(this);
        }

        private void FreeBuckets()
        {
            for (int i = 0; i < m_itemCapacity; i++)
            {
                this.bucket(i) = UNDEFINED;
            }
        }

        private void FreeItems()
        {
            for (int i = 0; i < m_itemCapacity; i++)
            {
                this.item(i).exists = false;
            }
        }

        private ref int item_count()
        {
            return ref m_agent.GetState<int>(m_itemCountPtr);
        }

        internal ref Node item(int index)
        {
            return ref m_agent.GetState<Node>(m_itemArrayPtr + index * m_agent.Facade.SizeOf<Node>());
        }

        internal ref int bucket(TRaw value)
        {
            int index = (int) ((uint) value.GetHashCode() % (uint) m_itemCapacity);
            return ref m_agent.GetState<int>(m_bucketsPtr + index * m_agent.Facade.SizeOf<int>());
        }

        private ref int bucket(int index)
        {
            return ref m_agent.GetState<int>(m_bucketsPtr + index * m_agent.Facade.SizeOf<int>());
        }

        internal ref int free_list()
        {
            return ref m_agent.GetState<int>(m_freeListPtr);
        }

        private ref int version()
        {
            return ref m_agent.GetState<int>(m_versionPtr);
        }

        private ref int event_required()
        {
            return ref m_agent.GetState<int>(m_eventRequiredPtr);
        }

        private ref Event event_arg(int index)
        {
            return ref m_agent.GetState<Event>(m_eventQueuePtr + index * m_agent.Facade.SizeOf<Event>());
        }

        #endregion

        private void SetupItems()
        {
            int index = 0;
            foreach (T value in m_localSet)
            {
                TRaw item = m_serializer.Serialize(value);
                ref int bucket = ref this.bucket(item);

                this.item(index) = new Node
                {
                    value = item,
                    next = bucket,
                    exists = true
                };

                bucket = index;
                index++;
            }
        }

        private enum EventType
        {
            ADDED = 0,
            REMOVED = 1
        }

        private readonly struct Event
        {
            public readonly EventType type;
            public readonly TRaw value;

            public Event(EventType type, TRaw value)
            {
                this.type = type;
                this.value = value;
            }
        }

        private void SendEvent(EventType type, TRaw value)
        {
            int eventIndex = this.event_required()++ % m_eventCapacity;
            this.event_arg(eventIndex) = new Event(type, value);
        }

        private void ConsumeEvents()
        {
            int required = this.event_required();
            int consumed = m_eventConsumed;

            if (consumed >= required)
            {
                return;
            }

            for (int i = Math.Max(consumed, required - m_eventCapacity); i < required; i++)
            {
                Event evt = this.event_arg(i % m_eventCapacity);
                this.ConsumeEvent(evt);
            }

            m_eventConsumed = required;
        }

        private void ConsumeEvent(Event evt)
        {
            TRaw item = evt.value;

            switch (evt.type)
            {
                case EventType.ADDED:
                    this.OnItemAdded?.Invoke(m_serializer.Deserialize(item));
                    break;

                case EventType.REMOVED:
                    this.OnItemRemoved?.Invoke(m_serializer.Deserialize(item));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConsumeVersion()
        {
            int currentVersion = this.version();
            if (m_versionConsumed != currentVersion)
            {
                m_versionConsumed = currentVersion;
                this.OnStateChanged?.Invoke();
            }
        }
    }
}