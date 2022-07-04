using UnityEngine;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public abstract class InteractiveElement : MonoBehaviour, IInteractive
    {
        [Header("INTERACTION SETUP")]
        [SerializeField] protected GameObject _interactionCanvas;
        [SerializeField] private InteractionActionType _interactionActionType = InteractionActionType.Unassigned;

        #region Public References
        public InteractionActionType InteractionActionType { get => _interactionActionType; }
        #endregion

        private void Start() => Init();

        private void Init()
        {
            HideInteractionUI();
        }

        public virtual void StartInteraction(Transform interactionActor, float interactionSpeedMultiplier = 1)
        {
            Debug.Log("Start interaction");
        }

        public virtual void ExitInteraction( Transform interactionActor )
        {
            Debug.Log("Exit interaction");
        }

        public virtual void DisplayInteractionUI()
        {
            Debug.Log("Display Interaction UI");

            Helper.DisplayGO(_interactionCanvas);
        }

        public void HideInteractionUI()
        {
            Debug.Log("Hide Interaction UI");

            Helper.HideGO(_interactionCanvas);
        }
    }
}