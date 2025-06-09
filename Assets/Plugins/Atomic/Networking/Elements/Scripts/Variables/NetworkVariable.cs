using System;

namespace Atomic.Networking.Elements
{
    public class NetworkVariable<T> : NetworkVariableManaged<T, T> where T : unmanaged
    {
        public NetworkVariable(INetworkObject obj, T value = default) :
            base(obj, NetworkSerializer<T>.Instance, value)
        {
        }
        
        public new static Builder StartBuild() => new();

        public new struct Builder
        {
            private INetworkObject _object;
            private T _initialValue;

            public Builder WithObject(INetworkObject obj)
            {
                _object = obj ?? throw new ArgumentNullException(nameof(obj));
                return this;
            }

            public Builder WithInitialValue(T value)
            {
                _initialValue = value;
                return this;
            }

            public NetworkVariable<T> Build()
            {
                if (_object == null)
                    throw new InvalidOperationException("Object must be provided.");
                
                return new NetworkVariable<T>(_object, _initialValue);
            }
        }
    }
}