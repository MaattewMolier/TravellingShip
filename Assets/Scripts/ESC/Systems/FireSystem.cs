using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SailingDemo
{
    /// <summary>Spawns one cannonball per fire request.</summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SailVisualSystem))]
    public partial struct FireSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, shipState, config, input) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<ShipState>, RefRO<ShipConfig>, RefRW<ShipInputData>>())
            {
                if (!input.ValueRO.Fire)
                {
                    continue;
                }

                input.ValueRW.Fire = false;

                ShipConfig cfg = config.ValueRO;
                if (cfg.ProjectilePrefab == Entity.Null)
                {
                    continue; // The prefab was not baked, nothing to fire.
                }
                quaternion rotation = transform.ValueRO.Rotation;

                float launchAngle = math.radians(cfg.LaunchAngleDegrees);
                float3 localDirection = new float3(0f, math.sin(launchAngle), math.cos(launchAngle));
                float3 direction = math.mul(rotation, localDirection);

                float3 muzzlePosition = transform.ValueRO.Position + math.mul(rotation, cfg.MuzzleOffset);
                float prefabScale = SystemAPI.GetComponent<LocalTransform>(cfg.ProjectilePrefab).Scale;

                Entity projectile = ecb.Instantiate(cfg.ProjectilePrefab);
                ecb.SetComponent(
                    projectile,
                    LocalTransform.FromPositionRotationScale(muzzlePosition, quaternion.identity, prefabScale));

                // The ball inherits the ship velocity, so it does not "fall behind" a moving ship.
                ecb.SetComponent(projectile, new Projectile
                {
                    Velocity = direction * cfg.MuzzleSpeed + shipState.ValueRO.Velocity,
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}