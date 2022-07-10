using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class DeadScreenEffect : MonoBehaviour
    {
        public Animator Animator;

        public TMP_Text ScoreText;
        public TMP_Text WaveText;

        public AudioSource AudioSource;
        public AudioClip AudioClip;

        void OnEnable() => Actions.OnPlayerDeath += DisplayDeathScreen;

        void OnDisable() => Actions.OnPlayerDeath -= DisplayDeathScreen;

        private void Start() => Init();

        void Init()
        {
            Color color = Color.white;
            color.a = 0;

            ScoreText.color = color;
            WaveText.color = color;
        }

        void DisplayDeathScreen()
        {
            AnimatorHelper.HandleThisAnimation( Animator, "Death", true );

            ScoreManager scoreManager = GameManager.Instance.ActivePlayer.GetComponent<ScoreManager>();
            ScoreText.SetText( "Your score: " + scoreManager.GlobalScore.ToString() + "." );

            int waveNumber = GameManager.Instance.WaveCount;
            WaveText.SetText( "You survived for " + waveNumber.ToString() + " wave(s)." );
        }

        // Called in animation
        void PlaySpecialDeathSFX()
        {
            AudioHelper.PlaySound( AudioSource, AudioClip, AudioSource.volume );
        }
    }
}