using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class SpawnerAuthoring : MonoBehaviour
{

    public Vector2 areaSize = new Vector2(5f, 5f);
    public float areaOffsetY = 0f;

    [Header("Random movement")]
    public float randomMovementDistanceMin = 1f;
    public float randomMovementDistanceMax = 3f;

    public class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Spawner
            {
                randomMovementDistanceMin = authoring.randomMovementDistanceMin,
                randomMovementDistanceMax = authoring.randomMovementDistanceMax,
                hasSpawned = false,
                areaSize = new float2(authoring.areaSize.x, authoring.areaSize.y),
                areaOffsetY = authoring.areaOffsetY,
            });
        }
    }
}

public struct Spawner : IComponentData
{
    public int spawnCount;
    public float randomMovementDistanceMin;
    public float randomMovementDistanceMax;
    public bool hasSpawned;
    public float2 areaSize;
    public float areaOffsetY;
}