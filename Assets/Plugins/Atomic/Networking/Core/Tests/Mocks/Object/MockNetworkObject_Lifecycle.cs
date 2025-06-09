using System.Collections.Generic;

namespace Atomic.Networking
{
    public sealed partial class MockNetworkObject
    {
        private readonly List<INetworkObject.IListener> _listeners = new();

        public bool AddListener(INetworkObject.IListener listener)
        {
            if (_listeners.Contains(listener))
                return false;

            _listeners.Add(listener);

            if (this.IsActive && listener is INetworkObject.ISpawned spawned) 
                spawned.OnSpawned();
            
            return true;
        }

        public bool RemoveListener(INetworkObject.IListener listener)
        {
            return _listeners.Remove(listener);
        }

        public void EnableFixedUpdate(bool enable)
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdateNetwork(float deltaTime)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                if (_listeners[i] is INetworkObject.ISimulate fixedUpdate)
                    fixedUpdate.OnSimulate(deltaTime);
        }

        public void Render()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                if (_listeners[i] is INetworkObject.IRender render)
                    render.OnRender();
        }

        public void Mock_Despawn()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                if (_listeners[i] is INetworkObject.IDespawned despawned)
                    despawned.OnDespawned();
        }
        
        public void Mock_Spawn()
        {
            this.AllocStateBuffer();
            for (int i = _listeners.Count - 1; i >= 0; i--)
                if (_listeners[i] is INetworkObject.ISpawned spawned)
                    spawned.OnSpawned();
        }
    }
}