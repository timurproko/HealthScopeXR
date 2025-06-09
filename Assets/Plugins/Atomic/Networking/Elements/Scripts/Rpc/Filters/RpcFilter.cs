using System;

namespace Atomic.Networking
{
    public sealed class RpcFilter : IRpcFilter
    {
        private readonly Predicate<int> _predicate;

        public RpcFilter(Predicate<int> predicate) => _predicate = predicate;

        public bool Matches(int player) => _predicate.Invoke(player);

        #region Presets

        private const int SERVER_ID = -1;

        public static readonly RpcFilter Client = new(player => player > SERVER_ID);
        
        public static readonly RpcFilter All = new(_ => true);

        public static readonly RpcFilter Server = new(player => player == SERVER_ID);

        public static RpcFilter Host(IRpcDomain facade) => new(facade.IsHostPlayer);

        public static RpcFilter ServerOrHost(IRpcDomain facade) =>
            new(player => player == SERVER_ID || facade.IsHostPlayer(player));

        public static RpcFilter InputAuthority(INetworkObject obj)
        {
            return new RpcFilter(player =>
            {
                if (player == SERVER_ID && obj.TryGetHostPlayer(out int hostPlayer)) player = hostPlayer;
                return obj.InputAuthority == player;
            });
        }

        public static RpcFilter StateAuthority(INetworkObject obj)
        {
            return new RpcFilter(player =>
                obj.StateAuthority == player || player == SERVER_ID || obj.IsHostPlayer(player));
        }

        #endregion
    }
}