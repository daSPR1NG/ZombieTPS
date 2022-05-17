using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class ReticleReload : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Image reloadCircle;

        private float _maxValue;
        private float _currentTimer = 0;

        private void OnEnable()
        {
            Actions.OnReloadValidThresholdReached += ChangeReloadCircleColor;
        }

        private void OnDisable() 
        {
            Actions.OnReloadValidThresholdReached -= ChangeReloadCircleColor;

            reloadCircle.color = Color.white; 
        }

        void Update() => ProcessReloadTimer();

        public void Init(float value)
        {
            _currentTimer = value;
            _maxValue = value;

            reloadCircle.fillAmount = _currentTimer / _maxValue;
        }

        private void ProcessReloadTimer()
        {
            _currentTimer -= Time.deltaTime;

            reloadCircle.fillAmount = _currentTimer / _maxValue;
        }

        private void ChangeReloadCircleColor(Color color)
        {
            reloadCircle.color = color;
        }
    }
}