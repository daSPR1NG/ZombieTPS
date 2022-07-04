using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public enum ElementalDamageType
    {
        Unassigned, None, Fire, Frost
    }

    public class WeaponElement : MonoBehaviour
    {
        // input fetch + event
        [SerializeField] private ElementalDamageType _elementalDamageType = ElementalDamageType.None;
        [SerializeField] private DamageOvertime _damageOvertime;
        [SerializeField] private Freeze _freeze;

        private WeaponSystem _weaponSystem;

        private PlayerInput _playerInputs;
        private InputAction _switchDamageType;

        void OnEnable()
        {
            _playerInputs = GetComponent<PlayerInput>();
            _switchDamageType = _playerInputs.actions["Swap Weapon"];

            _switchDamageType.performed += context => SwitchElementalDamage();
        }

        void OnDisable()
        {
            _switchDamageType.performed -= context => SwitchElementalDamage();
        }

        void Start() => Init();

        void Init()
        {
            _weaponSystem = GetComponent<WeaponSystem>();
        }

        public void ApplyElementalEffect(Transform target)
        {
            EffectManager effectManager = target.GetComponent<EffectManager>();

            if (target.Equals(null) || !effectManager) { return; }

            switch (_elementalDamageType)
            {
                case ElementalDamageType.Fire:
                    //_damageOvertime.SetTarget(target);
                    //_damageOvertime.Build(effectManager.HasOtherInstance(_damageOvertime));
                    new DamageOvertime(0, transform, target, 15, 4, 2, 7f).Build();
                    break;
                case ElementalDamageType.Frost:
                    new Freeze(1, transform, target, 8, 1).Build();
                    break;
            }
        }

        private void SwitchElementalDamage()
        {
            if (_weaponSystem.IsReloading) { return; }

            WeaponHelper weaponHelper = _weaponSystem.GetEquippedWeaponWeaponHelper();

            switch (_elementalDamageType)
            {
                case ElementalDamageType.None:
                    _elementalDamageType = ElementalDamageType.Fire;
                    weaponHelper.DisplayLookElement(WeaponLookElementPart.FireElements);

                    _weaponSystem.EquippedWeapon.SetBulletPf(null, 1);
                    _weaponSystem.EquippedWeapon.SetBulletTrailPf(null, 1);
                    _weaponSystem.EquippedWeapon.SetMuzzleFlash(null, 1);

                    ChangeMuzzleFlash();
                    break;
                case ElementalDamageType.Fire:
                    _elementalDamageType = ElementalDamageType.Frost;
                    weaponHelper.DisplayLookElement(WeaponLookElementPart.FrostElements);

                    _weaponSystem.EquippedWeapon.SetBulletPf(null, 2);
                    _weaponSystem.EquippedWeapon.SetBulletTrailPf(null, 2);
                    _weaponSystem.EquippedWeapon.SetMuzzleFlash(null, 2);

                    ChangeMuzzleFlash();
                    break;
                case ElementalDamageType.Frost:
                    _elementalDamageType = ElementalDamageType.None;
                    weaponHelper.HideAllElementalWeaponLookEffect();

                    _weaponSystem.EquippedWeapon.SetBulletPf(null, 0);
                    _weaponSystem.EquippedWeapon.SetBulletTrailPf(null, 0);
                    _weaponSystem.EquippedWeapon.SetMuzzleFlash(null, 0);

                    ChangeMuzzleFlash();
                    break;
            }

            // UI event call
        }

        private void ChangeMuzzleFlash()
        {
            _weaponSystem.SetMuzzleFlashInstance(_elementalDamageType);
        }
    }
}