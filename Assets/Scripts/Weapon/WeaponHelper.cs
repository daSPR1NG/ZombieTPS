using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Khynan_Coding
{
    public enum WeaponLookElementPart
    {
        Unassigned, None, Magazine, IronSight, Slide, Trigger, Handle, //...
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class WeaponHelper : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Transform shotPoint;
        [SerializeField] private Transform _bulletExitPoint;
        private AudioSource _audioSource;

        [Header("HOLDING RIG LEFT & RIGHT IK")]
        [SerializeField] private Transform rightHandTargetIK;
        [SerializeField] private Transform rightHandHintIK;
        [SerializeField] private Transform leftHandTargetIK;
        [SerializeField] private Transform leftHandHintIK;

        [Header("RELOADING RIG LEFT IK")]
        [SerializeField] private Transform leftHandTargetAddIK;
        [SerializeField] private Transform leftHandHintAddIK;

        [Header("LOOK ELEMENTS")]
        [SerializeField] private List<WeaponLookElement> _weaponLookElements = new();

        #region Public References
        public Transform ShotPoint { get => shotPoint; }
        public AudioSource AudioSource { get => _audioSource; }
        public Transform BulletExitPoint { get => _bulletExitPoint; }
        #endregion

        [System.Serializable]
        private class WeaponLookElement
        {
            public GameObject ElementPrefab;
            public WeaponLookElementPart WeaponLookElementPart = WeaponLookElementPart.Unassigned;

        }

        private void Start() => Init();

        private void Init()
        {
            _audioSource = GetComponent<AudioSource>();

            SetWeaponLookElements();
        }

        public void InitHoldingIK(TwoBoneIKConstraint rightHoldingIK, TwoBoneIKConstraint leftHoldingIK)
        {
            if (rightHoldingIK)
            {
                SetHandIK(rightHoldingIK, rightHandTargetIK, rightHandHintIK);
            }

            if (leftHoldingIK)
            {
                SetHandIK(leftHoldingIK, leftHandTargetIK, leftHandHintIK);
            }  
        }

        public void InitReloadingIK(TwoBoneIKConstraint leftReloadingIK)
        {
            if (leftReloadingIK)
            {
                SetHandIK(leftReloadingIK, leftHandTargetAddIK, leftHandHintAddIK);
            }
        }

        private void SetHandIK(TwoBoneIKConstraint twoBoneIKConstraint, Transform targetIK, Transform hintIK)
        {
            twoBoneIKConstraint.data.target = targetIK;
            twoBoneIKConstraint.data.hint = hintIK;
        }

        public void ClearIK(TwoBoneIKConstraint twoBoneIKConstraint)
        {
            twoBoneIKConstraint.data.target = null;
            twoBoneIKConstraint.data.hint = null;
        }

        #region Weapon Look Elements - Get/Set
        public GameObject GetWeaponLookElements(WeaponLookElementPart weaponLookElementPart)
        {
            if (_weaponLookElements.Count == 0) { return null; }

            for (int i = 0; i < _weaponLookElements.Count; i++)
            {
                if (_weaponLookElements[i].WeaponLookElementPart != weaponLookElementPart) { continue; }

                return _weaponLookElements[i].ElementPrefab;
            }

            return null;
        }


        private void SetWeaponLookElements()
        {
            Transform rendererParent = transform.parent.parent.parent;
            Weapon weapon = rendererParent.parent.GetComponent<WeaponSystem>().EquippedWeapon;

            // Magazine - 0
            weapon.SetMagPrefab(GetWeaponLookElements(WeaponLookElementPart.Magazine));

            // Iron Sight - 1

            // Slide - 2

            // Trigger - 3
        }
        #endregion
    }
}