using System;
using Atomic.Elements;

namespace Atomic.Networking.Elements
{
    public class RpcEventManaged<T, TRaw> : RpcBase<T, TRaw>, IEvent<T> where TRaw : unmanaged
    {
        public event Action<T> OnEvent;
        public event Action<RpcMessage, T> OnRpcEvent;

        public RpcEventManaged(
            in IRpcDomain domain,
            in INetworkSerializer<T, TRaw> serializer,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            in domain,
            in serializer,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T>) delegates[i];

            this.OnEvent = null;
        }

        protected override void Invoke(in RpcMessage data, in T arg)
        {
            this.OnEvent?.Invoke(arg);
            this.OnRpcEvent?.Invoke(data, arg);
        }

        public void Subscribe(Action<T> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            this.OnEvent -= action;
        }

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T, TRaw> _serializer;
            private IRpcFilter _sourceFilter;
            private IRpcFilter _targetFilter;
            private bool _reliable;
            private bool _invokeLocal;
            private bool _tickAligned;
            private bool _hostMode;

            public static Builder Start() => new()
            {
                _reliable = true,
                _invokeLocal = true,
                _tickAligned = true,
                _hostMode = true,
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithSerializer(INetworkSerializer<T, TRaw> serializer)
            {
                _serializer = serializer;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter sourceFilter)
            {
                _sourceFilter = sourceFilter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter targetFilter)
            {
                _targetFilter = targetFilter;
                return this;
            }

            public Builder SetReliable(bool reliable)
            {
                _reliable = reliable;
                return this;
            }

            public Builder SetInvokeLocal(bool invokeLocal)
            {
                _invokeLocal = invokeLocal;
                return this;
            }

            public Builder SetTickAligned(bool tickAligned)
            {
                _tickAligned = tickAligned;
                return this;
            }

            public Builder SetHostMode(bool hostMode)
            {
                _hostMode = hostMode;
                return this;
            }

            public RpcEventManaged<T, TRaw> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                if (_serializer == null)
                    throw new InvalidOperationException("Serializer must be provided.");

                return new RpcEventManaged<T, TRaw>(
                    in _domain,
                    in _serializer,
                    in _sourceFilter,
                    in _targetFilter,
                    in _reliable,
                    in _invokeLocal,
                    in _tickAligned,
                    in _hostMode
                );
            }
        }
    }

    public class RpcEventManaged<T1, T2, TRaw1, TRaw2> : RpcBase<T1, T2, TRaw1, TRaw2>,
        IEvent<T1, T2>
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
    {
        public event Action<T1, T2> OnEvent;
        public event Action<RpcMessage, T1, T2> OnRpcEvent;

        public RpcEventManaged(
            in IRpcDomain domain,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            in domain,
            in serializer1,
            in serializer2,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T1, T2>) delegates[i];

            this.OnEvent = null;
        }

        protected override void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2)
        {
            this.OnEvent?.Invoke(arg1, arg2);
            this.OnRpcEvent?.Invoke(data, arg1, arg2);
        }

        public void Subscribe(Action<T1, T2> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T1, T2> action)
        {
            this.OnEvent -= action;
        }

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T1, TRaw1> _serializer1;
            private INetworkSerializer<T2, TRaw2> _serializer2;
            private IRpcFilter _sourceFilter;
            private IRpcFilter _targetFilter;
            private bool _reliable;
            private bool _invokeLocal;
            private bool _tickAligned;
            private bool _hostMode;

            public static Builder Start() => new()
            {
                _reliable = true,
                _invokeLocal = true,
                _tickAligned = true,
                _hostMode = true,
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithSerializer1(INetworkSerializer<T1, TRaw1> serializer)
            {
                _serializer1 = serializer;
                return this;
            }

            public Builder WithSerializer2(INetworkSerializer<T2, TRaw2> serializer)
            {
                _serializer2 = serializer;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter sourceFilter)
            {
                _sourceFilter = sourceFilter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter targetFilter)
            {
                _targetFilter = targetFilter;
                return this;
            }

            public Builder SetReliable(bool reliable)
            {
                _reliable = reliable;
                return this;
            }

            public Builder SetInvokeLocal(bool invokeLocal)
            {
                _invokeLocal = invokeLocal;
                return this;
            }

            public Builder SetTickAligned(bool tickAligned)
            {
                _tickAligned = tickAligned;
                return this;
            }

            public Builder SetHostMode(bool hostMode)
            {
                _hostMode = hostMode;
                return this;
            }

            public RpcEventManaged<T1, T2, TRaw1, TRaw2> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                if (_serializer1 == null)
                    throw new InvalidOperationException("Serializer1 must be provided.");

                if (_serializer2 == null)
                    throw new InvalidOperationException("Serializer2 must be provided.");

                return new RpcEventManaged<T1, T2, TRaw1, TRaw2>(
                    in _domain,
                    in _serializer1,
                    in _serializer2,
                    in _sourceFilter,
                    in _targetFilter,
                    in _reliable,
                    in _invokeLocal,
                    in _tickAligned,
                    in _hostMode
                );
            }
        }
    }

    public class RpcEventManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3> : RpcBase<T1, T2, T3, TRaw1, TRaw2, TRaw3>,
        IEvent<T1, T2, T3>
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
        where TRaw3 : unmanaged
    {
        public event Action<T1, T2, T3> OnEvent;
        public event Action<RpcMessage, T1, T2, T3> OnRpcEvent;

        public RpcEventManaged(
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
        ) : base(
            in domain,
            in serializer1,
            in serializer2,
            in serializer3,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action<T1, T2, T3>) delegates[i];

            this.OnEvent = null;
        }

        protected override void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2, in T3 arg3)
        {
            this.OnEvent?.Invoke(arg1, arg2, arg3);
            this.OnRpcEvent?.Invoke(data, arg1, arg2, arg3);
        }

        public void Subscribe(Action<T1, T2, T3> action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action<T1, T2, T3> action)
        {
            this.OnEvent -= action;
        }

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T1, TRaw1> _serializer1;
            private INetworkSerializer<T2, TRaw2> _serializer2;
            private INetworkSerializer<T3, TRaw3> _serializer3;
            private IRpcFilter _sourceFilter;
            private IRpcFilter _targetFilter;
            private bool _reliable;
            private bool _invokeLocal;
            private bool _tickAligned;
            private bool _hostMode;

            public static Builder Start() => new()
            {
                _reliable = true,
                _invokeLocal = true,
                _tickAligned = true,
                _hostMode = true
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithSerializer1(INetworkSerializer<T1, TRaw1> serializer)
            {
                _serializer1 = serializer;
                return this;
            }

            public Builder WithSerializer2(INetworkSerializer<T2, TRaw2> serializer)
            {
                _serializer2 = serializer;
                return this;
            }

            public Builder WithSerializer3(INetworkSerializer<T3, TRaw3> serializer)
            {
                _serializer3 = serializer;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter sourceFilter)
            {
                _sourceFilter = sourceFilter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter targetFilter)
            {
                _targetFilter = targetFilter;
                return this;
            }

            public Builder SetReliable(bool reliable)
            {
                _reliable = reliable;
                return this;
            }

            public Builder SetInvokeLocal(bool invokeLocal)
            {
                _invokeLocal = invokeLocal;
                return this;
            }

            public Builder SetTickAligned(bool tickAligned)
            {
                _tickAligned = tickAligned;
                return this;
            }

            public Builder SetHostMode(bool hostMode)
            {
                _hostMode = hostMode;
                return this;
            }

            public RpcEventManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3> Build()
            {
                if (_domain == null) throw new InvalidOperationException("Domain must be provided.");
                if (_serializer1 == null) throw new InvalidOperationException("Serializer1 must be provided.");
                if (_serializer2 == null) throw new InvalidOperationException("Serializer2 must be provided.");
                if (_serializer3 == null) throw new InvalidOperationException("Serializer3 must be provided.");

                return new RpcEventManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3>(
                    in _domain,
                    in _serializer1,
                    in _serializer2,
                    in _serializer3,
                    in _sourceFilter,
                    in _targetFilter,
                    in _reliable,
                    in _invokeLocal,
                    in _tickAligned,
                    in _hostMode
                );
            }
        }
    }
}