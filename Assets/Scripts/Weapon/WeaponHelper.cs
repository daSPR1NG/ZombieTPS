using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Khynan_Coding
{
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
        [SerializeField] private List<GameObject> _weaponLookElements = new();

        #region Public References
        public Transform ShotPoint { get => shotPoint; }
        public AudioSource AudioSource { get => _audioSource; }
        public Transform BulletExitPoint { get => _bulletExitPoint; }
        #endregion

        private void Start() => Initialiaze();

        private void Initialiaze()
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

        private void SetWeaponLookElements()
        {
            Transform rendererParent = transform.parent.parent.parent;
            Weapon weapon = rendererParent.parent.GetComponent<WeaponSystem>().EquippedWeapon;

            // Magazine - 0
            weapon.SetMagPrefab(_weaponLookElements[0]);

            // Iron Sight - 1

            // Slide - 2

            // Trigger - 3
        }
    }
}