namespace Atomic.Networking
{
    public partial interface INetworkFacade
    {
        new bool IsServer { get; }
     
        new bool IsClient { get; }
        
        bool IsPlayer { get; }
    }
}