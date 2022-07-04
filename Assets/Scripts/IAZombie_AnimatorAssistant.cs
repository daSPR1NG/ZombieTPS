using UnityEngine;

namespace Khynan_Coding
{
    public class IAZombie_AnimatorAssistant : MonoBehaviour
    {
        private DefaultController _controller;
        private ControllerAudioSettingList _controllerAudioSettingList;

        private Animator _animator;
        private AudioSource _audioSource;
        private CombatSystem _combatSystem;

        #region Public References

        #endregion

        void Awake() => Init();

        void Init()
        {
            _controller = GetComponentInParent<DefaultController>();
            _controllerAudioSettingList = _controller.CharacterStats.GetControllerAudioSetting();

            _animator = GetComponent<Animator>();
            _audioSource = GetComponentInParent<AudioSource>();
            _combatSystem = GetComponentInParent<CombatSystem>();
        }

        public void SetAttackBooleanToFalse()
        {
            AnimatorHelper.SetAnimatorBoolean(_animator, "Attack", false);
        }

        public void ApplyDamageInAnimation()
        {
            _combatSystem.ApplyDamageToTargetWhileInCombat();
        }

        #region SFX
        public void PlayIdleSFX()
        {
            ControllerAudioSetting controllerAudioSetting =
                ControllerAudioSetting.GetControllerAudioSetting(_controllerAudioSettingList.ControllerAudioSettings, RelatedControllerAction.OnIdle);

            float randomPitch = Random.Range(controllerAudioSetting.GetPitchMinValue(), controllerAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(_audioSource, randomPitch);

            //Debug.Log("PlayIdleSFX | " + randomPitch);

            AudioHelper.Stop(_audioSource);

            AudioHelper.PlayOneShot(
                _audioSource,
                controllerAudioSetting.GetAudioClip(),
                controllerAudioSetting.GetVolumeMaxValue());
        }

        public void PlayAttackSFX()
        {
            ControllerAudioSetting controllerAudioSetting =
                ControllerAudioSetting.GetControllerAudioSetting(_controllerAudioSettingList.ControllerAudioSettings, RelatedControllerAction.OnAttack);

            float randomPitch = Random.Range(controllerAudioSetting.GetPitchMinValue(), controllerAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(_audioSource, randomPitch);

            //Debug.Log("PlayAttackSFX | " + randomPitch);

            AudioHelper.Stop(_audioSource);

            AudioHelper.PlayOneShot(
                _audioSource, 
                controllerAudioSetting.GetAudioClip(), 
                controllerAudioSetting.GetVolumeMaxValue());
        }

        public void PlayWalkingSFX()
        {
            ControllerAudioSetting controllerAudioSetting =
                ControllerAudioSetting.GetControllerAudioSetting(_controllerAudioSettingList.ControllerAudioSettings, RelatedControllerAction.OnWalking);

            float randomPitch = Random.Range(controllerAudioSetting.GetPitchMinValue(), controllerAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(_audioSource, randomPitch);

            //Debug.Log("PlayWalkingSFX | " + randomPitch);

            AudioHelper.Stop(_audioSource);

            AudioHelper.PlayOneShot(
                _audioSource,
                controllerAudioSetting.GetAudioClip(),
                controllerAudioSetting.GetVolumeMaxValue());
        }

        public void PlayRunningSFX()
        {
            ControllerAudioSetting controllerAudioSetting =
                ControllerAudioSetting.GetControllerAudioSetting(_controllerAudioSettingList.ControllerAudioSettings, RelatedControllerAction.OnRunning);

            float randomPitch = Random.Range(controllerAudioSetting.GetPitchMinValue(), controllerAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(_audioSource, randomPitch);

            //Debug.Log("PlayRunningSFX | " + randomPitch);

            AudioHelper.Stop(_audioSource);

            AudioHelper.PlayOneShot(
                _audioSource,
                controllerAudioSetting.GetAudioClip(),
                controllerAudioSetting.GetVolumeMaxValue());
        }
        #endregion
    }
}