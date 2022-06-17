using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class UIKillChain : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _killCountText;
        [SerializeField] private Image _fillImage;

        private bool _doesTimerNeedsToBeUpdated = false;

        private float _totalDuration;
        private float _currentTimer;

        private GameObject _content;
        private Animator _animator;

        private void OnEnable()
        {
            Actions.OnKillChainStarted += InitTimer;
            Actions.OnKillCountValueChanged += SetTimer;
        }

        private void OnDisable()
        {
            Actions.OnKillChainStarted -= InitTimer;
            Actions.OnKillCountValueChanged -= SetTimer;
        }

        void Start() => Init();

        void Update() => ProcessTimer();

        void Init()
        {
            _content = transform.GetChild(0).gameObject;
            if (_content.activeInHierarchy) { _content.SetActive(false); }
            
            _animator = GetComponent<Animator>();
        }

        private void SetKillCountText(int value)
        {
            _killCountText.SetText(value.ToString());
            _animator.Play("KillChain_Count_BreathEffect");
        }

        private void SetTimerImageFillAmount(float current, float max)
        {
            _fillImage.fillAmount = current / max;
        }

        #region Timer - value, text, processing
        private void InitTimer(float timer)
        {
            _totalDuration = timer;

            _currentTimer = timer;
            SetTimerText(timer);

            SetKillCountText(1);
            SetTimerImageFillAmount(_currentTimer, _totalDuration);

            DisplayContent();
            _doesTimerNeedsToBeUpdated = true;
        }

        private void SetTimer(int killCount, float timer)
        {
            _currentTimer = timer;
            SetTimerText(timer);

            SetTimerImageFillAmount(_currentTimer, _totalDuration);

            SetKillCountText(killCount);
        }

        private void SetTimerText(float timer)
        {
            //_timerText.SetText(Helper.GetStringOfValueInMinutesAndSeconds(timer));
            _timerText.SetText(timer.ToString("0.0") + " s");
        }

        private void ProcessTimer()
        {
            if (!_doesTimerNeedsToBeUpdated) { return; }

            _currentTimer -= Time.deltaTime;

            SetTimerText(_currentTimer);
            SetTimerImageFillAmount(_currentTimer, _totalDuration);

            if (_currentTimer <= 0)
            {
                _currentTimer = 0;
                _doesTimerNeedsToBeUpdated = false;

                HideContent();
            }
        }
        #endregion

        #region Content - Display / Hide
        private void DisplayContent()
        {
            _content.SetActive(true);
        }

        private void HideContent()
        {
            _content.SetActive(false);

            SetTimerText(0);
            SetKillCountText(0);
        }
        #endregion
    }
}