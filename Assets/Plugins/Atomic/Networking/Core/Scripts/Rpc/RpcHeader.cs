namespace Atomic.Networking
{
    public readonly struct RpcHeader
    {
        public readonly int instance;
        public readonly int ptr;

        public bool IsStatic => this.instance <= 0;

        public RpcHeader(in int instance, in int ptr)
        {
            this.instance = instance;
            this.ptr = ptr;
        }
        
        public static RpcHeader Static(in int method) => new(0, method);
    }
}