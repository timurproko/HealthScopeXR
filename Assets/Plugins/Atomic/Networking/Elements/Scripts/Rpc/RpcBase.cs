using System;
using Atomic.Elements;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Networking.Elements
{
    public abstract class RpcBase : IAction, IRpcHandler, IDisposable
    {
        private const int SERVER_ID = -1;

        private readonly IRpcDomain _domain;
        private readonly int _ptr;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly] 
#endif
        private readonly bool _reliable;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _invokeLocal;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _tickAligned;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _hostMode;

        private readonly IRpcFilter _sourceFilter;
        private readonly IRpcFilter _targetFilter;

        protected RpcBase(
            in IRpcDomain domain,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        )
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _sourceFilter = sourceFilter ?? RpcFilterDefault.Instance;
            _targetFilter = targetFilter ?? RpcFilterDefault.Instance;

            _reliable = reliable;
            _invokeLocal = invokeLocal;
            _tickAligned = tickAligned;
            _hostMode = hostMode;

            _ptr = domain.AddRpcHandler(this);
        }

        public virtual void Dispose()
        {
            _domain.RemoveRpcHandler(_ptr);
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke()
        {
            int localPlayer = _domain.LocalPlayer;
            if (!_sourceFilter.Matches(localPlayer))
                return;

            //Send to players:
            foreach (int player in _domain.ActivePlayers)
            {
                if (!_targetFilter.Matches(player))
                    continue;

                if (player == localPlayer && !_invokeLocal)
                    continue;

                if (_domain.IsHostPlayer(player) && !_hostMode)
                    continue;

                _domain.SendRpc(in _ptr, in player, in _reliable, _hostMode, in _tickAligned);
            }

            //Send to server:
            if (!_hostMode && _targetFilter.Matches(SERVER_ID))
                _domain.SendRpc(in _ptr, SERVER_ID, in _reliable, _hostMode, in _tickAligned);
        }

        void IRpcHandler.Handle(in RpcMessage data)
        {
            if (_sourceFilter.Matches(data.SourcePlayer))
                this.Invoke(data);
        }

        protected abstract void Invoke(in RpcMessage data);
    }

    public abstract class RpcBase<T, TRaw> : IAction<T>, IRpcHandler, IDisposable where TRaw : unmanaged
    {
        private const int SERVER_ID = -1;

        private readonly IRpcDomain _domain;
        private readonly INetworkSerializer<T, TRaw> _serializer;
        private readonly int _ptr;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _reliable;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _invokeLocal;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _tickAligned;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _hostMode;

        private readonly IRpcFilter _sourceFilter;
        private readonly IRpcFilter _targetFilter;

        protected RpcBase(
            in IRpcDomain domain,
            in INetworkSerializer<T, TRaw> serializer,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        )
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _sourceFilter = sourceFilter ?? RpcFilterDefault.Instance;
            _targetFilter = targetFilter ?? RpcFilterDefault.Instance;

            _reliable = reliable;
            _invokeLocal = invokeLocal;
            _tickAligned = tickAligned;
            _hostMode = hostMode;

            _ptr = domain.AddRpcHandler(this);
        }

        public virtual void Dispose()
        {
            _domain.RemoveRpcHandler(_ptr);
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T arg)
        {
            int localPlayer = _domain.LocalPlayer;
            if (!_sourceFilter.Matches(localPlayer))
                return;

            TRaw data = _serializer.Serialize(arg);

            //Send to players:
            foreach (int player in _domain.ActivePlayers)
            {
                if (!_targetFilter.Matches(player))
                    continue;

                if (player == localPlayer && !_invokeLocal)
                    continue;

                if (_domain.IsHostPlayer(player) && !_hostMode)
                    continue;

                _domain.SendRpc(in _ptr, in player, in data, in _reliable, _hostMode, in _tickAligned);
            }

            //Send to server:
            if (!_hostMode && _targetFilter.Matches(SERVER_ID))
                _domain.SendRpc(in _ptr, SERVER_ID, data, in _reliable, _hostMode, in _tickAligned);
        }

        void IRpcHandler.Handle(in RpcMessage data)
        {
            if (_sourceFilter.Matches(data.SourcePlayer))
                this.Invoke(data, _serializer.Deserialize(data.ReadBody<TRaw>()));
        }

        protected abstract void Invoke(in RpcMessage data, in T arg);
    }

    public abstract class RpcBase<T1, T2, TRaw1, TRaw2> : IAction<T1, T2>, IRpcHandler, IDisposable
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
    {
        private const int SERVER_ID = -1;

        private readonly IRpcDomain _domain;
        private readonly INetworkSerializer<T1, TRaw1> _serializer1;
        private readonly INetworkSerializer<T2, TRaw2> _serializer2;
        private readonly int _ptr;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _reliable;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _invokeLocal;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _tickAligned;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _hostMode;

        private readonly IRpcFilter _sourceFilter;
        private readonly IRpcFilter _targetFilter;

        protected RpcBase(
            in IRpcDomain domain,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        )
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _serializer1 = serializer1 ?? throw new ArgumentNullException(nameof(serializer1));
            _serializer2 = serializer2 ?? throw new ArgumentNullException(nameof(serializer2));
            
            _sourceFilter = sourceFilter ?? RpcFilterDefault.Instance;
            _targetFilter = targetFilter ?? RpcFilterDefault.Instance;

            _reliable = reliable;
            _invokeLocal = invokeLocal;
            _tickAligned = tickAligned;
            _hostMode = hostMode;

            _ptr = domain.AddRpcHandler(this);
        }

        public virtual void Dispose()
        {
            _domain.RemoveRpcHandler(_ptr);
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T1 arg1, T2 arg2)
        {
            int localPlayer = _domain.LocalPlayer;
            if (!_sourceFilter.Matches(localPlayer))
                return;

            (TRaw1, TRaw2) data = (_serializer1.Serialize(arg1), _serializer2.Serialize(arg2));

            //Send to players:
            foreach (int player in _domain.ActivePlayers)
            {
                if (!_targetFilter.Matches(player))
                    continue;

                if (player == localPlayer && !_invokeLocal)
                    continue;

                if (_domain.IsHostPlayer(player) && !_hostMode)
                    continue;

                _domain.SendRpc(in _ptr, in player, in data, in _reliable, _hostMode, in _tickAligned);
            }

            //Send to server:
            if (!_hostMode && _targetFilter.Matches(SERVER_ID))
                _domain.SendRpc(in _ptr, SERVER_ID, data, in _reliable, _hostMode, in _tickAligned);
        }

        void IRpcHandler.Handle(in RpcMessage data)
        {
            if (!_sourceFilter.Matches(data.SourcePlayer)) 
                return;

            (TRaw1 raw1, TRaw2 raw2) = data.ReadBody<(TRaw1, TRaw2)>();
            T1 arg1 = _serializer1.Deserialize(raw1);
            T2 arg2 = _serializer2.Deserialize(raw2);
            this.Invoke(data, arg1, arg2);
        }

        protected abstract void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2);
    }

    public abstract class RpcBase<T1, T2, T3, TRaw1, TRaw2, TRaw3> : IAction<T1, T2, T3>, IRpcHandler, IDisposable
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
        where TRaw3 : unmanaged
    {
        private const int SERVER_ID = -1;

        private readonly IRpcDomain _domain;
        private readonly INetworkSerializer<T1, TRaw1> _serializer1;
        private readonly INetworkSerializer<T2, TRaw2> _serializer2;
        private readonly INetworkSerializer<T3, TRaw3> _serializer3;
        private readonly int _ptr;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _reliable;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _invokeLocal;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _tickAligned;

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly bool _hostMode;

        private readonly IRpcFilter _sourceFilter;
        private readonly IRpcFilter _targetFilter;

        protected RpcBase(
            in IRpcDomain domain,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in INetworkSerializer<T3, TRaw3> serializer3,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        )
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            
            _serializer1 = serializer1 ?? throw new ArgumentNullException(nameof(serializer1));
            _serializer2 = serializer2 ?? throw new ArgumentNullException(nameof(serializer2));
            _serializer3 = serializer3 ?? throw new ArgumentNullException(nameof(serializer3));
            
            _sourceFilter = sourceFilter ?? RpcFilterDefault.Instance;
            _targetFilter = targetFilter ?? RpcFilterDefault.Instance;

            _reliable = reliable;
            _invokeLocal = invokeLocal;
            _tickAligned = tickAligned;
            _hostMode = hostMode;

            _ptr = domain.AddRpcHandler(this);
        }

        public virtual void Dispose()
        {
            _domain.RemoveRpcHandler(_ptr);
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            int localPlayer = _domain.LocalPlayer;
            if (!_sourceFilter.Matches(localPlayer))
                return;

            TRaw1 raw1 = _serializer1.Serialize(arg1);
            TRaw2 raw2 = _serializer2.Serialize(arg2);
            TRaw3 raw3 = _serializer3.Serialize(arg3);
            (TRaw1, TRaw2, TRaw3) data = (raw1, raw2, raw3);

            //Send to players:
            foreach (int player in _domain.ActivePlayers)
            {
                if (!_targetFilter.Matches(player))
                    continue;

                if (player == localPlayer && !_invokeLocal)
                    continue;

                if (_domain.IsHostPlayer(player) && !_hostMode)
                    continue;

                _domain.SendRpc(in _ptr, in player, in data, in _reliable, _hostMode, in _tickAligned);
            }

            //Send to server:
            if (!_hostMode && _targetFilter.Matches(SERVER_ID))
                _domain.SendRpc(in _ptr, SERVER_ID, data, in _reliable, _hostMode, in _tickAligned);
        }

        void IRpcHandler.Handle(in RpcMessage data)
        {
            if (!_sourceFilter.Matches(data.SourcePlayer)) 
                return;

            (TRaw1 raw1, TRaw2 raw2, TRaw3 raw3) = data.ReadBody<(TRaw1, TRaw2, TRaw3)>();
            T1 arg1 = _serializer1.Deserialize(raw1);
            T2 arg2 = _serializer2.Deserialize(raw2);
            T3 arg3 = _serializer3.Deserialize(raw3);
            this.Invoke(data, arg1, arg2, arg3);
        }

        protected abstract void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2, in T3 arg3);
    }
}