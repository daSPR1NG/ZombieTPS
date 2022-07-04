using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class UIInteractionAction : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private bool _hideContent = true;

        [Header("DEPENDENCIES")]
        [SerializeField] private TMP_Text _inputActionText;
        [SerializeField] private Image _inputIcon;

        Transform _content;
        Animator _animator;

        #region Public References

        #endregion

        private void OnEnable()
        {
            Actions.OnPlayerInteractionPossible += SetInteractionTextFeedback;
            Actions.OnPlayerInteractionImpossible += HideContent;
        }

        private void OnDisable()
        {
            Actions.OnPlayerInteractionPossible -= SetInteractionTextFeedback;
            Actions.OnPlayerInteractionImpossible -= HideContent;
        }

        void Start() => Init();

        void Init()
        {
            _content = transform.GetChild(0);
            _animator = GetComponent<Animator>();
            if (_hideContent) { Helper.HideGO(_content.gameObject); }
        }

        private void SetInteractionTextFeedback(Transform target, InteractionData interactionData)
        {
            WorldUI worldUI = _content.GetComponent<WorldUI>();
            worldUI.SetTarget(target);

            string interactionAction = interactionData.GetInteractionInputAction() + " to " + interactionData.GetInteractionActionType().ToString();
            _inputActionText.SetText(interactionAction);
            _inputActionText.color = Color.white;

            Sprite inputIcon = interactionData.GetInputIcon();
            _inputIcon.sprite = inputIcon;

            Helper.DisplayGO(_content.gameObject);
            _animator.Play( "PlayerActionFeedback" );

            Debug.Log( "SetInteractionTextFeedback in UIINteractionAction" );
        }

        private void HideContent()
        {
            _inputActionText.SetText(string.Empty);

            _inputIcon.sprite = null;
            Helper.HideGO(_content.gameObject);
        }
    }
}