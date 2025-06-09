using System;
using Atomic.Elements;

namespace Atomic.Networking.Elements
{
    public interface IRpcEvent : IEvent
    {
        event Action<RpcMessage> OnRpcEvent;
    }

    public interface IRpcEvent<T> : IEvent<T>
    {
        event Action<RpcMessage, T> OnRpcEvent;
    }

    public interface IRpcEvent<T1, T2> : IEvent<T1, T2>
    {
        event Action<RpcMessage, T1, T2> OnRpcEvent;
    }

    public interface IRpcEvent<T1, T2, T3> : IEvent<T1, T2, T3>
    {
        event Action<RpcMessage, T1, T2, T3> OnRpcEvent;
    }
}

