using System;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        private enum EventType
        {
            ADDED = 0,
            REMOVED = 1,
            CHANGED = 2,
        }

        private readonly struct Event
        {
            public readonly EventType type;
            public readonly KRaw key;
            public readonly VRaw value;

            public Event(in EventType type, in KRaw key, in VRaw value)
            {
                this.type = type;
                this.key = key;
                this.value = value;
            }
        }

        private void SendEvent(in EventType type, in K key, in V value)
        {
            int eventIndex = this.event_required()++ % this.m_eventCapacity;
            KRaw rawKey = m_keySerializer.Serialize(in key);
            VRaw rawValue = m_valueSerializer.Serialize(in value);
            this.event_arg(eventIndex) = new Event(in type, in rawKey, in rawValue);
        }

        void INetworkObject.IRender.OnRender()
        {
            this.ConsumeEvents();
            this.ConsumeVersion();
        }

        private void ConsumeEvents()
        {
            int required = this.event_required();
            int consumed = this.m_eventConsumed;

            if (consumed >= required)
                return;

            for (int i = Math.Max(consumed, required - this.m_eventCapacity); i < required; i++)
            {
                Event evt = this.event_arg(i % this.m_eventCapacity);
                this.ConsumeEvent(in evt);
            }

            this.m_eventConsumed = required;
        }

        private void ConsumeEvent(in Event evt)
        {
            K key = m_keySerializer.Deserialize(evt.key);
            V value = m_valueSerializer.Deserialize(evt.value);

            switch (evt.type)
            {
                case EventType.ADDED:
                    this.OnItemAdded?.Invoke(key, value);
                    break;

                case EventType.REMOVED:
                    this.OnItemRemoved?.Invoke(key, value);
                    break;

                case EventType.CHANGED:
                    this.OnItemChanged?.Invoke(key, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConsumeVersion()
        {
            int currentVersion = this.version();
            if (this.m_versionConsumed != currentVersion)
            {
                this.m_versionConsumed = currentVersion;
                this.OnStateChanged?.Invoke();
            }
        }
    }
}