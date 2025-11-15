using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct SpawnerSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        /*
        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();

        SpawnerJob job = new SpawnerJob
        {
            ecb = ecb,
            personPrefab = entitiesReferences.personPrefabEntity,
        };

        job.ScheduleParallel();
        */

        //*
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<Spawner> spawner)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<Spawner>>())
        {
        // If already spawned, skip
            if (spawner.ValueRO.hasSpawned)
            {
                return;
            }

            int count = math.max(0, spawner.ValueRO.spawnCount);

            for (int i = 0; i < count; i++)
            {
                // Instantiate prefab using the parallel ECB
                Entity personEntity = state.EntityManager.Instantiate(entitiesReferences.personPrefabEntity);

                Random rnd = new Random((uint)(personEntity.Index + 1) * 747796405u);

                // Sample a random point inside the user-defined rectangular area (centered on the spawner)
                // areaSize.x -> width (X), areaSize.y -> depth (Z)
                float rx = rnd.NextFloat(-0.5f, 0.5f);
                float rz = rnd.NextFloat(-0.5f, 0.5f);
                float3 offset = new float3(rx * spawner.ValueRO.areaSize.x, spawner.ValueRO.areaOffsetY, rz * spawner.ValueRO.areaSize.y);

                // Set/override the LocalTransform on the new entity
                LocalTransform spawnTransform = LocalTransform.FromPosition(localTransform.ValueRO.Position + offset);
                entityCommandBuffer.SetComponent(personEntity, spawnTransform);

                // Add RandomMovement component to the spawned entity
                entityCommandBuffer.AddComponent(personEntity, new RandomMovement
                {
                    originPosition = localTransform.ValueRO.Position + offset,
                    targetPosition = localTransform.ValueRO.Position + offset,
                    distanceMin = spawner.ValueRO.randomMovementDistanceMin,
                    distanceMax = spawner.ValueRO.randomMovementDistanceMax,
                    random = new Random((uint)personEntity.Index),
                    areaSize = spawner.ValueRO.areaSize,
                });
            }

            // Mark as spawned to avoid spawning again
            spawner.ValueRW.hasSpawned = true;
        }
    }
    //*/
}


// Questo job non serve
[BurstCompile]
public partial struct SpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public Entity personPrefab;

    public void Execute(RefRO<LocalTransform> localTransform, RefRW<Spawner> spawner, [EntityIndexInQuery] int entityInQueryIndex)
    {
        // If already spawned, skip
        if (spawner.ValueRO.hasSpawned)
        {
            return;
        }

        int count = math.max(0, spawner.ValueRO.spawnCount);

        // Create a local random seeded per-spawner (deterministic for this job frame).
        // Seed uses the query index so different spawners get different sequences.
        Unity.Mathematics.Random rnd = new Unity.Mathematics.Random((uint)(entityInQueryIndex + 1) * 747796405u);

        for (int i = 0; i < count; i++)
        {
            // Instantiate prefab using the parallel ECB
            Entity personEntity = ecb.Instantiate(entityInQueryIndex, personPrefab);

            // Sample a random point inside the user-defined rectangular area (centered on the spawner)
            // areaSize.x -> width (X), areaSize.y -> depth (Z)
            float rx = rnd.NextFloat(-0.5f, 0.5f);
            float rz = rnd.NextFloat(-0.5f, 0.5f);
            float3 offset = new float3(rx * spawner.ValueRO.areaSize.x, spawner.ValueRO.areaOffsetY, rz * spawner.ValueRO.areaSize.y);

            // Set/override the LocalTransform on the new entity
            LocalTransform spawnTransform = LocalTransform.FromPosition(localTransform.ValueRO.Position + offset);
            ecb.SetComponent(entityInQueryIndex, personEntity, spawnTransform);

            // Add RandomMovement component to the spawned entity
            ecb.AddComponent(entityInQueryIndex, personEntity, new RandomMovement
            {
                originPosition = localTransform.ValueRO.Position + offset,
                targetPosition = localTransform.ValueRO.Position + offset,
                distanceMin = spawner.ValueRO.randomMovementDistanceMin,
                distanceMax = spawner.ValueRO.randomMovementDistanceMax,
                random = new Unity.Mathematics.Random((uint)personEntity.Index),
            });
        }

        // Mark as spawned to avoid spawning again
        spawner.ValueRW.hasSpawned = true;
    }
}