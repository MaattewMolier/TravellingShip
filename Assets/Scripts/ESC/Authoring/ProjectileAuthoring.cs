using Unity.Entities;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>Marks a prefab as a cannonball.</summary>
    public class ProjectileAuthoring : MonoBehaviour
    {
        private class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Projectile>(entity);
            }
        }
    }
}