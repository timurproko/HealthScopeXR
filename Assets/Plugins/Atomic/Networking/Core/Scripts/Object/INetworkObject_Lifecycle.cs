namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        public interface IListener
        {
        }

        public interface ISpawned : IListener
        {
            void OnSpawned();
        }

        public interface ISimulate
        {
            void OnSimulate(float deltaTime);
        }

        public interface IRender
        {
            void OnRender();
        }

        public interface IDespawned : IListener
        {
            void OnDespawned();
        }

        bool AddListener(IListener listener);

        bool RemoveListener(IListener listener);

        void EnableFixedUpdate(bool enable);
    }
}