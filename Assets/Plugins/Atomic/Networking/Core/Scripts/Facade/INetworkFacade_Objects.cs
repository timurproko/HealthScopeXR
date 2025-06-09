using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atomic.Networking
{
    public partial interface INetworkFacade
    {
        event Action<INetworkObject> OnObjectAcquired;

        T GetPlayerObject<T>(int playerId);
        
        INetworkObject GetPlayerObject(int playerId);
        
        bool TryGetPlayerObject(int playerId, out INetworkObject obj);

        bool TryGetPlayerObject<T>(int playerId, out T entity);

        void PutPlayerObject(int playerId, INetworkObject obj);

        void RemovePlayerObject(int playerId);

        bool RemovePlayerObject(int playerId, out INetworkObject obj);
        
        INetworkObject GetLocalPlayerObject();

        bool TryGetLocalPlayerObject(out INetworkObject obj);
        
        T GetLocalPlayerObject<T>();

        bool TryGetLocalPlayerObject<T>(out T entity);

        INetworkObject FindObject(int entityId);
       
        T FindObject<T>(int entityId);
        
        bool FindObject(int entityId, out INetworkObject obj);
       
        bool FindObject<T>(int entityId, out T entity);

        INetworkObject[] GetAllObjects();

        int GetAllObjects(INetworkObject[] results);

        void GetAllObjects(List<INetworkObject> results);

        INetworkObject SpawnObject(INetworkObject prefab, int playerId);

        INetworkObject SpawnObject(INetworkObject prefab, Vector3 position, Quaternion rotation, int playerId);

        T SpawnObject<T>(INetworkObject prefab, int playerId);

        T SpawnObject<T>(INetworkObject prefab, Vector3 position, Quaternion rotation, int playerId);

        void DespawnObject(INetworkObject obj);

        bool ContainsObject(INetworkObject obj);

        void AttachObject(INetworkObject obj, int player);
    }
}