using System;

namespace Atomic.Networking
{
    public partial class MockNetworkObject
    {
        public event Action OnInterestEnter;
        
        public event Action OnInterestExit;
        
        public bool IsInterested()
        {
            throw new NotImplementedException();
        }

        public void SetInterested(in int player, in bool interested)
        {
            throw new System.NotImplementedException();
        }
    }
}