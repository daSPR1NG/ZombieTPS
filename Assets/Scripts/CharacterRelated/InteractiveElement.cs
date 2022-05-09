using UnityEngine;

namespace Khynan_Coding
{
    public abstract class InteractiveElement : MonoBehaviour, IInteractive
    {
        [Header("INTERACTION SETUP")]
        [SerializeField] private GameObject _interactionCanvas;
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

        public virtual void ExitInteraction()
        {
            Debug.Log("Exit interaction");
        }

        public void DisplayInteractionUI()
        {
            Debug.Log("Display Interaction UI");

            Helper.DisplayUIWindow(_interactionCanvas);
        }

        public void HideInteractionUI()
        {
            Debug.Log("Hide Interaction UI");

            Helper.HideUIWindow(_interactionCanvas);
        }
    }
}