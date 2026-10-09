using UnityEngine;

namespace SailingDemo
{
    /// <summary>Provides a global wind vector with optional gusts.</summary>
    public class WindSystem : MonoBehaviour
    {
        [Tooltip("Direction the wind blows TOWARD, in degrees (0 = world +Z, 90 = world +X).")]
        [SerializeField, Range(0f, 360f)] private float _directionDegrees = 90f;

        [Tooltip("Base wind speed in meters per second.")]
        [SerializeField, Min(0f)] private float _speed = 10f;

        [Tooltip("Relative gust strength (0 = constant wind).")]
        [SerializeField, Range(0f, 0.5f)] private float _gustStrength = 0.15f;

        [SerializeField, Min(0.01f)] private float _gustFrequency = 0.2f;

        /// <summary>Gets the current wind velocity (direction the air moves).</summary>
        public Vector3 Velocity
        {
            get
            {
                float gust = (Mathf.PerlinNoise(Time.time * _gustFrequency, 0.5f) - 0.5f) * 2f * _gustStrength;
                Vector3 direction = Quaternion.AngleAxis(_directionDegrees, Vector3.up) * Vector3.forward;
                return direction * (_speed * (1f + gust));
            }
        }
    }
}