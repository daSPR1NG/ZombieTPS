using UnityEngine;

namespace Khynan_Coding
{
    public class PlayerInteractionZone : MonoBehaviour
    {
        private Transform _interactiveElement;

        private PlayerInteractionHandler _playerInteractionHandler;
        private SphereCollider _sphereCollider;

        private void Start() => Initialiaze();

        private void Initialiaze()
        {
            _playerInteractionHandler = GetComponentInParent<PlayerInteractionHandler>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("On trigger enter. " + other.name, transform);

            InteractiveElement interactiveElement = other.GetComponent<InteractiveElement>();

            if (interactiveElement is not null)
            {
                _interactiveElement = other.transform;
                _playerInteractionHandler.SetCanInteract(interactiveElement, interactiveElement.InteractionActionType, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("On trigger exit. " + other.name, transform);

            if (!_interactiveElement) { return; }

            if (_interactiveElement == other.transform || _interactiveElement.Equals(null))
            {
                _playerInteractionHandler.CancelInteraction();
                _interactiveElement = null;
            }
        }

        public void SetRadius(float delta)
        {
            _sphereCollider.radius = delta;
        }
    }
}