using UnityEngine;

namespace Khynan_Coding
{
    public class UIThirdPersonManager : MonoBehaviour
    {
        private Transform _parent;
        private WeaponSystem _linkedWeaponSystem;

        #region Public References
        public WeaponSystem LinkedWeaponSystem { get => _linkedWeaponSystem; }
        #endregion

        void Awake() => Init();

        void Init()
        {
            _parent = transform.parent;
            _linkedWeaponSystem = _parent.GetComponent<WeaponSystem>();
        }
    }
}