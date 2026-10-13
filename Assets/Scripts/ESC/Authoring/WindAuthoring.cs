using Unity.Entities;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>Bakes the global wind settings into a singleton.</summary>
    public class WindAuthoring : MonoBehaviour
    {
        [SerializeField, Range(0f, 360f)] private float _directionDegrees = 45f;
        [SerializeField, Min(0f)] private float _speed = 10f;
        [SerializeField, Range(0f, 0.5f)] private float _gustStrength = 0.15f;
        [SerializeField, Min(0.01f)] private float _gustFrequency = 0.2f;

        private class Baker : Baker<WindAuthoring>
        {
            public override void Bake(WindAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new WindConfig
                {
                    DirectionDegrees = authoring._directionDegrees,
                    Speed = authoring._speed,
                    GustStrength = authoring._gustStrength,
                    GustFrequency = authoring._gustFrequency,
                });
            }
        }
    }
}