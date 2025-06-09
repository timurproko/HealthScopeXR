namespace Atomic.Networking
{
    public partial interface INetworkObject : IRpcDomain
    {
        INetworkFacade Facade { get; }

        bool IsActive { get; }

        int NetworkId { get; }

        float RenderTime { get; }
    }
}