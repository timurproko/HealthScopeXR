using System.Collections.Generic;

namespace Atomic.Networking
{
    public sealed partial class MockNetworkObject : INetworkObject
    {
        public bool TryGetSnapshots<T>(in int ptr, ref SnapshotSpan<T> previous, ref SnapshotSpan<T> current, out float alpha) where T : unmanaged
        {
            throw new System.NotImplementedException();

        }

        public INetworkFacade Facade { get; set; }
        public int NetworkId { get; set; }
        public float RenderTime { get; set; }

        public int LocalPlayer => this.Facade.LocalPlayer;
        public IReadOnlyCollection<int> ActivePlayers => this.Facade.ActivePlayers;

        public int Tick => this.Facade.Tick;
        public bool IsClient => this.Facade.IsClient;
        public bool IsServer => this.Facade.IsServer;

        public bool IsHostPlayer(int player) => this.Facade.IsHostPlayer(player);

        public bool TryGetHostPlayer(out int player) => this.Facade.TryGetHostPlayer(out player);

        public T Provide<T>()
        {
            throw new System.NotImplementedException();
        }

        public bool TryProvide<T>(out T value)
        {
            throw new System.NotImplementedException();
        }
    }
}