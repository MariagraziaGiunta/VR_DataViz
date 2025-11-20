using System;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct DataManagerSystem : ISystem
{
    // Removed [BurstCompile] because this system calls managed MonoBehaviour code.
    public void OnUpdate(ref SystemState state)
    {
        Spawner spawner = SystemAPI.GetSingleton<Spawner>();

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<DataManager> dataManager)
                in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<DataManager>>())
        {
            if (dataManager.ValueRO.hasLoaded)
                return;

            dataManager.ValueRW.fileName = FileSelector.Instance.GetFileName();

            spawner.spawnCount = FileSelector.Instance.GetNumberOfLinesInSelectedFile();

            // If Spawner is a singleton, write it back so other systems see the change
            SystemAPI.SetSingleton(spawner);

            dataManager.ValueRW.hasLoaded = true;
        }
    }
}
