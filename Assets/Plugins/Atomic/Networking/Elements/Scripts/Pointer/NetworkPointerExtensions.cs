namespace Atomic.Networking.Elements
{
    public static class NetworkPointerExtensions
    {
        public static NetworkPointer<T> Allocate<T>(this INetworkObject obj, int size = 1) where T : unmanaged =>
            new(in obj, obj.AllocState<T>(size));
    }    
}
