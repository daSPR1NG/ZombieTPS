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

        private void OnEnable()
        {
            Actions.OnWaveCountValueChanged += SetWaveCounterText;
        }

        private void OnDisable()
        {
            Actions.OnWaveCountValueChanged -= SetWaveCounterText;
        }

        void Start() => Init();

        void Init()
        {
            waveIconImage.sprite = waveIcon;
            SetWaveCounterText(0);
        }

        private void SetWaveCounterText(int waveCount)
        {
            waveCounterText.SetText(waveCount.ToString());
        }
    }
}