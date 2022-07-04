using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "Scriptable Object/Ammo Power Up", fileName = "New Ammo Power Up")]
    public class AmmoPowerUp : PowerUp
    {
        public override void Apply(Transform target)
        {
            Debug.Log(_name);

            WeaponSystem weaponSystem = target.GetComponent<WeaponSystem>();
            Weapon weapon = weaponSystem.EquippedWeapon;

            weapon.SetCurrentAmmo(weapon.GetMaxMagAmmo());
            weapon.SetCurrentMaxAmmo(weapon.GetMaxAmmo());

            GrabEffect(target);

            Actions.OnGettingMaxAmmo?.Invoke(weapon);
        }
    }
}