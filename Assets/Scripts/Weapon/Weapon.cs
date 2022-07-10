using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum WeaponType
    {
        Unassigned, Single, Automatic, SemiAutomatic, Burst
    }

    [CreateAssetMenu(menuName = "ScriptableObject/Weapon", fileName = "Weapon_", order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("GENERAL SETTING")]
        [SerializeField] private string _name;

        [Header("SHOT SETTINGS")]
        [SerializeField] private WeaponType _type = WeaponType.Unassigned;
        [SerializeField] private int _damage = 10, _ammoFiredPerShot = 1;
        [SerializeField] private float _fireRate = .15f, _range = 15f;
        
        [Header("AIM SETTINGS")]
        [SerializeField, Range(0, 2)] private float _aimSpeed = .5f;

        [Header("AMMO SETTINGS")]
        public int BaseMaxAmmo = 350;
        [SerializeField] private int _maxAmmo = 150;
        private int _currentAmmo, _currentMaxAmmo = 0;

        [Header("MAG SETTINGS")]
        [SerializeField] private int _magAmount = 6;
        private int _maxMagAmmo;

        [Header("RELOADING SETTINGS")]
        [SerializeField] private float _timeToReload = 2f;

        [Header("RECOIL SETTINGS")]
        [SerializeField] private float _recoilForce = 5f;
        [SerializeField] private float _impulseForce = .25f;

        [Header("STATIC LOOK ELEMENTS")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Sprite _icon;
        [SerializeField] private GameObject _muzzleFlashPf, _bulletTrailPf, _bulletPf, _magPf;

        [Header("ALTERNATE LOOK ELEMENTS")]
        [SerializeField] private GameObject[] _muzzleFlashes;
        [SerializeField] private GameObject[] _bulletTrailPfs;
        [SerializeField] private GameObject[] _bulletPfs;

        [Header("SOUND")]
        public bool EmitsSound = true;
        [field: SerializeField] public WeaponAudioSettingList WeaponAudioSettingList { get; private set; }

        private void OnDisable()
        {
            SetDefaultLook();
        }

        public void Init()
        {
            SetMaxMagAmmo();
            SetDefaultLook();
        }

        public string GetName() { return _name; }

        private void SetDefaultLook()
        {
            SetBulletPf(null, 0);
            SetBulletTrailPf(null, 0);
            SetMuzzleFlash(null, 0);
        }

        #region Weapon Type - Get
        public WeaponType GetWeaponType() { return _type; }
        public void SetWeaponType(WeaponType weaponType) { _type = weaponType; }
        #endregion

        #region Max Ammo + Current Max Ammo - Get / Set
        public int GetMaxAmmo() { return _maxAmmo; }
        public void SetMaxAmmo(int value)
        { 
            _maxAmmo = value;
            _currentMaxAmmo = _maxAmmo;
        }

        public int GetCurrentMaxAmmo() { return _currentMaxAmmo; }
        public void SetCurrentMaxAmmo(int value) 
        {
            _currentMaxAmmo = value; 
        }
        #endregion

        #region Max Mag Ammo - Get / Set
        public int GetMaxMagAmmo() { return _maxMagAmmo; }

        public void SetMaxMagAmmo()
        {
            _maxMagAmmo = Mathf.FloorToInt(_maxAmmo / _magAmount);
        }
        #endregion

        #region Current Ammo - Get / Set
        public int GetCurrentAmmo() { return _currentAmmo; }

        public void SetCurrentAmmo(int value)
        {
            _currentAmmo = value;

            _currentAmmo = Mathf.Clamp(_currentAmmo, 0, _maxMagAmmo);
        }
        #endregion

        #region Ammo Fired Per Shot - Get / Set
        public int GetAmmoFiredPerShot() { return _ammoFiredPerShot; }

        public void SetAmountOfAmmoFiredPerShot(int value)
        {
            _ammoFiredPerShot = value;
        }
        #endregion

        #region Aim Speed - Get / Set
        public float GetAimSpeed() { return _aimSpeed; }

        public void SetAimSpeed(float value)
        {
            _aimSpeed = value;
        }
        #endregion

        #region Fire Rate - Get / Set
        public float GetFireRate() { return _fireRate; }

        public void SetFireRate(float value)
        {
            _fireRate = value;
        }
        #endregion

        #region Reloading Timer - Get / Set
        public float GetReloadingTimer() { return _timeToReload; }

        public void SetReloadingSpeed(float value)
        {
            _timeToReload = value;
        }
        #endregion

        #region Range - Get / Set
        public float GetRange() { return _range; }

        public void SetRange(float value)
        {
            _range = value;
        }
        #endregion

        #region Damage - Get / Set
        public int GetDamage() { return _damage; }

        public void SetDamage(int value)
        {
            _damage = value;
        }
        #endregion

        #region Recoil Force - Get / Set
        public float GetRecoilForce() { return _recoilForce; }
        public void SetRecoilForce(float value)
        {
            _recoilForce = value;
        }
        #endregion

        #region Impulse Force - Get / Set
        public float GetImpulseForce() { return _impulseForce; }
        public void SetImpulseForce(float value)
        {
            _impulseForce = value;
        }
        #endregion

        #region Prefab - Get
        public GameObject GetPrefab() { return _prefab; }
        #endregion

        #region Icon - Get / Set
        public Sprite GetIcon() { return _icon; }
        public void SetWeaponIcon(Sprite newIcon)
        {
            _icon = newIcon;
            // Event to push
        }
        #endregion

        #region Muzzle Flash - Get / Set
        public GameObject GetMuzzleFlash() { return _muzzleFlashPf; }
        public GameObject GetMuzzleFlashOtherLook(int index) { return _muzzleFlashes[index]; }
        public void SetMuzzleFlash(GameObject gameObject, int index = 25000) 
        {
            if (index != 25000)
            {
                index = Mathf.Clamp(index, 0, _muzzleFlashes.Length);

                _muzzleFlashPf = _muzzleFlashes[index];
                return;
            }

            _muzzleFlashPf = gameObject; 
        }
        #endregion

        #region Bullet Trail Prefab - Get / Set
        public GameObject GetBulletTrailPf() { return _bulletTrailPf; }
        public void SetBulletTrailPf(GameObject gameObject, int index = 25000)
        {
            if (index != 25000)
            {
                index = Mathf.Clamp(index, 0, _bulletTrailPfs.Length);

                _bulletTrailPf = _bulletTrailPfs[index];
                return;
            }

            _bulletTrailPf = gameObject;
        }
        #endregion

        #region Bullet Prefab - Get / Set
        public GameObject GetBulletPf() { return _bulletPf; }
        public void SetBulletPf(GameObject gameObject, int index = 25000)
        {
            if (index != 25000)
            {
                index = Mathf.Clamp(index, 0, _bulletPfs.Length);

                _bulletPf = _bulletPfs[index];
                return;
            }

            _bulletPf = gameObject;
        }
        #endregion

        #region Mag Prefab - Get/Set
        public GameObject GetMagPrefab() { return _magPf; }
        public void SetMagPrefab(GameObject gameObject) 
        { 
            if (_magPf == gameObject) { return; }

            _magPf = gameObject; 
        }
        #endregion
    }
}