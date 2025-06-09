namespace Atomic.Networking
{
    public partial interface INetworkFacade : IRpcDomain
    {
        bool IsActive { get; }

        float LocalRenderTime { get; }
        float RemoteRenderTime { get; }
        float LocalAlpha { get; }

        new int Tick { get; }
        bool IsForwardTick { get; }
        bool IsResumulationTick { get; }
        
        float DeltaTime { get; }
        
        int SizeOf<T>(int count = 1) where T : unmanaged;

        T Provide<T>();
        bool TryProvide<T>(out T value);
    }
}