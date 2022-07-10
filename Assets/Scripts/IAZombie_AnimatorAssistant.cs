using UnityEngine;

namespace Khynan_Coding
{
    public class IAZombie_AnimatorAssistant : MonoBehaviour
    {
        public GameObject GreenFireVFX;
        public GameObject ProjectilePf;
        public Transform ProjectileSpawnPoint;

        private DefaultController _controller;
        private ControllerAudioSettingList _controllerAudioSettingList;

        private Animator _animator;
        private AudioSource _audioSource;
        private CombatSystem _combatSystem;
        private IAInteractionHandler _IAInteractionHandler;

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
            _IAInteractionHandler = GetComponentInParent<IAInteractionHandler>();
        }

        public void SetAttackBooleanToFalse()
        {
            AnimatorHelper.SetAnimatorBoolean(_animator, "Attack", false);
            AnimatorHelper.SetAnimatorBoolean(_animator, "RangedAttack", false);
        }

        public void ApplyDamageInAnimation()
        {
            _combatSystem.ApplyDamageToTargetWhileInCombat();
        }

        public void StopNavMeshAgent()
        {
            if ( !_controller.NavMeshAgent.hasPath ) { return; }

            _controller.NavMeshAgent.isStopped = true;
        }

        public void ResumeNavMeshAgent()
        {
            if ( !_controller.NavMeshAgent.hasPath ) { return; }

            _controller.NavMeshAgent.isStopped = false;
        }

        public void DisplayGreenFireVFX()
        {
            Helper.DisplayGO( GreenFireVFX );
        }

        public void HideGreenFireVFX()
        {
            Helper.HideGO( GreenFireVFX );
        }

        public void SpawnProjectile()
        {
            if ( _IAInteractionHandler._isTargetTooFar ) 
            {
                HideGreenFireVFX();
                return; 
            }

            GameObject projectileInstance = Instantiate(
                ProjectilePf,
                ProjectileSpawnPoint.position,
                ProjectilePf.transform.rotation );

            // Set Projectile Damage
            StatsManager statsManager = _controller.GetComponent<StatsManager>();
            ProjectileBehaviour projectileBehaviour = projectileInstance.GetComponent<ProjectileBehaviour>();

            projectileBehaviour.Target = _controller.GetComponent<IAInteractionHandler>().CurrentTarget;
            projectileBehaviour.Damage = statsManager.GetStat( StatAttribute.AttackDamage ).GetCurrentValue();
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