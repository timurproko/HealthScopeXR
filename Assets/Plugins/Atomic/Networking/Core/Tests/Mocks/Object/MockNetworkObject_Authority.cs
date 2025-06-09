using System;

namespace Atomic.Networking
{
    public partial class MockNetworkObject
    {
        public event Action OnInputAuthorityGained;
        public event Action OnInputAuthorityLost;

        public bool HasInputAuthority { get; set; }
        public bool HasStateAuthority { get; set; }
        public bool IsProxy { get; set; }

        public int InputAuthority { get; set; }
        public int StateAuthority { get; set; }

        public void AssignInputAuthority(int playerId)
        {
            throw new System.NotImplementedException();
        }
    }
}