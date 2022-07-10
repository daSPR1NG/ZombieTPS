using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Object/Score Power Up", fileName = "New Score Power Up" )]
    public class ScoreUp : PowerUp
    {
        public float ScoreMultiBonusValue = 2;
        private float m_previousScoreMultiValue = 0;

        public override void Apply( Transform target )
        {
            // Boost damage and CC
            PowerUpManager powerUpManager = target.GetComponent<PowerUpManager>();

            if ( !powerUpManager ) { return; }

            if ( powerUpManager.DoesPUpManagerContainsThisPUp( this ) )
            {
                powerUpManager.RefreshPowerUpDuration( this );
                return;
            }

            m_previousScoreMultiValue = GameManager.Instance.ScoreMultiplier;
            GameManager.Instance.ScoreMultiplier = ScoreMultiBonusValue;

            GrabEffect( target );

            powerUpManager.AddPowerUpToList( this );
            Actions.OnScoreMultiplierValueChanged?.Invoke( GameManager.Instance.ScoreMultiplier );
        }

        public override void RemovePowerUpEffect( Transform target )
        {
            GameManager.Instance.ScoreMultiplier = m_previousScoreMultiValue;
            Actions.OnScoreMultiplierValueChanged?.Invoke( GameManager.Instance.ScoreMultiplier );

            base.RemovePowerUpEffect( target );
        }
    }
}