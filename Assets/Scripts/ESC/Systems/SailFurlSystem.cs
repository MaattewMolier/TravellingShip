using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SailingDemo
{
    /// <summary>Scales the sail vertically by its opening, keeping the top edge fixed at the pivot.</summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ShipSimulationSystem))]
    public partial struct SailFurlSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, postTransform, furl) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<PostTransformMatrix>, RefRO<SailFurl>>())
            {
                if (furl.ValueRO.Ship == Entity.Null)
                {
                    continue;
                }

                ShipState ship = SystemAPI.GetComponent<ShipState>(furl.ValueRO.Ship);
                float openFactor = math.max(0.05f, ship.SailOpening / 100f);
                float3 baseScale = furl.ValueRO.BaseScale;

                postTransform.ValueRW.Value =
                    float4x4.Scale(baseScale.x, baseScale.y * openFactor, baseScale.z);
                transform.ValueRW.Position.y = furl.ValueRO.BaseLocalY * openFactor;
            }
        }
    }
}