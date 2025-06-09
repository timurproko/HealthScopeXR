namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        int SizeOf<T>(int count = 1) where T : unmanaged;

        ref T GetState<T>(int ptr = 0) where T : unmanaged;

        int AllocState(int size);

        int AllocState<T>(int size = 1) where T : unmanaged;

        void FreeState(int ptr, int size);

        void FreeState<T>(int ptr, int size = 1) where T : unmanaged;

        void ReplicateToAll(bool replicate);

        void ReplicateTo(in int player, bool replicate);

        void CopyStateFrom(INetworkObject source);

        void ResetState();
    }
}