using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SailingDemo
{
    /// <summary>
    /// Exposes ship control axes using the new Input System.
    /// Actions are created in code, so no Input Actions asset is required.
    /// </summary>
    public class ShipInput : MonoBehaviour
    {
        private InputAction _sailOpenAction;
        private InputAction _sailRotateAction;
        private InputAction _steerAction;

        /// <summary>Gets the sail opening axis in range [-1, 1] (W/S).</summary>
        public float SailOpenAxis => _sailOpenAction.ReadValue<float>();

        /// <summary>Gets the sail rotation axis in range [-1, 1] (Q/E).</summary>
        public float SailRotateAxis => _sailRotateAction.ReadValue<float>();

        /// <summary>Gets the optional steering axis in range [-1, 1] (A/D).</summary>
        public float SteerAxis => _steerAction.ReadValue<float>();

        private static InputAction CreateAxisAction(string name, string negativeKey, string positiveKey)
        {
            var action = new InputAction(name, InputActionType.Value);
            action.AddCompositeBinding("1DAxis")
                .With("Negative", negativeKey)
                .With("Positive", positiveKey);
            return action;
        }

        private void Awake()
        {
            _sailOpenAction = CreateAxisAction("SailOpen", "<Keyboard>/s", "<Keyboard>/w");
            _sailRotateAction = CreateAxisAction("SailRotate", "<Keyboard>/q", "<Keyboard>/e");
            _steerAction = CreateAxisAction("Steer", "<Keyboard>/a", "<Keyboard>/d");
        }

        private void OnEnable()
        {
            _sailOpenAction.Enable();
            _sailRotateAction.Enable();
            _steerAction.Enable();
        }

        private void OnDisable()
        {
            _sailOpenAction.Disable();
            _sailRotateAction.Disable();
            _steerAction.Disable();
        }

        private void OnDestroy()
        {
            _sailOpenAction.Dispose();
            _sailRotateAction.Dispose();
            _steerAction.Dispose();
        }
    }
}