using Unity.Entities;
using Unity.Mathematics;

namespace SailingDemo
{
    /// <summary>Static ship tuning values, baked from <see cref="ShipAuthoring"/>.</summary>
    public struct ShipConfig : IComponentData
    {
        public float SailArea;
        public float SailForceCoefficient;
        public float SailOpenSpeed;
        public float SailRotateSpeed;
        public float MaxSailAngle;
        public float Mass;
        public float ForwardDragCoefficient;
        public float LateralDragCoefficient;
        public float TurnRate;

        public Entity ProjectilePrefab;
        public float3 MuzzleOffset;
        public float MuzzleSpeed;
        public float LaunchAngleDegrees;
    }

    /// <summary>Mutable ship simulation state.</summary>
    public struct ShipState : IComponentData
    {
        public float3 Velocity;
        public float SailOpening; // 0..100
        public float SailAngle;   // -45..45 degrees
    }

    /// <summary>Input snapshot written by <see cref="ShipInputSystem"/>.</summary>
    public struct ShipInputData : IComponentData
    {
        public float SailOpenAxis;
        public float SailRotateAxis;
        public float SteerAxis;
        public bool Fire;
    }

    /// <summary>Drives sail furling by scaling the sail and keeping its top edge at the pivot.</summary>
    public struct SailFurl : IComponentData
    {
        public Entity Ship;
        public float3 BaseScale;  // Local scale when fully open.
        public float BaseLocalY;  // Local Y position when fully open.
    }

    /// <summary>Marks the sail pivot entity and points at the ship it belongs to.</summary>
    public struct SailVisual : IComponentData
    {
        public Entity Ship;
    }
}