using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class SpawnerAuthoring : MonoBehaviour
{
    public float areaOffsetY = 0f;
    public float randomMovementDistanceMin = 1f;
    public float randomMovementDistanceMax = 3f;
    public float minArea = 7.5f;

    public class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Spawner
            {
                randomMovementDistanceMin = authoring.randomMovementDistanceMin,
                randomMovementDistanceMax = authoring.randomMovementDistanceMax,
                minArea = authoring.minArea,
                hasSpawned = false,
                areaOffsetY = authoring.areaOffsetY,
            });
        }
    }
}

public struct Spawner : IComponentData
{
    public float areaOffsetY;
    public float randomMovementDistanceMin;
    public float randomMovementDistanceMax;
    public float minArea;

    public int spawnCount;
    public bool hasSpawned;
    public float2 areaSize;
}