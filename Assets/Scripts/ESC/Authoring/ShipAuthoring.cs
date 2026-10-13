using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>Bakes ship tuning values and runtime state into ECS components.</summary>
    public class ShipAuthoring : MonoBehaviour
    {
        [Header("Sail")]
        [SerializeField, Min(0f)] private float _sailArea = 25f;
        [SerializeField, Min(0f)] private float _sailForceCoefficient = 1.2f;
        [SerializeField, Min(0f)] private float _sailOpenSpeed = 25f;
        [SerializeField, Min(0f)] private float _sailRotateSpeed = 40f;
        [SerializeField, Range(0f, 90f)] private float _maxSailAngle = 45f;

        [Header("Hull")]
        [SerializeField, Min(1f)] private float _mass = 1000f;
        [SerializeField, Min(0f)] private float _forwardDragCoefficient = 30f;
        [SerializeField, Min(0f)] private float _lateralDragCoefficient = 600f;
        [SerializeField, Min(0f)] private float _turnRate = 25f;

        [Header("Cannon")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Vector3 _muzzleOffset = new Vector3(0f, 2f, 4f);
        [SerializeField, Min(0f)] private float _muzzleSpeed = 35f;
        [SerializeField, Range(0f, 80f)] private float _launchAngleDegrees = 30f;

        private class Baker : Baker<ShipAuthoring>
        {
            public override void Bake(ShipAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ShipConfig
                {
                    SailArea = authoring._sailArea,
                    SailForceCoefficient = authoring._sailForceCoefficient,
                    SailOpenSpeed = authoring._sailOpenSpeed,
                    SailRotateSpeed = authoring._sailRotateSpeed,
                    MaxSailAngle = authoring._maxSailAngle,
                    Mass = authoring._mass,
                    ForwardDragCoefficient = authoring._forwardDragCoefficient,
                    LateralDragCoefficient = authoring._lateralDragCoefficient,
                    TurnRate = authoring._turnRate,
                    ProjectilePrefab = GetEntity(authoring._projectilePrefab, TransformUsageFlags.Dynamic),
                    MuzzleOffset = authoring._muzzleOffset,
                    MuzzleSpeed = authoring._muzzleSpeed,
                    LaunchAngleDegrees = authoring._launchAngleDegrees,
                });
                AddComponent<ShipState>(entity);
                AddComponent<ShipInputData>(entity);
            }
        }
    }
}