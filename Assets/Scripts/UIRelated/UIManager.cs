using UnityEngine;

namespace Khynan_Coding
{
    public class UIManager : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private GameObject pauseMenuComponent;

        #region Public references

        #endregion

        #region Singleton
        public static UIManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        #region Enable / Disable
        private void OnEnable()
        {
            Actions.OnGameStateChanged += TogglePauseMenu;
        }

        private void OnDisable()
        {
            Actions.OnGameStateChanged -= TogglePauseMenu;
        }
        #endregion

        private void TogglePauseMenu()
        {
            if (!Helper.IsThisUIWindowDisplayed(pauseMenuComponent))
            {
                Helper.DisplayUIWindow(pauseMenuComponent);
                return;
            }

            Helper.HideUIWindow(pauseMenuComponent);
        }
    }
}