using Unity.Entities;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>Bakes the data needed to furl the sail by scaling it.</summary>
    public class SailFurlAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _ship;

        private class Baker : Baker<SailFurlAuthoring>
        {
            public override void Bake(SailFurlAuthoring authoring)
            {
                Transform sailTransform = GetComponent<Transform>();
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SailFurl
                {
                    Ship = GetEntity(authoring._ship, TransformUsageFlags.Dynamic),
                    BaseScale = sailTransform.localScale,
                    BaseLocalY = sailTransform.localPosition.y,
                });
            }
        }
    }
}