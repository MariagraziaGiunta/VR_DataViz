using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomMovementAuthoring : MonoBehaviour
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public uint randomSeed;

    public class Baker : Baker<RandomMovementAuthoring>
    {
        public override void Bake(RandomMovementAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new RandomMovement
            {
                targetPosition = authoring.targetPosition,
                originPosition = authoring.originPosition,
                distanceMin = authoring.distanceMin,
                distanceMax = authoring.distanceMax,
                random = new Unity.Mathematics.Random(authoring.randomSeed)
            });
        }
    }
}


public struct RandomMovement : IComponentData
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public Unity.Mathematics.Random random;

    public float2 areaSize;
}