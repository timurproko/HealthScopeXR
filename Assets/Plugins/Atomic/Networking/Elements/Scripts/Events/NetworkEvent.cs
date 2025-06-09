using System;
using Atomic.Elements;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

// ReSharper disable UseDeconstructionOnParameter

namespace Atomic.Networking.Elements
{
    public class NetworkEvent : IEvent, IDisposable,
        INetworkObject.ISpawned,
        INetworkObject.IRender
    {
        public event Action OnEvent;

        private readonly INetworkObject _object;
        private readonly int _requiredPtr;

        private int consumed;

        private int required
        {
            get => _object.GetState<int>(_requiredPtr);
            set => _object.GetState<int>(_requiredPtr) = value;
        }

        public NetworkEvent(in INetworkObject obj)
        {
            _object = obj ?? throw new ArgumentNullException(nameof(obj));
            _requiredPtr = obj.AllocState<int>();

            _object.AddListener(this);
        }

        public void Dispose()
        {
            _object.RemoveListener(this);
            this.UnsubscribeAll();
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke()
        {
            if (_object.IsActive)
                this.required++;
        }

        public void Subscribe(Action action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action action)
        {
            this.OnEvent -= action;
        }

        public void UnsubscribeAll()
        {
            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action) delegates[i];

            this.OnEvent = null;
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

            for (int i = this.consumed; i < required; i++)
                this.OnEvent?.Invoke();

            this.consumed = required;
        }
    }

    public class NetworkEvent<T> : NetworkEventManaged<T, T> where T : unmanaged
    {
        public NetworkEvent(in INetworkObject obj, in int bufferCapacity = 4) :
            base(in obj, NetworkSerializer<T>.Instance, in bufferCapacity)
        {
        }
    }

    public sealed class NetworkEvent<T1, T2> : NetworkEventManaged<T1, T2, T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public NetworkEvent(in INetworkObject obj, in int bufferCapacity = 4) :
            base(in obj, NetworkSerializer<T1>.Instance, NetworkSerializer<T2>.Instance, in bufferCapacity)
        {
        }
    }

    public sealed class NetworkEvent<T1, T2, T3> : NetworkEventManaged<T1, T2, T3, T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public NetworkEvent(in INetworkObject obj, in int bufferCapacity = 4) :
            base(in obj,
                NetworkSerializer<T1>.Instance,
                NetworkSerializer<T2>.Instance,
                NetworkSerializer<T3>.Instance,
                in bufferCapacity)
        {
        }
    }
}