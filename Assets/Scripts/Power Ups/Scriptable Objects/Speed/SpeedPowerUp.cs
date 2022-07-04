using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "Scriptable Object/Speed Power Up", fileName = "New Speed Power Up")]
    public class SpeedPowerUp : PowerUp
    {
        [Space(3), Header("SPEED UP SETTINGS")]
        [SerializeField] private float _moveSpeedFlatBonusValue;
        [SerializeField] private float _fireRateFlatBonusValue;

        private ThirdPersonController _controller;

        private WeaponSystem _weaponSystem;
        private Weapon _weapon;

        public override void Apply(Transform target)
        {
            // Boost damage and CC
            PowerUpManager powerUpManager = target.GetComponent<PowerUpManager>();

            if (!powerUpManager) { return; }

            if (powerUpManager.DoesPUpManagerContainsThisPUp(this))
            {
                powerUpManager.RefreshPowerUpDuration(this);
                return;
            }

            _controller = target.GetComponent<ThirdPersonController>();

            _weaponSystem = target.GetComponent<WeaponSystem>();
            _weapon = _weaponSystem.EquippedWeapon;

            StatsManager statsManager = target.GetComponent<StatsManager>();

            statsManager.GetStat(StatAttribute.MovementSpeed).AddModifier(
                new StatModifier(ModifierType.Flat,
                                    _moveSpeedFlatBonusValue,
                                    this,
                                    StatAttribute.MovementSpeed));

            _controller.SetTargetSpeedValues(statsManager.GetStat(StatAttribute.MovementSpeed).GetMaxValue());

            statsManager.GetStat(StatAttribute.FireRate).AddModifier(
                new StatModifier(ModifierType.Flat,
                                    _fireRateFlatBonusValue,
                                    this,
                                    StatAttribute.FireRate));
            _weapon.SetFireRate(statsManager.GetStat(StatAttribute.FireRate).GetCurrentValue());

            powerUpManager.AddPowerUpToList(this);
        }

        public override void RemovePowerUpEffect(Transform target)
        {
            base.RemovePowerUpEffect(target);

            StatsManager statsManager = target.GetComponent<StatsManager>();

            statsManager.GetStat(StatAttribute.MovementSpeed).RemoveSourceModifier(this);
            statsManager.GetStat(StatAttribute.FireRate).RemoveSourceModifier(this);

            _controller.SetTargetSpeedValues(statsManager.GetStat(StatAttribute.MovementSpeed).GetMaxValue());
            _weapon.SetFireRate(statsManager.GetStat(StatAttribute.FireRate).GetCurrentValue());
        }
    }
}