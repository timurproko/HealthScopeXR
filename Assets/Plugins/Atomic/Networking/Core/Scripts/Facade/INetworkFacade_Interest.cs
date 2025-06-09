using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atomic.Networking
{
    public partial interface INetworkFacade
    {
        event Action<INetworkObject> OnInterestEnter;
       
        event Action<INetworkObject> OnInterestExit;

        void SetupInterestArea(int gridX, int gridY, int gridZ, int cellSize);

        void ChangePlayerInterestArea(int playerId, Vector3 center, float radius);

        void ClearPlayerInterestArea(int playerId);

        void AddPlayerInterestArea(int playerId, Vector3 center, float radius);
        
        List<INetworkObject> GetInterestedObjects(int playerId);
    }
}