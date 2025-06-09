using System;
using System.Collections.Generic;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Atomic.Networking
{
    public partial interface INetworkFacade
    {
        event Action<int> OnPlayerJoin; 
        event Action<int> OnPlayerLeft;

        new int LocalPlayer { get; }

        new IReadOnlyCollection<int> ActivePlayers { get; }
        
        int PlayerCount { get; }

        new bool IsHostPlayer(int playerId);

        new bool TryGetHostPlayer(out int playerId);
    }
}