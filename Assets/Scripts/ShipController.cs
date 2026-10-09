using System;
using UnityEngine;

namespace SailingDemo
{
    /// <summary>
    /// Simplified sailing simulation: sail force from apparent wind plus anisotropic hull drag.
    /// Does not use Unity physics.
    /// </summary>
    [RequireComponent(typeof(ShipInput))]
    public class ShipController : MonoBehaviour
    {
        private const float AirDensity = 1.225f;
        private const float MaxDeltaTime = 0.05f;

        [Header("References")]
        [SerializeField] private WindSystem _wind;
        [SerializeField] private Transform _sailPivot;

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

        private ShipInput _input;
        private Vector3 _velocity;
        private float _sailOpening;
        private float _sailAngle;

        /// <summary>Raised when sail opening (0-100) or angle (-45..45) changes.</summary>
        public event Action<float, float> SailChanged;

        public float SailOpening => _sailOpening;

        public float SailAngle => _sailAngle;

        private void Awake()
        {
            _input = GetComponent<ShipInput>();
        }

        private void Start()
        {
            ApplySailVisuals();
            SailChanged?.Invoke(_sailOpening, _sailAngle);
        }

        private void Update()
        {
            float deltaTime = Mathf.Min(Time.deltaTime, MaxDeltaTime);

            UpdateSail(deltaTime);
            UpdateHeading(deltaTime);

            Vector3 force = CalculateSailForce() + CalculateHullDrag();
            _velocity += force / _mass * deltaTime;
            _velocity.y = 0f;

            transform.position += _velocity * deltaTime;
        }

        private void UpdateSail(float deltaTime)
        {
            float newOpening = Mathf.Clamp(
                _sailOpening + _input.SailOpenAxis * _sailOpenSpeed * deltaTime, 0f, 100f);
            float newAngle = Mathf.Clamp(
                _sailAngle + _input.SailRotateAxis * _sailRotateSpeed * deltaTime,
                -_maxSailAngle,
                _maxSailAngle);

            if (Mathf.Approximately(newOpening, _sailOpening) && Mathf.Approximately(newAngle, _sailAngle))
            {
                return;
            }

            _sailOpening = newOpening;
            _sailAngle = newAngle;
            ApplySailVisuals();
            SailChanged?.Invoke(_sailOpening, _sailAngle);
        }

        private void UpdateHeading(float deltaTime)
        {
            // The rudder works only when the ship is moving.
            float steerEffect = Mathf.Clamp01(_velocity.magnitude / 2f);
            transform.Rotate(0f, _input.SteerAxis * _turnRate * steerEffect * deltaTime, 0f);
        }

        private Vector3 CalculateSailForce()
        {
            Vector3 apparentWind = _wind.Velocity - _velocity;
            Vector3 sailDirection = Quaternion.AngleAxis(_sailAngle, Vector3.up) * transform.forward;
            Vector3 sailNormal = Vector3.Cross(Vector3.up, sailDirection);

            float normalSpeed = Vector3.Dot(apparentWind, sailNormal);
            float effectiveArea = _sailArea * _sailOpening / 100f;
            float magnitude = 0.5f * AirDensity * effectiveArea * _sailForceCoefficient
                * normalSpeed * normalSpeed;

            return sailNormal * (Mathf.Sign(normalSpeed) * magnitude);
        }

        private Vector3 CalculateHullDrag()
        {
            float forwardSpeed = Vector3.Dot(_velocity, transform.forward);
            float lateralSpeed = Vector3.Dot(_velocity, transform.right);

            Vector3 forwardDrag = -transform.forward
                * (_forwardDragCoefficient * forwardSpeed * Mathf.Abs(forwardSpeed));
            Vector3 lateralDrag = -transform.right
                * (_lateralDragCoefficient * lateralSpeed * Mathf.Abs(lateralSpeed));

            return forwardDrag + lateralDrag;
        }

        private void ApplySailVisuals()
        {
            if (_sailPivot == null)
            {
                return;
            }

            _sailPivot.localRotation = Quaternion.Euler(0f, _sailAngle, 0f);

            // Furling effect: the sail shrinks vertically toward the yard (the pivot).
            float scaleY = Mathf.Max(0.05f, _sailOpening / 100f);
            Vector3 scale = _sailPivot.localScale;
            scale.y = scaleY;
            _sailPivot.localScale = scale;
        }
    }
}