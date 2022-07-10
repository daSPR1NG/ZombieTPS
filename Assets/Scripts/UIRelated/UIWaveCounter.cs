using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class UIWaveCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text waveCounterText;
        [SerializeField] private Image waveIconImage;
        [SerializeField] private Sprite waveIcon;

        private Animator _animator;
        private int _waveCount;

        private void OnEnable()
        {
            Actions.OnWaveCountValueChanged += SetWaveCounterAndDisplayFeedback;
        }

        private void OnDisable()
        {
            Actions.OnWaveCountValueChanged -= SetWaveCounterAndDisplayFeedback;
        }

        void Start() => Init();

        void Init()
        {
            _animator = GetComponent<Animator>();

            //waveIconImage.sprite = waveIcon;

            SetWaveCounterAndDisplayFeedback(0);
            waveCounterText.SetText( _waveCount.ToString() );
        }

        private void SetWaveCounterAndDisplayFeedback( int waveCount )
        {
            _waveCount = waveCount;
            _animator.Play( "WaveCount" );
        }

        // Called in animation
        public void SetWaveCounterText()
        {
            waveCounterText.SetText( _waveCount.ToString() );
        }
    }
}