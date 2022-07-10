using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class WeaponCompartment : MonoBehaviour
    {
        [SerializeField] private GameObject weaponBoxPrefab;
        [SerializeField] private List<WeaponBox> weaponBoxes = new();

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

        private void Init() => InitWeaponBoxes();

        private void InitWeaponBoxes()
        {
            WeaponSystem WeaponSystem = transform.parent.parent.parent.GetComponent<WeaponSystem>();

            for (int i = 0; i < WeaponSystem.Weapons.Count; i++)
            {
                CreateAWeaponBox();
            }

            SetWeaponInAllWeaponBoxes(WeaponSystem);
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