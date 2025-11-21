using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[ExecuteAlways]
public class FloorScaler : MonoBehaviour
{
    private float scaleFactor = 0.1f;
    private EntityManager entityManager;
    private Entity spawnerEntity;
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = entityManager.CreateEntityQuery(typeof(Spawner));
        spawnerEntity = query.GetSingletonEntity();
    }
    void Update()
    {
        // If still not initialized, skip this frame
        if (entityManager == null || spawnerEntity == Entity.Null)
            return;

        // Safe check that entity still exists
        if (!entityManager.Exists(spawnerEntity))
            return;

        int count = entityManager.GetComponentData<Spawner>(spawnerEntity).spawnCount;
        float2 size = entityManager.GetComponentData<Spawner>(spawnerEntity).areaSize;

        transform.localScale = new Vector3(size.x * scaleFactor, transform.localScale.y, size.y * scaleFactor);

        var collider = GetComponent<Collider>();
        collider.enabled = false;
        collider.enabled = true;
    }
}