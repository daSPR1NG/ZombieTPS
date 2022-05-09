using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class UIHealthBar : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private TMP_Text _healthValueText;
        [SerializeField] private Image _healthIconImage;
        [SerializeField] private Sprite _healthIcon;

        [Header("FILL BARS")]
        [SerializeField] private Image _healthFillImage;
        [SerializeField] private Image _damagedFillImage;

        [Header("SETTINGS")]
        [SerializeField] private float _damagedFillBarUpdateDelay = .5f;
        [SerializeField] private float _damagedFillBarUpdateSpeed = 1.5f;

        private bool _canUpdateDamagedFillBar = false;
        private float _damageUpdateCurrentTimer;

        private float _currentValue;
        private float _maxValue;

        private Animator _animator;

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            Actions.OnPlayerHealthValueInitialized += InitHealthBarFill;
            Actions.OnPlayerHealthValueChanged += SetHealthBar;
        }

        private void OnDisable()
        {
            Actions.OnPlayerHealthValueInitialized -= InitHealthBarFill;
            Actions.OnPlayerHealthValueChanged -= SetHealthBar;
        }
        #endregion

        void Start() => Init();

        private void Update() => ProcessTimerBeforeUpdatingDamagedFillBar();

        void Init()
        {
            _healthIconImage.sprite = _healthIcon;
            _animator = GetComponent<Animator>();
        }

        private void InitHealthBarFill(float current, float max)
        {
            _currentValue = current;
            _maxValue = max;

            _healthValueText.SetText(current.ToString() + " / " + max.ToString());

            _healthFillImage.fillAmount = current / max;
            _damagedFillImage.fillAmount = current / max;
        }

        private void SetHealthBar(float current, float max, HealthInteraction healthInteraction)
        {
            Debug.Log("Set health bar.");

            _currentValue = current;
            _maxValue = max;

            _healthValueText.SetText(current.ToString() + " / " + max.ToString());

            _healthFillImage.fillAmount = current / max;

            _animator.Play("HealthBar_DamageTaken");

            switch (healthInteraction)
            {
                case HealthInteraction.Damage:
                    _canUpdateDamagedFillBar = true;
                    _damageUpdateCurrentTimer = _damagedFillBarUpdateDelay;
                    break;
                case HealthInteraction.Heal:
                    SetDamagedFillBarImmediatly(current, max);
                    break;
            }
        }

        private void ProcessTimerBeforeUpdatingDamagedFillBar()
        {
            if (!_canUpdateDamagedFillBar) { return; }

            _damageUpdateCurrentTimer -= Time.deltaTime;

            if (_damageUpdateCurrentTimer <= 0)
            {
                _damageUpdateCurrentTimer = 0;
                SetDamagedFillBarOvertime(_currentValue, _maxValue);
            }
        }

        private void SetDamagedFillBarOvertime(float current, float max)
        {
            float nextFillValue = current / max;

            _damagedFillImage.fillAmount = 
                Mathf.Lerp(_damagedFillImage.fillAmount, nextFillValue, Time.deltaTime * _damagedFillBarUpdateSpeed);

            if (_damagedFillImage.fillAmount != nextFillValue) { return; }

            _canUpdateDamagedFillBar = false;
        }

        private void SetDamagedFillBarImmediatly(float current, float max)
        {
            _damagedFillImage.fillAmount = current / max;
        }
    }
}