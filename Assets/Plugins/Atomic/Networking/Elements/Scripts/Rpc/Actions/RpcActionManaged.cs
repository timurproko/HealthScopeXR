using System;

namespace Atomic.Networking.Elements
{
    public class RpcActionManaged<T, TRaw> : RpcBase<T, TRaw> where TRaw : unmanaged
    {
        private readonly Action<RpcMessage, T> _action;

        public RpcActionManaged(
            in IRpcDomain domain,
            in INetworkSerializer<T, TRaw> serializer,
            in Action<RpcMessage, T> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            serializer,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        ) => _action = action ?? throw new ArgumentNullException(nameof(action));

        protected override void Invoke(in RpcMessage data, in T arg) => _action.Invoke(data, arg);
        
        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T, TRaw> _serializer;
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
                _serializer = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain ?? throw new ArgumentNullException(nameof(domain));
                return this;
            }

            public Builder WithSerializer(INetworkSerializer<T, TRaw> serializer)
            {
                _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }
            
            public Builder WithAction(Action<T> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                _action = (_, arg) => action.Invoke(arg);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
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

            public Builder SetReliable(bool value)
            {
                _reliable = value;
                return this;
            }

            public Builder SetInvokeLocal(bool value)
            {
                _invokeLocal = value;
                return this;
            }

            public Builder SetTickAligned(bool value)
            {
                _tickAligned = value;
                return this;
            }

            public Builder SetHostMode(bool value)
            {
                _hostMode = value;
                return this;
            }

            public RpcActionManaged<T, TRaw> Build()
            {
                if (_domain == null) throw new InvalidOperationException("Entity must be provided.");
                if (_serializer == null) throw new InvalidOperationException("Serializer must be provided.");
                if (_action == null) throw new InvalidOperationException("Action must be provided.");

                return new RpcActionManaged<T, TRaw>(
                    _domain,
                    _serializer,
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
    
    public class RpcActionManaged<T1, T2, TRaw1, TRaw2> : RpcBase<T1, T2, TRaw1, TRaw2> 
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
    {
        private readonly Action<RpcMessage, T1, T2> _action;

        public RpcActionManaged(
            in IRpcDomain domain,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in Action<RpcMessage, T1, T2> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            serializer1,
            serializer2,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        ) => _action = action ?? throw new ArgumentNullException(nameof(action));

        protected override void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2) => _action.Invoke(data, arg1, arg2);
        
        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T1, TRaw1> _serializer1;
            private INetworkSerializer<T2, TRaw2> _serializer2;
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
                _serializer1 = null,
                _serializer2 = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain ?? throw new ArgumentNullException(nameof(domain));
                return this;
            }

            public Builder WithSerializer1(INetworkSerializer<T1, TRaw1> serializer)
            {
                _serializer1 = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }

            public Builder WithSerializer2(INetworkSerializer<T2, TRaw2> serializer)
            {
                _serializer2 = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }
            
            public Builder WithAction(Action<T1, T2> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                _action = (_, arg1, arg2) => action.Invoke(arg1, arg2);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T1, T2> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
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

            public Builder SetReliable(bool value)
            {
                _reliable = value;
                return this;
            }

            public Builder SetInvokeLocal(bool value)
            {
                _invokeLocal = value;
                return this;
            }

            public Builder SetTickAligned(bool value)
            {
                _tickAligned = value;
                return this;
            }

            public Builder SetHostMode(bool value)
            {
                _hostMode = value;
                return this;
            }

            public RpcActionManaged<T1, T2, TRaw1, TRaw2> Build()
            {
                if (_domain == null) throw new InvalidOperationException("Entity must be provided.");
                if (_serializer1 == null) throw new InvalidOperationException("Serializer 1 must be provided.");
                if (_serializer2 == null) throw new InvalidOperationException("Serializer 2 must be provided.");
                if (_action == null) throw new InvalidOperationException("Action must be provided.");

                return new RpcActionManaged<T1, T2, TRaw1, TRaw2>(
                    _domain,
                    _serializer1,
                    _serializer2,
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


    public class RpcActionManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3> : RpcBase<T1, T2, T3, TRaw1, TRaw2, TRaw3>
        where TRaw1 : unmanaged
        where TRaw2 : unmanaged
        where TRaw3 : unmanaged
    {
        private readonly Action<RpcMessage, T1, T2, T3> _action;

        public RpcActionManaged(
            in IRpcDomain domain,
            in INetworkSerializer<T1, TRaw1> serializer1,
            in INetworkSerializer<T2, TRaw2> serializer2,
            in INetworkSerializer<T3, TRaw3> serializer3,
            in Action<RpcMessage, T1, T2, T3> action,
            in IRpcFilter sourceFilter = null,
            in IRpcFilter targetFilter = null,
            in bool reliable = true,
            in bool invokeLocal = true,
            in bool tickAligned = true,
            in bool hostMode = true
        ) : base(
            domain,
            serializer1,
            serializer2,
            serializer3,
            sourceFilter,
            targetFilter,
            reliable,
            invokeLocal,
            tickAligned,
            hostMode
        ) => _action = action ?? throw new ArgumentNullException(nameof(action));

        protected override void Invoke(in RpcMessage data, in T1 arg1, in T2 arg2, in T3 arg3) =>
            _action.Invoke(data, arg1, arg2, arg3);

        public static Builder StartBuild() => Builder.Start();

        public struct Builder
        {
            private IRpcDomain _domain;
            private INetworkSerializer<T1, TRaw1> _serializer1;
            private INetworkSerializer<T2, TRaw2> _serializer2;
            private INetworkSerializer<T3, TRaw3> _serializer3;
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
                _serializer1 = null,
                _serializer2 = null,
                _serializer3 = null,
                _action = null,
                _sourceFilter = null,
                _targetFilter = null
            };

            public Builder WithDomain(IRpcDomain domain)
            {
                _domain = domain ?? throw new ArgumentNullException(nameof(domain));
                return this;
            }

            public Builder WithSerializer1(INetworkSerializer<T1, TRaw1> serializer)
            {
                _serializer1 = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }

            public Builder WithSerializer2(INetworkSerializer<T2, TRaw2> serializer)
            {
                _serializer2 = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }
            
            public Builder WithSerializer3(INetworkSerializer<T3, TRaw3> serializer)
            {
                _serializer3 = serializer ?? throw new ArgumentNullException(nameof(serializer));
                return this;
            }

            public Builder WithAction(Action<T1, T2, T3> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                _action = (_, arg1, arg2, arg3) => action.Invoke(arg1, arg2, arg3);
                return this;
            }

            public Builder WithAction(Action<RpcMessage, T1, T2, T3> action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
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

            public Builder SetReliable(bool value)
            {
                _reliable = value;
                return this;
            }

            public Builder SetInvokeLocal(bool value)
            {
                _invokeLocal = value;
                return this;
            }

            public Builder SetTickAligned(bool value)
            {
                _tickAligned = value;
                return this;
            }

            public Builder SetHostMode(bool value)
            {
                _hostMode = value;
                return this;
            }

            public RpcActionManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3> Build()
            {
                if (_domain == null) throw new InvalidOperationException("Entity must be provided.");
                if (_serializer1 == null) throw new InvalidOperationException("Serializer 1 must be provided.");
                if (_serializer2 == null) throw new InvalidOperationException("Serializer 2 must be provided.");
                if (_serializer3 == null) throw new InvalidOperationException("Serializer 3 must be provided.");
                if (_action == null) throw new InvalidOperationException("Action must be provided.");

                return new RpcActionManaged<T1, T2, T3, TRaw1, TRaw2, TRaw3>(
                    _domain,
                    _serializer1,
                    _serializer2,
                    _serializer3,
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