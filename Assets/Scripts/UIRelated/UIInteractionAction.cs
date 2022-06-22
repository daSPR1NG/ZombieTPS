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
            if (_hideContent) { Helper.HideGO(_content.gameObject); }
        }

        private void SetInteractionTextFeedback(Transform target, InteractionData interactionData)
        {
            WorldUI worldUI = _content.GetComponent<WorldUI>();
            worldUI.SetTarget(target);

            string interactionAction = interactionData.GetInteractionInputAction() + " to " + interactionData.GetInteractionActionType().ToString();
            _inputActionText.SetText(interactionAction);

            Sprite inputIcon = interactionData.GetInputIcon();
            _inputIcon.sprite = inputIcon;

            Helper.DisplayGO(_content.gameObject);
        }

        private void HideContent()
        {
            _inputActionText.SetText(string.Empty);

            _inputIcon.sprite = null;
            Helper.HideGO(_content.gameObject);
        }
    }
}