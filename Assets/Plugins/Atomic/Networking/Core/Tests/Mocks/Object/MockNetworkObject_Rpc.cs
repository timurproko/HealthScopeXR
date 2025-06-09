namespace Atomic.Networking
{
    public sealed partial class MockNetworkObject
    {
        public void SendRpc(in int ptr, in int player, in bool reliable, in bool isHost, in bool tickAligned)
        {
            throw new System.NotImplementedException();
        }

        public void SendRpc<T>(in int ptr, in int player, in T data, in bool reliable, in bool isHost, in bool tickAligned) where T : unmanaged
        {
            throw new System.NotImplementedException();
        }
        
        public int AddRpcHandler(IRpcHandler handler)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveRpcHandler(int ptr)
        {
            throw new System.NotImplementedException();
        }

    }
}