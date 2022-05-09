using UnityEngine;

namespace Khynan_Coding
{
    public interface IKillable
    {
        public void OnDeath(Transform killer);
    }
}