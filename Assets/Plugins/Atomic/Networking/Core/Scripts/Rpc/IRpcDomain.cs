using System.Collections.Generic;

namespace Atomic.Networking
{
    public interface IRpcDomain
    {
        int LocalPlayer { get; }
        
        IReadOnlyCollection<int> ActivePlayers { get; }
        
        int Tick { get; }
        
        bool IsServer { get; }
        
        bool IsClient { get; }

        bool IsHostPlayer(int player);

        bool TryGetHostPlayer(out int i);

        void SendRpc(
            in int ptr,
            in int player,
            in bool reliable,
            in bool hostMode,
            in bool tickAligned
        );

        void SendRpc<T>(
            in int ptr,
            in int player,
            in T data,
            in bool reliable,
            in bool hostMode,
            in bool tickAligned
        ) where T : unmanaged;

        int AddRpcHandler(IRpcHandler handler);
        
        bool RemoveRpcHandler(int ptr);
    }
}