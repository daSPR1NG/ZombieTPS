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

        private Animator _animator;

        private void OnEnable()
        {
            Actions.OnGlobalScoreValueChanged += SetScoreText;
        }

        private void OnDisable()
        {
            Actions.OnGlobalScoreValueChanged -= SetScoreText;
        }

        private void Start() => Init();

        private void Init()
        {
            _animator = GetComponent<Animator>();

            _scoreIconImage.sprite = _scoreIconSprite;
        }

        private void SetScoreText(int value, bool addingToScore)
        {
            _scoreText.SetText(value.ToString());

            if (value > 0) { _animator.Play("Score_Amount_BreathEffect"); }
        }
    }
}