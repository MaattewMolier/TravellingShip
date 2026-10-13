using Unity.Entities;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>Lets the ECS drive the sail pivot rotation.</summary>
    public class SailPivotAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _ship;

        private class Baker : Baker<SailPivotAuthoring>
        {
            public override void Bake(SailPivotAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SailVisual
                {
                    Ship = GetEntity(authoring._ship, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}