using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SailingDemo
{
    /// <summary>Moves cannonballs along a parabola and destroys them when they reach the water level.</summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FireSystem))]
    public partial struct ProjectileSystem : ISystem
    {
        private const float Gravity = 9.81f;
        private const float WaterLevel = 0f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            float3 gravity = new float3(0f, -Gravity, 0f);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, projectile, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<Projectile>>().WithEntityAccess())
            {
                // Exact integration for constant acceleration: p += v*dt + 0.5*g*dt^2.
                transform.ValueRW.Position +=
                    projectile.ValueRO.Velocity * deltaTime + 0.5f * gravity * deltaTime * deltaTime;
                projectile.ValueRW.Velocity += gravity * deltaTime;

                if (transform.ValueRO.Position.y <= WaterLevel)
                {
                    ecb.DestroyEntity(entity);
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}