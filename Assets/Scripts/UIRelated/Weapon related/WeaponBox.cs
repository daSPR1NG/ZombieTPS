using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class WeaponBox : MonoBehaviour
    {
        [Header("SELECTION DEPENDENCIES")]
        [SerializeField] private GameObject selectedGO;
        [SerializeField] private GameObject notSelectedGO;

        [Header("LOOK DEPENDENCIES")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponCurrentAmmoText;
        [SerializeField] private TMP_Text weaponMaxAmmoText;
        [SerializeField] private Gradient colorGradient;

        int animationIDBreathEffect;

        private Weapon _weaponReference;
        private WeaponCompartment _weaponCompartment;
        private Animator _animator;

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            Actions.OnInitializingWeapon += InitializeAmmoText;

            Actions.OnShooting += SetAmmoText;
            Actions.OnReloadEndedSetWeaponData += SetAmmoText;
        }

        private void OnDisable()
        {
            Actions.OnInitializingWeapon -= InitializeAmmoText;

            Actions.OnShooting -= SetAmmoText;
            Actions.OnReloadEndedSetWeaponData -= SetAmmoText;
        }
        #endregion

        public void Init(Weapon weapon)
        {
            _weaponCompartment = transform.parent.GetComponent<WeaponCompartment>();

            _animator = GetComponent<Animator>();
            animationIDBreathEffect = Animator.StringToHash("AmmoText_BreathEffect");

            weaponIcon.sprite = weapon.GetIcon();
            weaponCurrentAmmoText.SetText(weapon.GetCurrentAmmo() + " | " + weapon.GetMaxAmmo());

            _weaponReference = weapon;
        }

        private void InitializeAmmoText(Weapon weapon)
        {
            SetCurrentAndMaxAmmoTexts(weapon.GetCurrentAmmo().ToString(), "| " + weapon.GetMaxAmmo().ToString());

            SetCurrentAmmoTextColor(colorGradient.Evaluate(
                Helper.GetPercentage(weapon.GetCurrentAmmo(), weapon.GetMaxMagAmmo())));
        }

        private void SetAmmoText(Weapon weapon)
        {
            if (weapon != _weaponReference) { return; }

            SetCurrentAndMaxAmmoTexts(weapon.GetCurrentAmmo().ToString(), "| " + weapon.GetMaxAmmo().ToString());

            SetCurrentAmmoTextColor(colorGradient.Evaluate(
                Helper.GetPercentage(weapon.GetCurrentAmmo(), weapon.GetMaxMagAmmo())));

            _animator.Play(animationIDBreathEffect, 0);
        }

        private void SetCurrentAndMaxAmmoTexts(string currentAmmo, string maxAmmo)
        {
            weaponCurrentAmmoText.SetText(currentAmmo);
            weaponMaxAmmoText.SetText(maxAmmo);
        }

        private void SetCurrentAmmoTextColor(Color color)
        {
            weaponCurrentAmmoText.color = color;
        }

        public void Select()
        {
            if (notSelectedGO.activeInHierarchy) { notSelectedGO.SetActive(false); }

            selectedGO.SetActive(true);
        }

        public void Deselect()
        {
            if (selectedGO.activeInHierarchy) { selectedGO.SetActive(false); }

            notSelectedGO.SetActive(true);
        }
    }
}