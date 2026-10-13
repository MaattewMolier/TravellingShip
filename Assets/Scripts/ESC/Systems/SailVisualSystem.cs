using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SailingDemo
{
    /// <summary>Applies the sail angle to the sail pivot rotation.</summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ShipSimulationSystem))]
    public partial struct SailVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, visual) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<SailVisual>>())
            {
                if (visual.ValueRO.Ship == Entity.Null)
                {
                    continue;
                }

                ShipState ship = SystemAPI.GetComponent<ShipState>(visual.ValueRO.Ship);
                transform.ValueRW.Rotation = quaternion.RotateY(math.radians(ship.SailAngle));
            }
        }
    }
}