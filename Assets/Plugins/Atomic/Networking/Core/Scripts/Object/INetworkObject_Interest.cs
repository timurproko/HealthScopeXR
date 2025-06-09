using System;

namespace Atomic.Networking
{
    public partial interface INetworkObject
    {
        event Action OnInterestEnter;
     
        event Action OnInterestExit;

        bool IsInterested();
        
        void SetInterested(in int player, in bool interested);
    }
}