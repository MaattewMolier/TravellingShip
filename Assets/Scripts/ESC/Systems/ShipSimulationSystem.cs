using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SailingDemo
{
    /// <summary>Simplified sailing simulation: sail force from apparent wind plus anisotropic hull drag.</summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ShipSimulationSystem : ISystem
    {
        private const float AirDensity = 1.225f;
        private const float MaxDeltaTime = 0.05f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WindConfig>();
            state.RequireForUpdate<ShipConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = math.min(SystemAPI.Time.DeltaTime, MaxDeltaTime);
            float3 windVelocity = SystemAPI.GetSingleton<WindConfig>().GetVelocity((float)SystemAPI.Time.ElapsedTime);

            foreach (var (transform, shipState, config, input) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShipState>, RefRO<ShipConfig>, RefRO<ShipInputData>>())
            {
                ShipConfig cfg = config.ValueRO;
                ShipInputData inp = input.ValueRO;
                ref ShipState ship = ref shipState.ValueRW;
                ref LocalTransform local = ref transform.ValueRW;

                // Sail controls.
                ship.SailOpening = math.clamp(
                    ship.SailOpening + inp.SailOpenAxis * cfg.SailOpenSpeed * deltaTime, 0f, 100f);
                ship.SailAngle = math.clamp(
                    ship.SailAngle + inp.SailRotateAxis * cfg.SailRotateSpeed * deltaTime,
                    -cfg.MaxSailAngle,
                    cfg.MaxSailAngle);

                // The rudder works only when the ship is moving.
                float steerEffect = math.saturate(math.length(ship.Velocity) / 2f);
                float yawRadians = math.radians(inp.SteerAxis * cfg.TurnRate * steerEffect * deltaTime);
                local.Rotation = math.mul(local.Rotation, quaternion.RotateY(yawRadians));

                float3 forward = math.mul(local.Rotation, math.forward());
                float3 right = math.mul(local.Rotation, math.right());

                // Sail force from apparent wind (flat plate model).
                float3 apparentWind = windVelocity - ship.Velocity;
                float3 sailDirection = math.mul(quaternion.RotateY(math.radians(ship.SailAngle)), forward);
                float3 sailNormal = math.cross(math.up(), sailDirection);
                float normalSpeed = math.dot(apparentWind, sailNormal);
                float effectiveArea = cfg.SailArea * ship.SailOpening / 100f;
                float magnitude = 0.5f * AirDensity * effectiveArea * cfg.SailForceCoefficient
                    * normalSpeed * normalSpeed;
                float3 sailForce = sailNormal * (math.sign(normalSpeed) * magnitude);

                // Hull drag: small along the ship, large sideways (keel).
                float forwardSpeed = math.dot(ship.Velocity, forward);
                float lateralSpeed = math.dot(ship.Velocity, right);
                float3 drag = -forward * (cfg.ForwardDragCoefficient * forwardSpeed * math.abs(forwardSpeed))
                    - right * (cfg.LateralDragCoefficient * lateralSpeed * math.abs(lateralSpeed));

                ship.Velocity += (sailForce + drag) / cfg.Mass * deltaTime;
                ship.Velocity.y = 0f;
                local.Position += ship.Velocity * deltaTime;
            }
        }
    }
}