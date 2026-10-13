using Unity.Entities;
using Unity.Mathematics;

namespace SailingDemo
{
    /// <summary>A cannonball in flight.</summary>
    public struct Projectile : IComponentData
    {
        public float3 Velocity;
    }
}