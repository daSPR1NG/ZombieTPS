using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public enum InteractionActionType
    {
        Unassigned, Talk, Look, Interact
    }

    [System.Serializable]
    public class InteractionData
    {
        [SerializeField] private string _name = "New interaction type";
        [SerializeField] private InteractionActionType interactionType = InteractionActionType.Unassigned;
        [SerializeField] private string _interactionInputAction;
        [SerializeField] private Sprite _interactionInputIcon;

        public InteractionActionType GetInteractionActionType()
        {
            return interactionType;
        }

        public string GetInteractionInputAction()
        {
            return _interactionInputAction;
        }

        public Sprite GetInputIcon()
        {
            return _interactionInputIcon;
        }
    }

    public class PlayerInteractionHandler : MonoBehaviour
    {
        public bool IsInteracting = false;
        [SerializeField] private bool _uiFeedbackIsFromPlayer = true;
        [SerializeField] private float _interactionRange = 1.25f;
        [SerializeField] private List<InteractionData> interactionDatas = new();

        private InteractiveElement _interactiveElement;
        private bool _canInteract = false;

        private PlayerInput _playerInput;
        private InputAction _interactionInput;

        private ThirdPersonController _thirdPersonController;
        private PlayerInteractionZone _playerInteractionZone;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _interactionInput = _playerInput.actions["Interaction"];
        }

        private void OnEnable() => _interactionInput.performed += context => Interact();

        private void OnDisable() => _interactionInput.performed -= context => Interact();

        private void Start() => Initialize();

        private void Initialize()
        {
            _thirdPersonController = GetComponentInChildren<ThirdPersonController>();

            _playerInteractionZone = GetComponentInChildren<PlayerInteractionZone>();
            _playerInteractionZone.SetRadius(_interactionRange);
        }

        private void Interact()
        {
            if (!_canInteract) {
                Debug.Log("Can't interact");
                return; 
            }

            _thirdPersonController.LookAtSomething(_interactiveElement.transform);

            _interactiveElement.StartInteraction(transform);
            _thirdPersonController.SwitchState(_thirdPersonController.Interaction());
            Debug.Log("Interact");
        }

        public void CancelInteraction()
        {
            _interactiveElement.GetComponent<InteractiveElement>();
            _interactiveElement.ExitInteraction();

            SetCanInteract(null, InteractionActionType.Unassigned, false);
        }

        public void SetCanInteract(InteractiveElement interactiveElement, InteractionActionType interactionActionType, bool value)
        {
            if (interactiveElement) { _interactiveElement = interactiveElement; }
            
            _canInteract = value;

            switch (_uiFeedbackIsFromPlayer)
            {
                case true:
                    // This block below is used whenever we display the ui interaction from the player
                    if (_canInteract)
                    {
                        Actions.OnPlayerInteractionPossible?.Invoke(_interactiveElement.transform, GetInteractionData(interactionActionType));
                        return;
                    }

                    Actions.OnPlayerInteractionImpossible?.Invoke();
                    break;
                case false:
                    // This block below is used whenever we display the ui interaction from the interactive element
                    if (_uiFeedbackIsFromPlayer) { return; }

                    if (_canInteract)
                    {
                        _interactiveElement.DisplayInteractionUI();
                        return;
                    }

                    _interactiveElement.HideInteractionUI();
                    break;
            }

            if (!interactiveElement) { _interactiveElement = interactiveElement; }
        }

        private InteractionData GetInteractionData(InteractionActionType interactionActionType)
        {
            if(interactionDatas.Count == 0) { return null; }

            for (int i = 0; i < interactionDatas.Count; i++)
            {
                if (interactionDatas[i].GetInteractionActionType() == interactionActionType)
                {
                    return interactionDatas[i];
                }
            }

            return null;
        }
    }
}