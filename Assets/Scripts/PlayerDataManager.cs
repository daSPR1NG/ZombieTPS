using Cinemachine;
using UnityEngine;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class PlayerDataManager : MonoBehaviour
    {
        private Transform _playerCharacterTransform;

        [Space][Header("PLAYER INFORMATIONS")]
        public Vector3 PlayerPosition;

        #region References
        //
        #endregion

        private void Awake() => SetPlayerCharacterTransform();

        #region Player position - Save / Get / Set
        public void SavePlayerPosition()
        {
            PlayerPosition = GetCharacterPosition(_playerCharacterTransform);
            //Save it with PlayerPrefs
        }

        public Vector3 GetCharacterPosition(Transform observedTransform)
        {
            if (PlayerPosition == Vector3.zero)
            {
                return observedTransform.position;
            }

            return PlayerPosition;
        }

        public void SetCharacterPosition(Transform observedTransform, Vector3 newPos)
        {
            SetPlayerCharacterTransform();
            observedTransform.position = newPos;
        }
        #endregion

        #region Player Transform - Get /Set
        public Transform GetPlayerCharacterTransform()
        {
            return _playerCharacterTransform;
        }

        private void SetPlayerCharacterTransform()
        {
            _playerCharacterTransform = transform;
        }
        #endregion

        public bool PlayerCharacterIsActive()
        {
            if (_playerCharacterTransform.gameObject.activeInHierarchy)
            {
                return true;
            }

            return false;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) { return; }

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }
}