using Unity.Entities;
using Unity.Mathematics;

namespace SailingDemo
{
    /// <summary>Global wind singleton.</summary>
    public struct WindConfig : IComponentData
    {
        public float DirectionDegrees; // direction the wind blows TOWARD (0 = +Z, 90 = +X)
        public float Speed;
        public float GustStrength;
        public float GustFrequency;

        /// <summary>Returns the wind velocity (direction the air moves) at the given time.</summary>
        public float3 GetVelocity(float time)
        {
            float gust = noise.cnoise(new float2(time * GustFrequency, 0.5f)) * GustStrength;
            float3 direction = math.mul(quaternion.RotateY(math.radians(DirectionDegrees)), math.forward());
            return direction * (Speed * (1f + gust));
        }
    }
}