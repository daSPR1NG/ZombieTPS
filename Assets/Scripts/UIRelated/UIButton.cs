using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("SELECTION SETTINGS")]
        [SerializeField] private Image selectionImage;
        private bool _isSelected = false;

        #region Public References
        public bool IsSelected { get => _isSelected; private set => _isSelected = value; }
        #endregion

        private void OnDisable()
        {
            DeselectButton();
            CancelHoverButton();
        }

        void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            DeselectButton();
            CancelHoverButton();
        }

        #region Selection - Select / Deselect
        public virtual void SelectButton()
        {
            selectionImage.gameObject.SetActive(true);

            IsSelected = true;
        }

        public virtual void DeselectButton()
        {
            selectionImage.gameObject.SetActive(false);

            IsSelected = false;
        }
        #endregion

        #region Selection - Select / Deselect
        protected virtual void HoverButton()
        {

        }

        protected virtual void CancelHoverButton()
        {

        }
        #endregion

        #region Pointer events - Enter / Exit / Click
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            HoverButton();
            Debug.Log("Pointer enter");
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            CancelHoverButton();
            Debug.Log("Pointer exit");
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if(!IsSelected)
            {
                SelectButton();
            }

            Debug.Log("Pointer click");
        }
        #endregion
    }
}