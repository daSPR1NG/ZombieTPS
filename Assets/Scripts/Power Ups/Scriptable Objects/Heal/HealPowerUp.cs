using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "Scriptable Object/Heal Power Up", fileName = "New Heal Power Up")]
    public class HealPowerUp : PowerUp
    {
        public override void Apply(Transform target)
        {
            StatsManager statsManager = target.GetComponent<StatsManager>();
            statsManager.HealTarget(target, target, statsManager.GetStat(StatAttribute.Health).GetMaxValue());

            GrabEffect(target);
        }
    }
}