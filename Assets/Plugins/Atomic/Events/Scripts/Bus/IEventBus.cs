using System;
using System.Collections.Generic;

namespace Atomic.Events
{
    public interface IEventBus : IDisposable
    {
        IReadOnlyCollection<int> DefinedEvents { get; }

        void Def(in int key);
        void Def<T>(in int key);
        void Def<T1, T2>(in int key);
        void Def<T1, T2, T3>(in int key);

        bool Undef(in int key);
        bool IsDefined(in int key);

        Action Subscribe(in int key, in Action action);
        Action<T> Subscribe<T>(in int key, in Action<T> action);
        Action<T1, T2> Subscribe<T1, T2>(in int key, in Action<T1, T2> action);
        Action<T1, T2, T3> Subscribe<T1, T2, T3>(in int key, in Action<T1, T2, T3> action);
        
        void Unsubscribe(in int key, in Action action);
        void Unsubscribe<T>(in int key, in Action<T> action);
        void Unsubscribe<T1, T2>(in int key, in Action<T1, T2> action);
        void Unsubscribe<T1, T2, T3>(in int key, in Action<T1, T2, T3> action);
        
        void Invoke(in int key);
        void Invoke<T>(in int key, in T arg);
        void Invoke<T1, T2>(in int key, in T1 arg1, in T2 arg2);
        void Invoke<T1, T2, T3>(in int key, in T1 arg1, in T2 arg2, in T3 arg3);
    }
}