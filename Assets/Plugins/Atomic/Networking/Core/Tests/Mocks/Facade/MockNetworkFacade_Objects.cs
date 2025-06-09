using System;
using UnityEngine;

namespace Atomic.Networking
{
    public sealed partial class MockNetworkFacade
    {
        public event Action OnConnectedToServer;
        public event Action OnDisconnectedFromServer;
        public event Action<string> OnConnectionFailed;

        public event Action<INetworkObject> OnObjectAcquired;

        public T GetPlayerObject<T>(int playerId)
        {
            throw new System.NotImplementedException();
        }

        public INetworkObject GetPlayerObject(int playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPlayerObject(int playerId, out INetworkObject agent)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetPlayerObject<T>(int playerId, out T agent)
        {
            throw new System.NotImplementedException();
        }

        public void PutPlayerObject(int playerId, INetworkObject agent)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePlayerObject(int playerId)
        {
            throw new System.NotImplementedException();
        }

        public bool RemovePlayerObject(int playerId, out INetworkObject agent)
        {
            throw new System.NotImplementedException();
        }

        public INetworkObject GetLocalPlayerObject()
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetLocalPlayerObject(out INetworkObject obj)
        {
            throw new System.NotImplementedException();
        }

        public T GetLocalPlayerObject<T>()
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetLocalPlayerObject<T>(out T entity)
        {
            throw new System.NotImplementedException();
        }

        public INetworkObject FindObject(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public T FindObject<T>(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public bool FindObject(int entityId, out INetworkObject agent)
        {
            throw new System.NotImplementedException();
        }

        public bool FindObject<T>(int entityId, out T entity)
        {
            throw new System.NotImplementedException();
        }

        public INetworkObject[] GetAllObjects()
        {
            throw new System.NotImplementedException();
        }

        public int GetAllObjects(INetworkObject[] results)
        {
            throw new System.NotImplementedException();
        }

        public INetworkObject SpawnObject(INetworkObject prefab, Vector3 position, Quaternion rotation, int playerId)
        {
            throw new System.NotImplementedException();
        }

        public T SpawnObject<T>(INetworkObject prefab, int playerId)
        {
            throw new System.NotImplementedException();
        }

        public T SpawnObject<T>(INetworkObject prefab, Vector3 position, Quaternion rotation, int playerId)
        {
            throw new System.NotImplementedException();
        }

        public void DespawnObject(INetworkObject obj)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsObject(INetworkObject obj)
        {
            throw new NotImplementedException();
        }

        public void AttachObject(INetworkObject obj, int player)
        {
            throw new NotImplementedException();
        }
    }
}