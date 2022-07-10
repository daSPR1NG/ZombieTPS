using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    // NOTES : CREATE AN EDITOR PROPERTY DRAWER TO MASK VARIABLES 

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class UIHealthBar : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] protected bool _displaysTextInfos = false;

        [Header("DEPENDENCIES")]
        [SerializeField] protected TMP_Text _healthValueText;
        [SerializeField] protected Image _healthIconImage;
        [SerializeField] protected Sprite _healthIcon;

        [Header("FILL BARS")]
        [SerializeField] protected Image _healthFillImage;
        [SerializeField] protected Image _damagedFillImage;

        [Header("FILL SPEED SETTINGS")]
        [SerializeField] protected float _damagedFillBarUpdateDelay = .5f;
        [SerializeField] protected float _damagedFillBarUpdateSpeed = 1.5f;

        protected bool _canUpdateDamagedFillBar = false;
        protected float _damageUpdateCurrentTimer;

        protected float _currentValue;
        protected float _maxValue;

        protected Animator _animator;

        #region OnEnable / OnDisable
        protected virtual void OnEnable()
        {
            Actions.OnPlayerHealthValueInitialized += InitHealthBarFill;
            Actions.OnPlayerHealthValueChanged += SetHealthBar;
            Actions.OnPlayerHealthValueAugmented += SetDamagedFillBarImmediatly;
        }

        protected virtual void OnDisable()
        {
            Actions.OnPlayerHealthValueInitialized -= InitHealthBarFill;
            Actions.OnPlayerHealthValueChanged -= SetHealthBar;
            Actions.OnPlayerHealthValueAugmented -= SetDamagedFillBarImmediatly;
        }
        #endregion

        void Start() => Init();

        private void Update() => ProcessTimerBeforeUpdatingDamagedFillBar();

        void Init()
        {
            _animator = GetComponent<Animator>();

            if (!_displaysTextInfos) { _healthValueText.gameObject.SetActive(false); }

            if (_healthIconImage) { _healthIconImage.sprite = _healthIcon; }
        }

        public virtual void InitHealthBarFill(float current, float max)
        {
            //Debug.Log("InitHealthBarFill");

            _currentValue = current;
            _maxValue = max;

            if (_displaysTextInfos) { _healthValueText.SetText(current.ToString()); }

            //_healthValueText.SetText($"<size=18>{current}</size>" + " " + $"<size=12><color=grey>{max}</color></size>");

            _healthFillImage.fillAmount = current / max;

            //Debug.Log(_healthFillImage.fillAmount + " " + current + " " + max);

            _damagedFillImage.fillAmount = current / max;
        }

        public virtual void SetHealthBar(float current, float max, HealthInteraction healthInteraction)
        {
            //Debug.Log("Set health bar.");

            _currentValue = current;
            _maxValue = max;

            if (_displaysTextInfos)
            {
                _healthValueText.SetText(current.ToString()/* + " / " + max.ToString()*/);
            }

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

        protected virtual void ProcessTimerBeforeUpdatingDamagedFillBar()
        {
            if (!_canUpdateDamagedFillBar) { return; }

            _damageUpdateCurrentTimer -= Time.deltaTime;

            if (_damageUpdateCurrentTimer <= 0)
            {
                _damageUpdateCurrentTimer = 0;
                SetDamagedFillBarOvertime(_currentValue, _maxValue);
            }
        }

        protected virtual void SetDamagedFillBarOvertime(float current, float max)
        {
            float nextFillValue = current / max;

            _damagedFillImage.fillAmount = 
                Mathf.Lerp(_damagedFillImage.fillAmount, nextFillValue, Time.deltaTime * _damagedFillBarUpdateSpeed);

            if (_damagedFillImage.fillAmount != nextFillValue) { return; }

            _canUpdateDamagedFillBar = false;
        }

        protected virtual void SetDamagedFillBarImmediatly(float current, float max)
        {
            _damagedFillImage.fillAmount = current / max;
        }
    }
}