using System;
using Atomic.Elements;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Networking.Elements
{
    public class NetworkEventManaged<T, TRaw> : IEvent<T>,
        IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw : unmanaged
    {
        public event Action<T> OnEvent;

        private readonly INetworkObject _object;
        private readonly INetworkSerializer<T, TRaw> _serializer;

        private readonly int _requiredPtr;
        private readonly int _bufferPtr;
        private readonly int _bufferCapacity;

        private int consumed;

        private int required
        {
            get => _object.GetState<int>(_requiredPtr);
            set => _object.GetState<int>(_requiredPtr) = value;
        }

        public NetworkEventManaged(
            in INetworkObject obj,
            in INetworkSerializer<T, TRaw> serializer,
            in int bufferCapacity = 4
        )
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
           
            _bufferCapacity = bufferCapacity <= 0
                ? throw new ArgumentOutOfRangeException($"Buffer capacity should be positive! Actual: {bufferCapacity}")
                : bufferCapacity;

            _requiredPtr = obj.AllocState<int>();
            _bufferPtr = obj.AllocState<TRaw>(bufferCapacity);

            _object.AddListener(this);
        }

        public void Dispose()
        {
            _object.FreeState<int>(_requiredPtr);
            _object.FreeState<TRaw>(_bufferCapacity);
            
            _object.RemoveListener(this);
            this.UnsubscribeAll();
        }
        
        void INetworkObject.ISpawned.OnSpawned()
        {
            this.consumed = this.required;
        }

        void INetworkObject.IRender.OnRender()
        {
            int required = this.required;
            if (required <= this.consumed)
                return;

            for (int i = Math.Max(this.consumed, required - _bufferCapacity); i < required; i++)
            {
                TRaw arg = this.arg(i % _bufferCapacity);
                this.OnEvent?.Invoke(_serializer.Deserialize(in arg));
            }

            this.consumed = required;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T arg)
        {
            if (_object.IsActive)
                this.arg(this.required++ % _bufferCapacity) = _serializer.Serialize(in arg);
        }

        public void Subscribe(Action<T> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            this.OnEvent -= action;
        }

        public void UnsubscribeAll()
        {
            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T>) delegates[i];

            this.OnEvent = null;
        }

        private ref TRaw arg(in int index)
        {
            return ref _object.GetState<TRaw>(_bufferPtr + _object.SizeOf<TRaw>(index));
        }
    }

    public class NetworkEventManaged<T1, T2, TRaw1, TRaw2> :
        IEvent<T1, T2>,
        IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
    {
        public event Action<T1, T2> OnEvent;

        private readonly INetworkObject _object;
        private readonly INetworkSerializer<T1, TRaw1> _serializer1;
        private readonly INetworkSerializer<T2, TRaw2> _serializer2;

        private readonly int _requiredPtr;
        private readonly int _bufferPtr;
        private readonly int _bufferCapacity;

        private int consumed;

        private int required
        {
            get => _object.GetState<int>(_requiredPtr);
            set => _object.GetState<int>(_requiredPtr) = value;
        }

        public NetworkEventManaged(
            in INetworkObject obj,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in int bufferCapacity = 4
        )
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _serializer1 = serializer1 ?? throw new ArgumentNullException(nameof(serializer1));
            _serializer2 = serializer2 ?? throw new ArgumentNullException(nameof(serializer2));

            _bufferCapacity = bufferCapacity <= 0
                ? throw new ArgumentOutOfRangeException($"Capacity should be positive! Actual: {bufferCapacity}")
                : bufferCapacity;

            _requiredPtr = obj.AllocState<int>();
            _bufferPtr = obj.AllocState<(TRaw1, TRaw2)>(bufferCapacity);

            _object.AddListener(this);
        }

        public void Dispose()
        {
            _object.FreeState<int>(_requiredPtr);
            _object.FreeState<(TRaw1, TRaw2)>(_bufferCapacity);
            _object.RemoveListener(this);
            this.UnsubscribeAll();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T1 arg1, T2 arg2)
        {
            if (!_object.IsActive)
                return;

            TRaw1 raw1 = _serializer1.Serialize(arg1);
            TRaw2 raw2 = _serializer2.Serialize(arg2);
            this.args(this.required++ % _bufferCapacity) = (raw1, raw2);
        }

        public void Subscribe(Action<T1, T2> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T1, T2> action)
        {
            this.OnEvent -= action;
        }

        public void UnsubscribeAll()
        {
            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T1, T2>) delegates[i];

            this.OnEvent = null;
        }

        private ref (TRaw1, TRaw2) args(in int index)
        {
            int ptr = _bufferPtr + _object.SizeOf<(TRaw1, TRaw2)>(index);
            return ref _object.GetState<(TRaw1, TRaw2)>(ptr);
        }

        public void OnSpawned()
        {
            this.consumed = this.required;
        }

        public void OnRender()
        {
            int required = this.required;
            if (required <= this.consumed)
                return;

            for (int i = Math.Max(this.consumed, required - _bufferCapacity); i < required; i++)
            {
                (TRaw1 raw1, TRaw2 raw2) = this.args(i % _bufferCapacity);
                T1 arg1 = _serializer1.Deserialize(raw1);
                T2 arg2 = _serializer2.Deserialize(raw2);
                this.OnEvent?.Invoke(arg1, arg2);
            }

            this.consumed = required;
        }
    }

    public class NetworkEventManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3> :
        IEvent<T1, T2, T3>,
        IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
        where TRaw3 : unmanaged
    {
        public event Action<T1, T2, T3> OnEvent;

        private readonly INetworkObject _object;
        private readonly INetworkSerializer<T1, TRaw1> _serializer1;
        private readonly INetworkSerializer<T2, TRaw2> _serializer2;
        private readonly INetworkSerializer<T3, TRaw3> _serializer3;

        private readonly int _requiredPtr;
        private readonly int _bufferPtr;
        private readonly int _bufferCapacity;

        private int consumed;

        private int required
        {
            get { return _object.GetState<int>(_requiredPtr); }
            set { _object.GetState<int>(_requiredPtr) = value; }
        }

        public NetworkEventManaged(
            in INetworkObject obj,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in INetworkSerializer<T3, TRaw3> serializer3,
            in int bufferCapacity = 4
        )
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _serializer1 = serializer1 ?? throw new ArgumentNullException(nameof(serializer1));
            _serializer2 = serializer2 ?? throw new ArgumentNullException(nameof(serializer2));
            _serializer3 = serializer3 ?? throw new ArgumentNullException(nameof(serializer3));

            _bufferCapacity = bufferCapacity <= 0
                ? throw new ArgumentOutOfRangeException($"Capacity should be positive! Actual: {bufferCapacity}")
                : bufferCapacity;

            _requiredPtr = obj.AllocState<int>();
            _bufferPtr = obj.AllocState<(TRaw1, TRaw2, TRaw3)>(bufferCapacity);

            _object.AddListener(this);
        }

        public void Dispose()
        {
            _object.FreeState<int>(_requiredPtr);
            _object.FreeState<(TRaw1, TRaw2, TRaw3)>(_bufferCapacity);
            _object.RemoveListener(this);
            this.UnsubscribeAll();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            if (!_object.IsActive)
                return;
            
            TRaw1 raw1 = _serializer1.Serialize(arg1);
            TRaw2 raw2 = _serializer2.Serialize(arg2);
            TRaw3 raw3 = _serializer3.Serialize(arg3);
            this.args(this.required++ % _bufferCapacity) = (raw1, raw2, raw3);
        }

        public void Subscribe(Action<T1, T2, T3> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T1, T2, T3> action)
        {
            this.OnEvent -= action;
        }

        public void UnsubscribeAll()
        {
            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T1, T2, T3>) delegates[i];

            this.OnEvent = null;
        }

        private ref (TRaw1, TRaw2, TRaw3) args(in int index)
        {
            int ptr = _bufferPtr + _object.SizeOf<(TRaw1, TRaw2, TRaw3)>(index);
            return ref _object.GetState<(TRaw1, TRaw2, TRaw3)>(ptr);
        }

        void INetworkObject.ISpawned.OnSpawned()
        {
            this.consumed = this.required;
        }

        void INetworkObject.IRender.OnRender()
        {
            int required = this.required;
            if (required <= this.consumed)
                return;

            for (int i = Math.Max(this.consumed, required - _bufferCapacity); i < required; i++)
            {
                (TRaw1 raw1, TRaw2 raw2, TRaw3 raw3) = this.args(i % _bufferCapacity);
                T1 arg1 = _serializer1.Deserialize(raw1);
                T2 arg2 = _serializer2.Deserialize(raw2);
                T3 arg3 = _serializer3.Deserialize(raw3);
                this.OnEvent?.Invoke(arg1, arg2, arg3);
            }

            this.consumed = required;
        }
    }
}