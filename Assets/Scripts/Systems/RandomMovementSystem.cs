using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Linq;

partial struct RandomMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RandomMovementJob job = new RandomMovementJob { };
        job.ScheduleParallel();
        //*/

        /*
        foreach ((
            RefRW<RandomMovement> randomMovement,
            RefRW<UnitMover> unitMover,
            RefRO<LocalTransform> localTransform)
            in SystemAPI.Query<
                RefRW<RandomMovement>,
                RefRW<UnitMover>,
                RefRO<LocalTransform>>()){


            if (math.distancesq(localTransform.ValueRO.Position, randomMovement.ValueRO.targetPosition) < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ){
                //Target position raggiunta
                Random random = randomMovement.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f));
                randomDirection = math.normalize(randomDirection);

                randomMovement.ValueRW.targetPosition =
                    randomMovement.ValueRO.targetPosition +
                    randomDirection * random.NextFloat(randomMovement.ValueRO.distanceMin, randomMovement.ValueRO.distanceMax);

                randomMovement.ValueRW.random = random;
            } else{
                //Target position non ancora raggiunta
                unitMover.ValueRW.targetPosition = randomMovement.ValueRO.targetPosition;
            }
        }
        */
    }
}

[BurstCompile]
public partial struct RandomMovementJob : IJobEntity
{
    public void Execute(ref RandomMovement randomMovement, ref UnitMover unitMover, in LocalTransform localTransform)
    {
        // Previous behavior: if reached target, pick a new one; else push target to UnitMover
        if (math.distancesq(localTransform.Position, randomMovement.targetPosition) < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
        {
            Unity.Mathematics.Random rnd = randomMovement.randomSeed;

            float3 newTarget;
            float2 areaSize = randomMovement.areaSize;

            // keep generating until the new target is inside [-5, 5] on X and Z
            do
            {
                float3 randomDirection = new float3(rnd.NextFloat(-1f, +1f), 0f, rnd.NextFloat(-1f, +1f));
                randomDirection = math.normalize(randomDirection);

                float distance = rnd.NextFloat(randomMovement.distanceMin, randomMovement.distanceMax);
                newTarget = randomMovement.targetPosition + randomDirection * distance;

            } while ((newTarget.x < areaSize.x * -0.5f || newTarget.x > areaSize.x * 0.5f || newTarget.z < areaSize.y * -0.5f || newTarget.z > areaSize.y * 0.5f));

            randomMovement.targetPosition = newTarget;
            randomMovement.randomSeed = rnd;
        }
        else
        {
            unitMover.targetPosition = randomMovement.targetPosition;
        }
    }
}