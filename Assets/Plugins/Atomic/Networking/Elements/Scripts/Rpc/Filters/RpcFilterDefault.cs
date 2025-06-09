namespace Atomic.Networking
{
    public sealed class RpcFilterDefault : IRpcFilter
    {
        public static readonly RpcFilterDefault Instance = new();
        
        public bool Matches(int player) => true;
    }
}