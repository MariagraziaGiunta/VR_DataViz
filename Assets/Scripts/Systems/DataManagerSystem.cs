using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct DataManagerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        Spawner spawner = SystemAPI.GetSingleton<Spawner>();

        foreach (
            RefRW<DataManager> dataManager
                in SystemAPI.Query<
                RefRW<DataManager>>())
        {
            if (dataManager.ValueRO.hasLoaded)
                return;

            //var entitiesToSpawn = DataReader.Instance.GetLinesNumber();

            //test only
            var entitiesToSpawn = 500;

            var scaleFactor = new float();
            
            scaleFactor = 2f;

            dataManager.ValueRW.fileName = DataReader.Instance.GetFileName();

            spawner.spawnCount = entitiesToSpawn;

            spawner.areaSize = new float2(math.sqrt(entitiesToSpawn * scaleFactor), math.sqrt(entitiesToSpawn * scaleFactor));

            UnityEngine.Debug.Log("DataManagerSystem: areaSize " + spawner.areaSize);

            // If Spawner is a singleton, write it back so other systems see the change
            SystemAPI.SetSingleton(spawner);

            dataManager.ValueRW.hasLoaded = true;
        }
    }
}
