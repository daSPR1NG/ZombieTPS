using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class PlayerCharacterAnimatorAssistant : MonoBehaviour
    {
        public AudioClip HitSFX;
        public AudioClip DeathSFX;
        public AudioSource AudioSource;

        public void PlayHitSound()
        {
            float random = Random.Range( 0.75f, 0.95f );
            AudioHelper.SetPitch( AudioSource, random );

            AudioHelper.PlaySound( AudioSource, HitSFX, AudioSource.volume );
        }

        public void PlayDeathSound()
        {
            AudioHelper.PlaySound( AudioSource, DeathSFX, AudioSource.volume );
        }

        public void ResetGotHitBoolean()
        {
            AnimatorHelper.HandleThisAnimation( GetComponent<Animator>(), "GotHit", false, 1, 1 );
        }
    }
}