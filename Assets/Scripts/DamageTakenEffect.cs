using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class DamageTakenEffect : MonoBehaviour
    {
        public Animator Animator;

        void OnEnable() => Actions.OnPlayerDamageTaken += DisplayDamageTakenEffect;

        void OnDisable() => Actions.OnPlayerDamageTaken -= DisplayDamageTakenEffect;

        private void DisplayDamageTakenEffect()
        {
            Animator.ResetTrigger( "DamageTaken" );

            Animator.SetTrigger( "DamageTaken" );
        }
    }
}