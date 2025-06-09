using System;

namespace Atomic.Networking
{
    public partial interface INetworkFacade
    {
        event Action OnConnectedToServer;
        event Action OnDisconnectedFromServer;
        event Action<string> OnConnectionFailed;
    }
}