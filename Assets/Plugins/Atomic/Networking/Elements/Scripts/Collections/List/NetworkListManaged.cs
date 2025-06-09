using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Elements;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Atomic.Networking.Elements
{
    //TODO: SNAPSHOTS
    //TODO: LINKED LIST
    public class NetworkListManaged<T, TRaw> : IReactiveList<T>, IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw : unmanaged
    {
        private static readonly IEqualityComparer<TRaw> COMPARER = EqualityComparer<TRaw>.Default;

        private enum EventType
        {
            UPDATED = 0,
            INSERTED = 1,
            DELETED = 2
        }

        private readonly struct Event
        {
            public readonly EventType type;
            public readonly int index;
            public readonly TRaw raw;

            public Event(EventType type, int index, TRaw raw)
            {
                this.type = type;
                this.index = index;
                this.raw = raw;
            }
        }

        public event StateChangedHandler OnStateChanged;
        public event ChangeItemHandler<T> OnItemChanged;
        public event InsertItemHandler<T> OnItemInserted;
        public event DeleteItemHandler<T> OnItemDeleted;

        public int Capacity => _capacity;
        public int Count => _agent.IsActive ? this.item_count() : _localItems.Count;

        public bool IsReadOnly => false;

        private readonly INetworkObject _agent;
        private readonly INetworkSerializer<T, TRaw> _serializer;

        //Items:
        private readonly int _capacity;
        private readonly int _itemCountPtr;
        private readonly int _itemArrayPtr;
        private readonly List<T> _localItems;

        //Events:
        private readonly int _eventCapacity;
        private readonly int _eventRequiredPtr;
        private readonly int _eventQueuePtr;
        private int _eventConsumed;

        //Version:
        private readonly int _versionPtr;
        private int _versionConsumed;

        public NetworkListManaged(
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

        public NetworkListManaged(
            in INetworkObject agent,
            in INetworkSerializer<T, TRaw> serializer,
            in int capacity,
            in int eventCapacity = 4
        )
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _capacity = capacity <= 0
                ? throw new ArgumentOutOfRangeException($"Item capacity must be more than zero! Actual: {capacity}")
                : capacity;

            _eventCapacity = eventCapacity < 0
                ? throw new ArgumentOutOfRangeException(
                    $"Event capacity shouldn't be a negative! Actual: {eventCapacity}")
                : eventCapacity;
            
            _itemCountPtr = agent.AllocState<int>();
            _itemArrayPtr = agent.AllocState<TRaw>(capacity);

            _eventQueuePtr = agent.AllocState<Event>(eventCapacity);
            _eventRequiredPtr = agent.AllocState<int>();

            _versionPtr = agent.AllocState<int>();
            _localItems = new List<T>(capacity);

            _agent.AddListener(this);
        }

        public void Dispose()
        {
            _agent.RemoveListener(this);
            this.UnsubscribeAll();
        }

        void INetworkObject.ISpawned.OnSpawned()
        {
            int count = _localItems.Count;
            this.item_count() = count;

            for (int i = 0; i < count; i++)
                this.item(i) = _serializer.Serialize(_localItems[i]);

            _eventConsumed = this.event_required();
            _versionConsumed = this.version();
        }

        void INetworkObject.IRender.OnRender()
        {
            this.ConsumeEvents();
            this.ConsumeVersion();
        }

        public void UnsubscribeAll()
        {
            if (this.OnItemChanged != null)
            {
                Delegate[] delegates = this.OnItemChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemChanged -= (ChangeItemHandler<T>) delegates[i];

                this.OnItemChanged = null;
            }

            if (this.OnItemInserted != null)
            {
                Delegate[] delegates = this.OnItemInserted.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemInserted -= (InsertItemHandler<T>) delegates[i];

                this.OnItemInserted = null;
            }

            if (this.OnItemDeleted != null)
            {
                Delegate[] delegates = this.OnItemDeleted.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemDeleted -= (DeleteItemHandler<T>) delegates[i];

                this.OnItemDeleted = null;
            }

            if (this.OnStateChanged != null)
            {
                Delegate[] delegates = this.OnStateChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnStateChanged -= (StateChangedHandler) delegates[i];

                this.OnStateChanged = null;
            }
        }

        public T this[int index]
        {
            get => this.GetValue(index);
            set => this.Update(index, value);
        }

        public void Update(int index, T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            bool isValid = _agent.IsActive;
            int count = isValid ? this.item_count() : _localItems.Count;

            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException($"Index out of range! Index: {index}, Count: {count}");

            if (isValid)
            {
                ref TRaw currentItem = ref this.item(index);
                TRaw newItem = _serializer.Serialize(value);

                if (COMPARER.Equals(currentItem, newItem))
                    return;

                currentItem = newItem;
                this.version()++;
                this.SendEvent(EventType.UPDATED, index, newItem);
            }
            else
            {
                _localItems[index] = value;
            }
        }

        public void Clear()
        {
            if (_agent.IsActive)
            {
                ref int count = ref this.item_count();
                if (count > 0)
                {
                    count = 0;
                    this.version()++;
                }
            }
            else
            {
                _localItems.Clear();
            }
        }

        public bool Contains(T item)
        {
            if (item == null)
                return false;

            if (_agent.IsActive)
            {
                TRaw raw = _serializer.Serialize(item);
                for (int i = 0, count = this.item_count(); i < count; i++)
                    if (COMPARER.Equals(this.item(i), raw))
                        return true;

                return false;
            }

            for (int i = 0, count = _localItems.Count; i < count; i++)
                if (_localItems[i].Equals(item))
                    return true;

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_agent.IsActive)
                for (int i = 0, count = item_count(); i < count; i++)
                    yield return _serializer.Deserialize(this.item(i));
            else
                for (int i = 0, count = _localItems.Count; i < count; i++)
                    yield return _localItems[i];
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool isValid = _agent.IsActive;
            int index = isValid ? this.item_count() : _localItems.Count;

            if (index == _capacity)
                throw new Exception("Can't add item! List is full!");

            if (isValid)
            {
                TRaw rawItem = _serializer.Serialize(item);

                this.item(index) = rawItem;
                this.item_count()++;
                this.version()++;
                this.SendEvent(EventType.INSERTED, index, rawItem);
            }
            else
            {
                _localItems.Add(item);
            }
        }

        public void Insert(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool isValid = _agent.IsActive;

            int count = isValid ? this.item_count() : _localItems.Count;
            if (count == _capacity)
                throw new Exception("Can't insert item! List is full!");

            if (index < 0 || index >= _capacity)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (isValid)
            {
                for (int i = count; i > index; i--)
                    this.item(i) = this.item(i - 1);

                TRaw rawItem = _serializer.Serialize(item);

                this.item(index) = rawItem;
                this.item_count()++;
                this.version()++;
                this.SendEvent(EventType.INSERTED, index, rawItem);
            }
            else
            {
                _localItems.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            bool isValid = _agent.IsActive;
            if (!isValid)
                return _localItems.Remove(item);

            ref int count = ref this.item_count();
            if (count == 0)
                return false;

            TRaw rawItem = _serializer.Serialize(item);

            for (int i = 0; i < count; i++)
            {
                if (!COMPARER.Equals(this.item(i), rawItem))
                    continue;

                count--;

                for (int j = i; j < count; j++)
                    this.item(j) = this.item(j + 1);

                this.version()++;
                this.SendEvent(EventType.DELETED, i, rawItem);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _capacity)
                return;

            bool isValid = _agent.IsActive;
            if (!isValid)
            {
                _localItems.RemoveAt(index);
                return;
            }

            ref int count = ref this.item_count();
            if (count == 0)
                return;

            TRaw item = this.item(index);
            count--;

            for (int j = index; j < count; j++) this.item(j) = this.item(j + 1);

            this.version()++;
            this.SendEvent(EventType.DELETED, index, item);
        }

        public int IndexOf(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!_agent.IsActive)
                return _localItems.IndexOf(item);

            TRaw rawItem = _serializer.Serialize(item);

            for (int i = 0, count = this.item_count(); i < count; i++)
                if (COMPARER.Equals(this.item(i), rawItem))
                    return i;

            return -1;
        }

        public void CopyTo(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (_agent.IsActive)
                for (int i = 0, count = Math.Min(array.Length, this.item_count()); i < count; i++)
                    array[i] = _serializer.Deserialize(this.item(i));
            else
                _localItems.CopyTo(array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (_agent.IsActive)
                for (int i = arrayIndex, count = Math.Min(array.Length, this.item_count()); i < count; i++)
                    array[i] = _serializer.Deserialize(this.item(i));
            else
                _localItems.CopyTo(array);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private T GetValue(int index)
        {
            bool isValid = _agent.IsActive;
            int count = isValid ? this.item_count() : _localItems.Count;

            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException($"Index out of range! Index: {index}, Count: {count}");

            T value = isValid ? _serializer.Deserialize(this.item(index)) : _localItems[index];
            return value;
        }

        private ref int item_count()
        {
            return ref _agent.GetState<int>(_itemCountPtr);
        }

        private ref TRaw item(int index)
        {
            return ref _agent.GetState<TRaw>(_itemArrayPtr + index * _agent.Facade.SizeOf<TRaw>());
        }

        private ref int event_required()
        {
            return ref _agent.GetState<int>(_eventRequiredPtr);
        }

        private ref int version()
        {
            return ref _agent.GetState<int>(_versionPtr);
        }

        private ref Event event_arg(int index)
        {
            return ref _agent.GetState<Event>(_eventQueuePtr + index * _agent.Facade.SizeOf<Event>());
        }

        private void SendEvent(EventType type, int index, TRaw key)
        {
            int eventIndex = this.event_required()++ % _eventCapacity;
            this.event_arg(eventIndex) = new Event(type, index, key);
        }

        private void ConsumeEvents()
        {
            int required = this.event_required();
            int consumed = _eventConsumed;

            if (consumed >= required)
                return;

            for (int i = Math.Max(consumed, required - _eventCapacity); i < required; i++)
            {
                Event evt = this.event_arg(i % _eventCapacity);
                this.ConsumeEvent(evt);
            }

            _eventConsumed = required;
        }

        private void ConsumeEvent(Event evt)
        {
            int index = evt.index;
            TRaw raw = evt.raw;

            switch (evt.type)
            {
                case EventType.UPDATED:
                    this.OnItemChanged?.Invoke(index, _serializer.Deserialize(raw));
                    break;

                case EventType.INSERTED:
                    this.OnItemInserted?.Invoke(index, _serializer.Deserialize(raw));
                    break;

                case EventType.DELETED:
                    this.OnItemDeleted?.Invoke(index, _serializer.Deserialize(raw));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConsumeVersion()
        {
            int currentVersion = this.version();
            if (_versionConsumed != currentVersion)
            {
                _versionConsumed = currentVersion;
                this.OnStateChanged?.Invoke();
            }
        }

#if UNITY_EDITOR && ODIN_INSPECTOR
        private readonly struct DebugElement
        {
            [ShowInInspector]
            public T Value
            {
                get { return _list[_index]; }
                set { _list[_index] = value; }
            }

            private readonly NetworkListManaged<T, TRaw> _list;
            private readonly int _index;

            public DebugElement(NetworkListManaged<T, TRaw> list, int index)
            {
                _index = index;
                _list = list;
            }
        }

        [ListDrawerSettings(
            DraggableItems = false,
            ShowFoldout = false,
            HideAddButton = true,
            HideRemoveButton = true,
            OnBeginListElementGUI = nameof(OnBeginDebugElement)
        )]
        [ShowInInspector]
        private List<DebugElement> DebugElements
        {
            get
            {
                var list = new List<DebugElement>();
                for (int i = 0; i < this.Count; i++)
                    list.Add(new DebugElement(this, i));

                return list;
            }
            set { }
        }

        private void OnBeginDebugElement(int index)
        {
            GUILayout.Label($"Index: {index}");
        }
#endif
    }
}