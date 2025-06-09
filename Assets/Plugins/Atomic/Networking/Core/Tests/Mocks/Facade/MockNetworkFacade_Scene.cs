using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atomic.Networking
{
    public partial class MockNetworkFacade
    {
        public PhysicsScene GetPhysicsScene()
        {
            return SceneManager.GetActiveScene().GetPhysicsScene();
        }
    }
}