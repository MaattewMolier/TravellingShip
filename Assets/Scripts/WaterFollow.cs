using UnityEngine;

namespace SailingDemo
{
    /// <summary>
    /// Keeps the water plane under the ship, snapped to the texture tile size,
    /// so the texture appears to scroll while the plane never ends.
    /// </summary>
    public class WaterFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Tooltip("World-space size of one texture tile. Plane scale 100 = 1000 m, tiling 100 = 10 m per tile.")]
        [SerializeField, Min(0.1f)] private float _tileSize = 10f;

        private void LateUpdate()
        {
            Vector3 position = _target.position;
            position.x = Mathf.Round(position.x / _tileSize) * _tileSize;
            position.z = Mathf.Round(position.z / _tileSize) * _tileSize;
            position.y = transform.position.y;
            transform.position = position;
        }
    }
}