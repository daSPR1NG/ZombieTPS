using UnityEngine;

namespace Khynan_Coding
{
    public interface IHealable
    {
        public void HealTarget(Transform provider, Transform target, float healAmount);
    }
}