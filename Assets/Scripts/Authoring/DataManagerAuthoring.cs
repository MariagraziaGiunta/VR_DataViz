using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DataManagerAuthoring : MonoBehaviour
{

    public class Baker : Baker<DataManagerAuthoring>
    {
        public override void Bake(DataManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DataManager{});
        }
    }
}

public struct DataManager : IComponentData
{
    public FixedString128Bytes fileName;
    public bool hasLoaded;
}
