// ReSharper disable JoinNullCheckWithUsage

using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Networking.Elements
{
    public class NetworkVariableManaged<T, TRaw> :
        INetworkVariable<T>,
        IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw : unmanaged
    {
        private static readonly EqualityComparer<TRaw> COMPARER = EqualityComparer<TRaw>.Default;

        public event Action<T> OnValueChanged;

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public T Value
        {
            get
            {
                return _object.IsActive
                    ? _serializer.Deserialize(_object.GetState<TRaw>(_valuePtr))
                    : _localValue;
            }
            set
            {
                if (_object.IsActive)
                    _object.GetState<TRaw>(_valuePtr) = _serializer.Serialize(value);
                else _localValue = value;
            }
        }

        private readonly INetworkObject _object;
        private readonly INetworkSerializer<T, TRaw> _serializer;
        private readonly int _valuePtr;

        private T _localValue;

        public NetworkVariableManaged(
            INetworkObject obj,
            INetworkSerializer<T, TRaw> serializer,
            T value = default
        )
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _valuePtr = obj.AllocState<TRaw>();
            _localValue = value;

            _object.AddListener(this);
        }

        public void Dispose()
        {
            _object.FreeState<TRaw>(_valuePtr);
            _object.RemoveListener(this);
            this.UnsubscribeAll();
        }

        public void Subscribe(Action<T> action)
        {
            this.OnValueChanged += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            this.OnValueChanged -= action;
        }

        public void UnsubscribeAll()
        {
            if (this.OnValueChanged == null)
                return;

            Delegate[] delegates = this.OnValueChanged.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnValueChanged -= (Action<T>) delegates[i];

            this.OnValueChanged = null;
        }

        void INetworkObject.ISpawned.OnSpawned()
        {
            _object.GetState<TRaw>(_valuePtr) = _serializer.Serialize(_localValue);
        }

        void INetworkObject.IRender.OnRender()
        {
            TRaw raw = _object.GetState<TRaw>(_valuePtr);
            if (this.AreEquals(raw, _serializer.Serialize(_localValue)))
                return;

            T currentValue = _serializer.Deserialize(raw);
            _localValue = currentValue;
            this.OnValueChanged?.Invoke(currentValue);
        }

        protected virtual bool AreEquals(in TRaw o1, in TRaw o2)
        {
            return COMPARER.Equals(o1, o2);
        }

        public bool TryGetSnapshots(out Snapshot<T> previous, out Snapshot<T> current, out float alpha)
        {
            if (_object.TryGetSnapshots(_valuePtr, out Snapshot<TRaw> from, out Snapshot<TRaw> to, out alpha))
            {
                previous = new Snapshot<T>(from.tick, _serializer.Deserialize(from.data));
                current = new Snapshot<T>(to.tick, _serializer.Deserialize(to.data));
                return true;
            }

            previous = default;
            current = default;
            return false;
        }

        public static Builder StartBuild() => new();

        public struct Builder
        {
            private INetworkObject _object;
            private INetworkSerializer<T, TRaw> _serializer;
            private T _initialValue;

            public Builder WithObject(INetworkObject obj)
            {
                _object = obj ?? throw new ArgumentNullException(nameof(obj));
                return this;
            }

            public Builder WithSerializer(INetworkSerializer<T, TRaw> serializer)
            {
                _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }

            public Builder WithInitialValue(T value)
            {
                _initialValue = value;
                return this;
            }

            public NetworkVariableManaged<T, TRaw> Build()
            {
                if (_object == null)
                    throw new InvalidOperationException("Object must be provided.");

                if (_serializer == null)
                    throw new InvalidOperationException("Serializer must be provided.");

                return new NetworkVariableManaged<T, TRaw>(_object, _serializer, _initialValue);
            }
        }
    }
}