using UnityEngine;

namespace Khynan_Coding
{
    public interface IRespawnable
    {
        public abstract void Respawn(Transform who, Transform where);
    }
}