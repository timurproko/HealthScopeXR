using System;
using System.Collections;
using System.Collections.Generic;
using Atomic.Elements;
using Sirenix.OdinInspector;
using UnityEngine;

// ReSharper disable JoinNullCheckWithUsage

namespace Atomic.Networking.Elements
{
    public class NetworkArrayManaged<T, TRaw> : INetworkArray<T>,
        IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw : unmanaged
    {
        private static readonly IEqualityComparer<TRaw> COMPARER = EqualityComparer<TRaw>.Default;

        public event ChangeItemHandler<T> OnItemChanged;
        public event StateChangedHandler OnStateChanged;

        public int Length => _capacity;

        private readonly INetworkObject _obj;
        private readonly INetworkSerializer<T, TRaw> _serializer;

        private readonly int _capacity;
        private readonly int _arrayPtr;

        private readonly T[] _localArray;

        public T this[int index]
        {
            get => this.GetValue(in index);
            set => this.SetValue(in index, value);
        }

        public NetworkArrayManaged(
            in INetworkObject obj,
            in INetworkSerializer<T, TRaw> serializer,
            in int capacity,
            in IEnumerable<T> initial
        ) : this(obj, serializer, capacity)
        {
            int i = 0;

            if (initial == null)
                return;

            foreach (T element in initial)
            {
                if (i < capacity) this[i++] = element;
                else break;
            }
        }

        public NetworkArrayManaged(
            in INetworkObject obj,
            in INetworkSerializer<T, TRaw> serializer,
            in int capacity
        )
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _capacity = capacity <= 0
                ? throw new ArgumentOutOfRangeException($"Capacity must be positive! Actual: {capacity}")
                : capacity;

            _arrayPtr = obj.AllocState<TRaw>(capacity);
            _localArray = new T[capacity];

            _obj.AddListener(this);
        }

        public void Dispose()
        {
            _obj.FreeState<TRaw>(_capacity);
            _obj.RemoveListener(this);
            this.UnsubscribeAll();
        }

        void INetworkObject.ISpawned.OnSpawned()
        {
            for (int i = 0; i < _capacity; i++)
                this.element(in i) = _serializer.Serialize(in _localArray[i]);
        }

        void INetworkObject.IRender.OnRender()
        {
            bool stateChanged = false;

            for (int i = 0; i < _capacity; i++)
            {
                ref T previousValue = ref _localArray[i];
                TRaw previousKey = _serializer.Serialize(in previousValue);
                TRaw currentKey = this.element(in i);

                if (COMPARER.Equals(previousKey, currentKey))
                    continue;

                T currentValue = _serializer.Deserialize(in currentKey);
                previousValue = currentValue;
                this.OnItemChanged?.Invoke(i, currentValue);

                stateChanged = true;
            }

            if (stateChanged)
                this.OnStateChanged?.Invoke();
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

            if (this.OnStateChanged != null)
            {
                Delegate[] delegates = this.OnStateChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnStateChanged -= (StateChangedHandler) delegates[i];

                this.OnStateChanged = null;
            }
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        private T GetValue(in int index)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException($"Index out of range! Index: {index}, Length: {_capacity}");

            return _obj.IsActive
                ? _serializer.Deserialize(in this.element(in index))
                : _localArray[index];
        }

        private void SetValue(in int index, T value)
        {
            if (index < 0 || index >= _capacity)
                throw new IndexOutOfRangeException($"Index out of range! Index: {index}, Length: {_capacity}");

            if (!_obj.IsActive)
                _localArray[index] = value;
            else
                this.element(in index) = _serializer.Serialize(in value);
        }

        private ref TRaw element(in int index)
        {
            return ref _obj.GetState<TRaw>(_arrayPtr + _obj.SizeOf<TRaw>(index));
        }

        public struct Enumerator : IEnumerator<T>
        {
            public T Current => _current;
            object IEnumerator.Current => _current;

            private readonly NetworkArrayManaged<T, TRaw> _array;
            private T _current;
            private int _index;

            public Enumerator(in NetworkArrayManaged<T, TRaw> array)
            {
                _array = array;
                _current = default;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _array._capacity)
                    return false;

                _current = _array.GetValue(in _index);
                return true;
            }

            public void Reset()
            {
                _current = default;
                _index = -1;
            }

            public void Dispose()
            {
                //Do nothing...
            }
        }

#if UNITY_EDITOR && ODIN_INSPECTOR

        private readonly struct DebugElement
        {
            [ShowInInspector]
            public T Value
            {
                get { return _array.GetValue(in _index); }
                set { _array.SetValue(in _index, value); }
            }

            private readonly NetworkArrayManaged<T, TRaw> _array;
            private readonly int _index;

            public DebugElement(NetworkArrayManaged<T, TRaw> array, int index)
            {
                _index = index;
                _array = array;
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
        private DebugElement[] DebugElements
        {
            get
            {
                DebugElement[] result = new DebugElement[_capacity];

                if (_obj.IsActive)
                    for (int i = 0; i < _capacity; i++)
                        result[i] = new DebugElement(this, i);
                else
                    for (int i = 0; i < _capacity; i++)
                        result[i] = new DebugElement(this, i);

                return result;
            }
            set { }
        }

        private void OnBeginDebugElement(int index) =>
            GUILayout.Label($"Index: {index}");
#endif

        public bool TryGetSnapshots(in int index, out Snapshot<T> previous, out Snapshot<T> current, out float alpha)
        {
            if (!_obj.TryGetSnapshots<TRaw>(_arrayPtr + _obj.SizeOf<TRaw>(index), out var from, out var to, out alpha))
            {
                previous = default;
                current = default;
                return false;
            }

            previous.tick = from.tick;
            previous.data = _serializer.Deserialize(from.data);
            
            current.tick = to.tick;
            current.data = _serializer.Deserialize(to.data);
            return true;
        }

        public bool TryGetSnapshots(ref SnapshotSpan<T> previous, ref SnapshotSpan<T> current, out float alpha)
        {
            SnapshotSpan<TRaw> from = stackalloc TRaw[_capacity];
            SnapshotSpan<TRaw> to = stackalloc TRaw[_capacity];

            if (!_obj.TryGetSnapshots(_arrayPtr, ref from, ref to, out alpha))
                return false;

            previous.Tick = from.Tick;
            current.Tick = to.Tick;

            for (int i = 0; i < _capacity; i++)
            {
                previous[i] = _serializer.Deserialize(from[i]);
                current[i] = _serializer.Deserialize(to[i]);
            }

            return true;
        }

        public bool TryGetSnapshots(in SnapshotArray<T> previous, in SnapshotArray<T> current, out float alpha)
        {
            SnapshotSpan<TRaw> from = stackalloc TRaw[_capacity];
            SnapshotSpan<TRaw> to = stackalloc TRaw[_capacity];

            if (!_obj.TryGetSnapshots(_arrayPtr, ref from, ref to, out alpha))
                return false;

            previous.Tick = from.Tick;
            current.Tick = to.Tick;

            for (int i = 0; i < _capacity; i++)
            {
                previous[i] = _serializer.Deserialize(from[i]);
                current[i] = _serializer.Deserialize(to[i]);
            }

            return true;
        }
    }
}