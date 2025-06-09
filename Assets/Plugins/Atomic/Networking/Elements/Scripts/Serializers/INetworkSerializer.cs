namespace Atomic.Networking.Elements
{
    public interface INetworkSerializer<T, TRaw> 
        where TRaw : unmanaged
    {
        TRaw Serialize(in T value);
        
        T Deserialize(in TRaw raw);
    }
}