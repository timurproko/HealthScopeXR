using System;

namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        event Action OnInputAuthorityGained;
        
        event Action OnInputAuthorityLost;
        
        bool HasInputAuthority { get; }
       
        bool HasStateAuthority { get; }
        
        bool IsProxy { get; }
        
        int InputAuthority { get; }
        
        int StateAuthority { get; }

        void AssignInputAuthority(int playerId);
    }
}