using System;

namespace Atomic.Networking.Elements
{
    public class RpcAction : RpcBase
    {
        private readonly Action<RpcMessage> _action;

        public RpcAction(
            in IRpcDomain domain,
            in Action<RpcMessage> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(domain, sourceFilter, targetFilter, reliable, invokeLocal, tickAligned, hostMode) =>
            _action = action ?? throw new ArgumentNullException(nameof(action));

        protected override void Invoke(in RpcMessage data) =>
            _action.Invoke(data);

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private Action<RpcMessage> _action;
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
                _domain = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithAction(Action action)
            {
                _action = _ => action.Invoke();
                return this;
            }

            public Builder WithAction(Action<RpcMessage> action)
            {
                _action = action;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter filter)
            {
                _sourceFilter = filter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter filter)
            {
                _targetFilter = filter;
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

            public RpcAction Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Agent must be set.");
                if (_action == null)
                    throw new InvalidOperationException("Action must be set.");

                return new RpcAction(
                    _domain,
                    _action,
                    _sourceFilter,
                    _targetFilter,
                    _reliable,
                    _invokeLocal,
                    _tickAligned,
                    _hostMode
                );
            }
        }
    }

    public class RpcAction<T> : RpcActionManaged<T, T> where T : unmanaged
    {
        public RpcAction(
            in IRpcDomain domain,
            in Action<RpcMessage, T> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            NetworkSerializer<T>.Instance,
            action,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
            private Action<RpcMessage, T> _action;
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
                _domain = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithAction(Action<T> action)
            {
                _action = (_, arg) => action.Invoke(arg);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T> action)
            {
                _action = action;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter filter)
            {
                _sourceFilter = filter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter filter)
            {
                _targetFilter = filter;
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

            public RpcAction<T> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Agent must be set.");
                if (_action == null)
                    throw new InvalidOperationException("Action must be set.");

                return new RpcAction<T>(
                    _domain,
                    _action,
                    _sourceFilter,
                    _targetFilter,
                    _reliable,
                    _invokeLocal,
                    _tickAligned,
                    _hostMode
                );
            }
        }
    }

    public class RpcAction<T1, T2> : RpcActionManaged<T1, T2, T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public RpcAction(
            in IRpcDomain domain,
            in Action<RpcMessage, T1, T2> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            NetworkSerializer<T1>.Instance,
            NetworkSerializer<T2>.Instance,
            action,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
            private Action<RpcMessage, T1, T2> _action;
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
                _domain = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithAction(Action<T1, T2> action)
            {
                _action = (_, arg1, arg2) => action.Invoke(arg1, arg2);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T1, T2> action)
            {
                _action = action;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter filter)
            {
                _sourceFilter = filter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter filter)
            {
                _targetFilter = filter;
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

            public RpcAction<T1, T2> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Agent must be set.");
                if (_action == null)
                    throw new InvalidOperationException("Action must be set.");

                return new RpcAction<T1, T2>(
                    _domain,
                    _action,
                    _sourceFilter,
                    _targetFilter,
                    _reliable,
                    _invokeLocal,
                    _tickAligned,
                    _hostMode
                );
            }
        }
    }

    public class RpcAction<T1, T2, T3> : RpcActionManaged<T1, T2, T3, T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public RpcAction(
            in IRpcDomain domain,
            in Action<RpcMessage, T1, T2, T3> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            NetworkSerializer<T1>.Instance,
            NetworkSerializer<T2>.Instance,
            NetworkSerializer<T3>.Instance,
            action,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        )
        {
        }
        
        public new static Builder StartBuild() => Builder.Start();

        public new struct Builder
        {
            private IRpcDomain _domain;
            private Action<RpcMessage, T1, T2, T3> _action;
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
                _domain = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain;
                return this;
            }

            public Builder WithAction(Action<T1, T2, T3> action)
            {
                _action = (_, arg1, arg2, arg3) => action.Invoke(arg1, arg2, arg3);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T1, T2, T3> action)
            {
                _action = action;
                return this;
            }

            public Builder WithSourceFilter(IRpcFilter filter)
            {
                _sourceFilter = filter;
                return this;
            }

            public Builder WithTargetFilter(IRpcFilter filter)
            {
                _targetFilter = filter;
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

            public RpcAction<T1, T2, T3> Build()
            {
                if (_domain == null)
                    throw new InvalidOperationException("Agent must be set.");
                if (_action == null)
                    throw new InvalidOperationException("Action must be set.");

                return new RpcAction<T1, T2, T3>(
                    _domain,
                    _action,
                    _sourceFilter,
                    _targetFilter,
                    _reliable,
                    _invokeLocal,
                    _tickAligned,
                    _hostMode
                );
            }
        }
    }
}