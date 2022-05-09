using UnityEngine;

namespace Khynan_Coding
{
    public interface IDamageable
    {
        public void ApplyDamageToTarget(
            Transform provider, 
            Transform target, 
            float damageAmount);
    }
}