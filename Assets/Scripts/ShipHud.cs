using UnityEngine;
using UnityEngine.UIElements;

namespace SailingDemo
{
    /// <summary>Updates the two HUD labels (sail opening and rotation) using UI Toolkit.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class ShipHud : MonoBehaviour
    {
        [SerializeField] private ShipBridge _ship;

        private Label _openingLabel;
        private Label _rotationLabel;

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _openingLabel = root.Q<Label>("sail-opening-label");
            _rotationLabel = root.Q<Label>("sail-rotation-label");

            _ship.SailChanged += OnSailChanged;
            OnSailChanged(_ship.SailOpening, _ship.SailAngle);
        }

        private void OnDisable()
        {
            _ship.SailChanged -= OnSailChanged;
        }

        private void OnSailChanged(float opening, float angle)
        {
            _openingLabel.text = $"Sail opening: {opening:0}%";
            _rotationLabel.text = $"Sail rotation: {angle:+0;-0;0}°";
        }
    }
}