using System;

namespace Atomic.Networking.Elements
{
    public sealed class NetworkSerializer<TRaw> : INetworkSerializer<TRaw, TRaw> where TRaw : unmanaged
    {
        public static readonly NetworkSerializer<TRaw> Instance = new();

        public TRaw Serialize(in TRaw value) => value;

        public TRaw Deserialize(in TRaw raw) => raw;
    }

    public sealed class NetworkSerializer<T, TRaw> : INetworkSerializer<T, TRaw> where TRaw : unmanaged
    {
        private readonly Func<T, TRaw> _serialize;
        private readonly Func<TRaw, T> _deserialize;

        public NetworkSerializer(Func<T, TRaw> serialize, Func<TRaw, T> deserialize)
        {
            this._serialize = serialize;
            this._deserialize = deserialize;
        }

        public TRaw Serialize(in T value) => _serialize.Invoke(value);

        public T Deserialize(in TRaw raw) => _deserialize.Invoke(raw);
    }
}