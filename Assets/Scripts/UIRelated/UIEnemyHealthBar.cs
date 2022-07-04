using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class UIEnemyHealthBar : UIHealthBar
    {
        [Header("Enemy Infos")]
        [SerializeField] private TMP_Text _characterNameText;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ControllerAudioSetting _audioSetting;

        public void InitEnemyHealthBarFill(/*Transform target,*/ float current, float max)
        {
            _currentValue = current;
            _maxValue = max;

            if (_displaysTextInfos) { _healthValueText.SetText(current.ToString()); }

            _healthFillImage.fillAmount = current / max;
            _damagedFillImage.fillAmount = current / max;
        }

        public override void SetHealthBar(float current, float max, HealthInteraction healthInteraction)
        {
            base.SetHealthBar(current, max, healthInteraction);
        }

        protected override void ProcessTimerBeforeUpdatingDamagedFillBar()
        {
            base.ProcessTimerBeforeUpdatingDamagedFillBar();
        }

        protected override void SetDamagedFillBarImmediatly(float current, float max)
        {
            base.SetDamagedFillBarImmediatly(current, max);
        }

        protected override void SetDamagedFillBarOvertime(float current, float max)
        {
            base.SetDamagedFillBarOvertime(current, max);
        }

        public void PlayDeathEffect()
        {
            Debug.Log("PlayDeathEffect");
            _animator.SetTrigger("DeathEffect");

            AudioHelper.SetPitch(_audioSource, _audioSetting.GetPitchMaxValue());

            AudioHelper.PlaySound(_audioSource, _audioSetting.GetAudioClip(), _audioSetting.GetVolumeMaxValue());
        }
    }
}