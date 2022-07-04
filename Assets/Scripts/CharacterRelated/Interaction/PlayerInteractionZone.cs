using UnityEngine;

namespace Khynan_Coding
{
    public class PlayerInteractionZone : MonoBehaviour
    {
        private Transform _interactiveElementTransform;

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
            Debug.Log( "PlayerInteractionZone > On trigger enter. " + other.name, transform );

            InteractiveElement interactiveElement = other.GetComponent<InteractiveElement>();

            if (interactiveElement)
            {
                _interactiveElementTransform = other.transform;
                _playerInteractionHandler.SetCanInteract(interactiveElement, interactiveElement.InteractionActionType, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log( "PlayerInteractionZone > On trigger exit. " + other.name, transform );

            if (!_interactiveElementTransform) { return; }

            if (_interactiveElementTransform == other.transform || _interactiveElementTransform.Equals(null))
            {
                _playerInteractionHandler.CancelInteraction();
                _interactiveElementTransform = null;
            }
        }

        public void SetRadius(float delta)
        {
            _sphereCollider.radius = delta;
        }
    }
}