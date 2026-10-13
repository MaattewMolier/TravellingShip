using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>
    /// Copies the ship entity pose to this GameObject (for camera and water) and exposes sail state to the HUD.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class ShipBridge : MonoBehaviour
    {
        private EntityManager _entityManager;
        private EntityQuery _shipQuery;
        private bool _hasState;

        /// <summary>Raised when sail opening (0-100) or angle (-45..45) changes.</summary>
        public event Action<float, float> SailChanged;

        public float SailOpening { get; private set; }

        public float SailAngle { get; private set; }

        private void Start()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                Debug.LogError("Default ECS world not found.", this);
                enabled = false;
                return;
            }

            _entityManager = world.EntityManager;
            _shipQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ShipState>(),
                ComponentType.ReadOnly<LocalTransform>());
        }

        private void LateUpdate()
        {
            // The sub scene loads asynchronously, so the ship may not exist yet.
            if (!_shipQuery.TryGetSingletonEntity<ShipState>(out Entity ship))
            {
                return;
            }

            LocalTransform pose = _entityManager.GetComponentData<LocalTransform>(ship);
            transform.SetPositionAndRotation(pose.Position, pose.Rotation);

            ShipState state = _entityManager.GetComponentData<ShipState>(ship);
            bool changed = !_hasState
                || !Mathf.Approximately(state.SailOpening, SailOpening)
                || !Mathf.Approximately(state.SailAngle, SailAngle);

            if (!changed)
            {
                return;
            }

            _hasState = true;
            SailOpening = state.SailOpening;
            SailAngle = state.SailAngle;
            SailChanged?.Invoke(SailOpening, SailAngle);
        }
    }
}