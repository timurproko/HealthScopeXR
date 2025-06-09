using Meta.XR.MRUtilityKit;
using UnityEngine;
using System;

public class SpawnAtPoint : MonoBehaviour
{
    public event Action OnSpawned;

    [SerializeField] GameObject objectToSpawn;
    [SerializeField] bool isScenePrefab;
    [SerializeField] MRUKAnchor.SceneLabels label;
    [SerializeField] bool spawnOnBase;
    [SerializeField] bool matchSize;

    public void TriggerSpawn()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        bool spawned = false;

        foreach (var item in room.Anchors)
        {
            if (item.Label == label)
            {
                var obj = Instantiate(objectToSpawn, item.transform.position, Quaternion.identity);

                if (spawnOnBase && item.VolumeBounds.HasValue)
                {
                    var anchorSize = item.VolumeBounds.Value;
                    obj.transform.position = new Vector3(
                        obj.transform.position.x,
                        obj.transform.position.y - anchorSize.extents.z * 2,
                        obj.transform.position.z
                    );
                }

                if (matchSize && item.VolumeBounds.HasValue)
                {
                    var bounds = item.VolumeBounds.Value;
                    float originalY = objectToSpawn.transform.localScale.y;
                    obj.transform.localScale = new Vector3(bounds.size.x, originalY, bounds.size.z);
                }

                obj.transform.SetParent(transform);
                spawned = true;
                OnSpawned?.Invoke();
                break;
            }
        }

        if (isScenePrefab && spawned)
        {
            objectToSpawn.SetActive(false);
        }
    }
}