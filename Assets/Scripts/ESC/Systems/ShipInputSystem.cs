using Unity.Entities;
using UnityEngine.InputSystem;

namespace SailingDemo
{
    /// <summary>Reads the new Input System and writes the result into <see cref="ShipInputData"/>.</summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ShipSimulationSystem))]
    public partial class ShipInputSystem : SystemBase
    {
        private InputAction _sailOpenAction;
        private InputAction _sailRotateAction;
        private InputAction _steerAction;
        private InputAction _fireAction;
        private float _nextLogTime;

        protected override void OnCreate()
        {
            _sailOpenAction = CreateAxisAction("SailOpen", "<Keyboard>/s", "<Keyboard>/w");
            _sailRotateAction = CreateAxisAction("SailRotate", "<Keyboard>/q", "<Keyboard>/e");
            _steerAction = CreateAxisAction("Steer", "<Keyboard>/a", "<Keyboard>/d");
            _fireAction = new InputAction("Fire", InputActionType.Button, "<Keyboard>/space");

            _sailOpenAction.Enable();
            _sailRotateAction.Enable();
            _steerAction.Enable();
            _fireAction.Enable();

            //RequireForUpdate<ShipInputData>();
            UnityEngine.Debug.Log("ShipInputSystem: created");
        }

        protected override void OnStartRunning()
        {
            UnityEngine.Debug.Log("ShipInputSystem: running (ShipInputData found)");
        }

        protected override void OnUpdate()
        {
            if (UnityEngine.Time.time >= _nextLogTime)
            {
                _nextLogTime = UnityEngine.Time.time + 1f;
                int count = SystemAPI.QueryBuilder().WithAll<ShipInputData>().Build().CalculateEntityCount();
                UnityEngine.Debug.Log($"ShipInputData entities: {count}");
            }

            float sailOpen = _sailOpenAction.ReadValue<float>();
            float sailRotate = _sailRotateAction.ReadValue<float>();
            float steer = _steerAction.ReadValue<float>();
            bool fire = _fireAction.WasPressedThisFrame();

            if (sailOpen != 0f || sailRotate != 0f || steer != 0f || fire)
            {
                UnityEngine.Debug.Log($"Input: open={sailOpen}, rotate={sailRotate}, steer={steer}, fire={fire}");
            }

            foreach (RefRW<ShipInputData> input in SystemAPI.Query<RefRW<ShipInputData>>())
            {
                input.ValueRW.SailOpenAxis = sailOpen;
                input.ValueRW.SailRotateAxis = sailRotate;
                input.ValueRW.SteerAxis = steer;

                // The flag is cleared by FireSystem once the shot is spawned.
                if (fire)
                {
                    input.ValueRW.Fire = true;
                }
                if (sailOpen != 0f)
                {
                    UnityEngine.Debug.Log($"Sail open axis: {sailOpen}");
                }
            }
        }

        protected override void OnDestroy()
        {
            _sailOpenAction.Dispose();
            _sailRotateAction.Dispose();
            _steerAction.Dispose();
            _fireAction.Dispose();
        }

        private static InputAction CreateAxisAction(string name, string negativeKey, string positiveKey)
        {
            var action = new InputAction(name, InputActionType.Value);
            action.AddCompositeBinding("1DAxis")
                .With("Negative", negativeKey)
                .With("Positive", positiveKey);
            return action;
        }
    }
}