using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class WeaponCompartment : MonoBehaviour
    {
        [SerializeField] private GameObject weaponBoxPrefab;
        [SerializeField] private List<WeaponBox> weaponBoxes = new();

        private UIThirdPersonManager _UIThirdPersonManager;
        public UIThirdPersonManager UIThirdPersonManager { get => _UIThirdPersonManager; private set => _UIThirdPersonManager = value; }

        private void Awake() => Init();

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            Actions.OnEquippingWeapon += SelectThisWeaponBox;
        }

        private void OnDisable()
        {
            Actions.OnEquippingWeapon -= SelectThisWeaponBox;
        }
        #endregion

        private void Init()
        {
            UIThirdPersonManager = transform.parent.GetComponent<UIThirdPersonManager>();

            InitWeaponBoxes();
        }

        private void InitWeaponBoxes()
        {
            for (int i = 0; i < UIThirdPersonManager.LinkedWeaponSystem.Weapons.Count; i++)
            {
                CreateAWeaponBox();
            }

            SetWeaponInAllWeaponBoxes(UIThirdPersonManager.LinkedWeaponSystem);
            DeselectEachWeaponBoxes();
        }

        private void CreateAWeaponBox()
        {
            GameObject weaponBoxInstance = Instantiate(weaponBoxPrefab, transform);

            if (weaponBoxInstance.TryGetComponent(out WeaponBox weaponBox))
            {
                weaponBoxes.Add(weaponBox);
            }
        }

        private void SetWeaponInAllWeaponBoxes(WeaponSystem weaponSystem)
        {
            for (int i = 0; i < weaponSystem.Weapons.Count; i++)
            {
                weaponBoxes[i].Init(weaponSystem.Weapons[i]);
            }
        }

        private void DeselectEachWeaponBoxes()
        {
            if (weaponBoxes.Count == 0) { return; }

            for (int i = 0; i < weaponBoxes.Count; i++)
            {
                weaponBoxes[i].Deselect();
            }
        }

        private void SelectThisWeaponBox(Weapon weapon, int index)
        {
            DeselectEachWeaponBoxes();

            weaponBoxes[index].Select();
        }
    }
}