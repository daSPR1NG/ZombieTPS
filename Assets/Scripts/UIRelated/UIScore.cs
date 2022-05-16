using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class UIScore : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Image _scoreIconImage;
        [SerializeField] private Sprite _scoreIconSprite;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _scoreMultiplierText;

        private Animator _animator;

        private void OnEnable()
        {
            Actions.OnGlobalScoreValueChanged += SetScoreText;
            Actions.OnScoreMultiplierValueChanged += SetScoreMultiplierText;
        }

        private void OnDisable()
        {
            Actions.OnGlobalScoreValueChanged -= SetScoreText;
            Actions.OnScoreMultiplierValueChanged -= SetScoreMultiplierText;
        }

        private void Start() => Init();

        private void Init()
        {
            _animator = GetComponent<Animator>();

            _scoreIconImage.sprite = _scoreIconSprite;
            _scoreMultiplierText.SetText("x1");
        }

        private void SetScoreText(int value, bool addingToScore)
        {
            if (value > 0) { _animator.Play("Score_Amount_BreathEffect"); }

            // Switch score text color to green or red - then in the animation set that color to white again
            switch (addingToScore)
            {
                // We are adding score
                case true:
                    break;
                // We are removing score
                case false:
                    break;
            }

            // Change score string value when reaching over 1.000, 1.000.000
            if (value < 1_000)
            {
                _scoreText.SetText(value.ToString());
                return;
            }

            if (value >= 1_000 && value < 1_000_000)
            {
                _scoreText.SetText((value / 1_000).ToString() + "." + (value % 1_000 / 10).ToString() + "K");
                return;
            }

            _scoreText.SetText(value.ToString());

            //if (value >= 1_000_000)
            //{
            //    _scoreText.SetText((value / 1_000_000).ToString() + "M");
            //    return;
            //}
        }

        private void SetScoreMultiplierText(float multiplierValue)
        {
            _scoreMultiplierText.SetText("x" + multiplierValue);
        }
    }
}