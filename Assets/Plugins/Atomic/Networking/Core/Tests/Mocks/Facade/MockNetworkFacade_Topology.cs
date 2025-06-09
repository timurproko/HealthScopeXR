namespace Atomic.Networking
{
    public sealed partial class MockNetworkFacade
    {
        public bool IsServer { get; set; }
        public bool IsClient { get; set; }
        public bool IsPlayer { get; set; }
    }
}