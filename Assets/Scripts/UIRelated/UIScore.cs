using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class UIScore : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private void OnEnable()
        {
            Actions.OnGlobalScoreValueChanged += SetScoreText;
        }

        private void OnDisable()
        {
            Actions.OnGlobalScoreValueChanged -= SetScoreText;
        }

        private void SetScoreText(int value)
        {
            scoreText.SetText(value.ToString());
        }
    }
}