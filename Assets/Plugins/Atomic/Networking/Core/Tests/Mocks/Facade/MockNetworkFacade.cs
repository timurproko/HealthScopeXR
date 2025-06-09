using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atomic.Networking
{
    public sealed unsafe partial class MockNetworkFacade : INetworkFacade
    {
        public void GetAllObjects(List<INetworkObject> results)
        {
            throw new NotImplementedException();
        }

        public INetworkObject SpawnObject(INetworkObject prefab, int playerId)
        {
            throw new System.NotImplementedException();
        }

        public event Action<INetworkObject> OnInterestEnter;
        public event Action<INetworkObject> OnInterestExit;

        public void SetupInterestArea(int gridX, int gridY, int gridZ, int cellSize)
        {
            throw new NotImplementedException();
        }

        public void ChangePlayerInterestArea(int playerId, Vector3 center, float radius)
        {
            throw new NotImplementedException();
        }

        public void ClearPlayerInterestArea(int playerId)
        {
            throw new NotImplementedException();
        }

        public void AddPlayerInterestArea(int playerId, Vector3 center, float radius)
        {
            throw new NotImplementedException();
        }

        public List<INetworkObject> GetInterestedObjects(int playerId)
        {
            throw new NotImplementedException();
        }

        public bool IsActive { get; set; }

        public int Tick { get; set; }
        public bool IsForwardTick { get; }
        public bool IsResumulationTick { get; }

        public float LocalAlpha { get; }
        public float LocalRenderTime { get; }
        public float RemoteRenderTime { get; }

        public float DeltaTime { get; set; }

        public int SizeOf<T>(int count = 1) where T : unmanaged => sizeof(T) * count;
        public T Provide<T>()
        {
            throw new NotImplementedException();
        }

        public bool TryProvide<T>(out T value)
        {
            throw new NotImplementedException();
        }
    }
}