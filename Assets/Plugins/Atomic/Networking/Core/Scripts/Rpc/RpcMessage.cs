using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Atomic.Networking
{
    public readonly struct RpcMessage
    {
        public int Tick => _tick;
        public bool Reliable => _reliable;
        public int SourcePlayer => _sourcePlayer;
        public bool IsLocal => _isLocal;

        private readonly byte[] _body;
        private readonly int _tick;
        private readonly bool _reliable;
        private readonly int _sourcePlayer;
        private readonly bool _isLocal;

        public RpcMessage([CanBeNull] in byte[] body, in int tick, in bool reliable, int sourcePlayer, bool isLocal)
        {
            _body = body;
            _tick = tick;
            _reliable = reliable;
            _sourcePlayer = sourcePlayer;
            _isLocal = isLocal;
        }

        public unsafe bool TryReadBody<T>(out T value) where T : unmanaged
        {
            value = default;

            if (_body == null)
                return false;

            if (_body.Length != sizeof(T))
                return false;

            fixed (byte* pointer = _body)
                value = *(T*) pointer;

            return true;
        }

        public unsafe T ReadBody<T>() where T : unmanaged
        {
            Assert.IsTrue(_body != null, "Body is null!");
            Assert.IsTrue(_body != null && _body.Length == sizeof(T),
                $"Mismatch {typeof(T).Name} type! expected bytes: {sizeof(T)}, actual bytes: {_body.Length}");

            fixed (byte* pointer = _body)
                return *(T*) pointer;
        }

        public override string ToString() => $"{nameof(_sourcePlayer)}: {_sourcePlayer}, " +
                                             $"{nameof(_body)}: {_body?.Length}, " +
                                             $"{nameof(_tick)}: {_tick}, " +
                                             $"{nameof(_reliable)}: {_reliable}";
    }
}