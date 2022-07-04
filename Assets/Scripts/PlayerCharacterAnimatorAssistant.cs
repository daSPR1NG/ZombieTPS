using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class PlayerCharacterAnimatorAssistant : MonoBehaviour
    {
        public AudioClip HitSFX;
        public AudioSource AudioSource;

        public void PlayHitSound()
        {
            AudioHelper.PlaySound( AudioSource, HitSFX, AudioSource.volume );
        }

        public void ResetGotHitBoolean()
        {
            GetComponent<Animator>().SetBool( "GotHit", false );
        }
    }
}