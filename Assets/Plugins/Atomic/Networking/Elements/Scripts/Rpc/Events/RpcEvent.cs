using System;

namespace Atomic.Networking.Elements
{
    public class RpcEvent : RpcBase, IRpcEvent
    {
        public event Action OnEvent;
        public event Action<RpcMessage> OnRpcEvent;

        public RpcEvent(
            in IRpcDomain domain,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(in domain, in sourceFilter, in targetFilter, in reliable, in invokeLocal, in tickAligned, in hostMode)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.OnEvent == null)
                return;

            Delegate[] delegates = this.OnEvent.GetInvocationList();
            for (int i = 0, count = delegates.Length; i < count; i++)
                this.OnEvent -= (Action) delegates[i];

            this.OnEvent = null;
        }

        protected override void Invoke(in RpcMessage data)
        {
            this.OnEvent?.Invoke();
            this.OnRpcEvent?.Invoke(data);
        }

        public void Subscribe(Action action)
        {
            this.OnEvent += action;
        }

        public void Unsubscribe(Action action)
        {
            this.OnEvent -= action;
        }

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
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

            public RpcEvent Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                return new RpcEvent(
                    in _domain,
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

    public class RpcEvent<T> : RpcEventManaged<T, T>, IRpcEvent<T> where T : unmanaged
    {
        public RpcEvent(
            in IRpcDomain domain,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            in domain,
            NetworkSerializer<T>.Instance,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
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

            public RpcEvent<T> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                return new RpcEvent<T>(
                    in _domain,
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

    public class RpcEvent<T1, T2> : RpcEventManaged<T1, T2, T1, T2>, IRpcEvent<T1, T2> 
        where T1 : unmanaged 
        where T2 : unmanaged
    {
        public RpcEvent(
            in IRpcDomain domain,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            in domain,
            NetworkSerializer<T1>.Instance,
            NetworkSerializer<T2>.Instance,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
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

            public RpcEvent<T1, T2> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                return new RpcEvent<T1, T2>(
                    in _domain,
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
    
    public class RpcEvent<T1, T2, T3> : RpcEventManaged<T1, T2, T3, T1, T2, T3>, IRpcEvent<T1, T2, T3> 
        where T1 : unmanaged 
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public RpcEvent(
            in IRpcDomain domain,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            in domain,
            NetworkSerializer<T1>.Instance,
            NetworkSerializer<T2>.Instance,
            NetworkSerializer<T3>.Instance,
            in sourceFilter,
            in targetFilter,
            in reliable,
            in invokeLocal,
            in tickAligned,
            in hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
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

            public RpcEvent<T1, T2, T3> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Domain must be provided.");

                return new RpcEvent<T1, T2, T3>(
                    in _domain,
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