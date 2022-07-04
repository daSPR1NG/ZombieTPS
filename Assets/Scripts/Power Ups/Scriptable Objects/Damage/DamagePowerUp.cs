using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "Scriptable Object/Damage Power Up", fileName = "New Damage Power Up")]
    public class DamagePowerUp : PowerUp
    {
        [Space(3), Header("DAMAGE UP SETTINGS")]
        [SerializeField] private float _damageFlatBonusValue;

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

            _weaponSystem = target.GetComponent<WeaponSystem>();
            _weapon = _weaponSystem.EquippedWeapon;

            StatsManager statsManager = target.GetComponent<StatsManager>();

            statsManager.GetStat(StatAttribute.AttackDamage).AddModifier(
                new StatModifier(   ModifierType.Flat,
                                    _damageFlatBonusValue,
                                    this,
                                    StatAttribute.AttackDamage));

            _weapon.SetDamage((int)statsManager.GetStat(StatAttribute.AttackDamage).GetCurrentValue());

            statsManager.GetStat(StatAttribute.CriticalChance).AddModifier(
                new StatModifier(ModifierType.Flat,
                                    _damageFlatBonusValue,
                                    this,
                                    StatAttribute.CriticalChance));

            powerUpManager.AddPowerUpToList(this);
        }

        public override void RemovePowerUpEffect(Transform target)
        {
            base.RemovePowerUpEffect(target);

            StatsManager statsManager = target.GetComponent<StatsManager>();

            statsManager.GetStat(StatAttribute.AttackDamage).RemoveSourceModifier(this);
            statsManager.GetStat(StatAttribute.CriticalChance).RemoveSourceModifier(this);

            _weapon.SetDamage((int)statsManager.GetStat(StatAttribute.AttackDamage).GetCurrentValue());
        }
    }
}