using System;
using System.Collections.Generic;

namespace Atomic.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<int, IEvent> _events = new();

        public IReadOnlyCollection<int> DefinedEvents => _events.Keys;

        #region Def

        public void Def(in int key)
        {
            if (_events.ContainsKey(key))
                throw new Exception($"Event {key} is already declared!");

            _events.Add(key, new Event());
        }

        public void Def<T>(in int key)
        {
            if (_events.ContainsKey(key))
                throw new Exception($"Key {key} is already declared!");

            _events.Add(key, new Event<T>());
        }

        public void Def<T1, T2>(in int key)
        {
            if (_events.ContainsKey(key))
                throw new Exception($"Key {key} is already declared!");

            _events.Add(key, new Event<T1, T2>());
        }

        public void Def<T1, T2, T3>(in int key)
        {
            if (_events.ContainsKey(key))
                throw new Exception($"Key {key} is already declared!");

            _events.Add(key, new Event<T1, T2, T3>());
        }

        public bool IsDefined(in int key)
        {
            return _events.ContainsKey(key);
        }

        public bool Undef(in int key)
        {
            return _events.Remove(key);
        }

        #endregion

        #region Subscribe

        public Action Subscribe(in int key, in Action action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event evt = (Event) ptr;
            evt.Delegate += action;

            return action;
        }

        public Action<T> Subscribe<T>(in int key, in Action<T> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T> evt = (Event<T>) ptr;
            evt.Delegate += action;

            return action;
        }

        public Action<T1, T2> Subscribe<T1, T2>(in int key, in Action<T1, T2> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T1, T2> evt = (Event<T1, T2>) ptr;
            evt.Delegate += action;

            return action;
        }

        public Action<T1, T2, T3> Subscribe<T1, T2, T3>(in int key, in Action<T1, T2, T3> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T1, T2, T3> evt = (Event<T1, T2, T3>) ptr;
            evt.Delegate += action;

            return action;
        }

        

        #endregion

        #region Unsubscribe

        public void Unsubscribe(in int key, in Action action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                return;

            Event evt = (Event) ptr;
            evt.Delegate -= action;
        }

        public void Unsubscribe<T>(in int key, in Action<T> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                return;

            Event<T> evt = (Event<T>) ptr;
            evt.Delegate -= action;
        }

        public void Unsubscribe<T1, T2>(in int key, in Action<T1, T2> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                return;

            Event<T1, T2> evt = (Event<T1, T2>) ptr;
            evt.Delegate -= action;
        }

        public void Unsubscribe<T1, T2, T3>(in int key, in Action<T1, T2, T3> action)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                return;

            Event<T1, T2, T3> evt = (Event<T1, T2, T3>) ptr;
            evt.Delegate -= action;
        }

        #endregion

        #region Invoke

        public void Invoke(in int key)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event evt = (Event) ptr;
            evt.Delegate?.Invoke();
        }

        public void Invoke<T>(in int key, in T arg)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T> evt = (Event<T>) ptr;
            evt.Delegate?.Invoke(arg);
        }

        public void Invoke<T1, T2>(in int key, in T1 arg1, in T2 arg2)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T1, T2> evt = (Event<T1, T2>) ptr;
            evt.Delegate?.Invoke(arg1, arg2);
        }

        public void Invoke<T1, T2, T3>(in int key, in T1 arg1, in T2 arg2, in T3 arg3)
        {
            if (!_events.TryGetValue(key, out IEvent ptr))
                throw new Exception($"Event {key} is not declared yet!");

            Event<T1, T2, T3> evt = (Event<T1, T2, T3>) ptr;
            evt.Delegate?.Invoke(arg1, arg2, arg3);
        }

        #endregion

        public void Dispose()
        {
            foreach (IEvent e in _events.Values)
                e.Dispose();

            _events.Clear();
        }
    }
}