using System;
using System.Collections.Generic;

namespace Atomic.Networking
{
    public sealed partial class MockNetworkFacade
    {
        public event Action<int> OnPlayerJoin;
        public event Action<int> OnPlayerLeft;
        public int LocalPlayer { get; }
        public IReadOnlyCollection<int> ActivePlayers { get; }
        public int PlayerCount { get; }
        public bool IsHostPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetHostPlayer(out int playerId)
        {
            throw new NotImplementedException();
        }
    }
}