namespace Atomic.Networking
{
    public interface IRpcHandler
    { 
        void Handle(in RpcMessage data);
    }
}