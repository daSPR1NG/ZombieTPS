using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public class WeaponSystem : MonoBehaviour
    {
        [Header("CAMERAS")]
        [SerializeField] private CinemachineVirtualCamera _followCamera;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        private int _followCameraPriority;
        private int _aimCameraPriority;

        [Header("DEPENDENCIES")]
        [SerializeField] private Transform weaponPivot;
        [SerializeField] private float _offset = 5f;
        [SerializeField] private int _weaponLimit = 3;
        [SerializeField] private List<Weapon> _weapons = new();
        private WeaponHelper _weaponHelper = null;
        private Weapon _equippedWeapon = null;
        private int _currentWeaponIndex = 0;

        [Header("AFFECTED LAYERS")]
        [SerializeField] private LayerMask _shootables;
        [SerializeField] private LayerMask _interactables;

        [Header("SETTINGS")]
        [Range(0.1f, 10)][SerializeField] private float _aimAssist = 2f;
        [SerializeField] private bool _reloadingCanBeCanceled = false;
        [SerializeField] private bool _autoReload = false;
        private bool _isBursting = false;
        private int _burstRoundLeft = 0;
        private int _currentAmmo = 0;

        [Header("RANDOM SHOOTING SETTINGS")]
        [SerializeField] private Vector3 _minRandomDirection;
        [SerializeField] private Vector3 _maxRandomDirection;

        [Header("RIG SETTINGS")]
        [Range(10, 100)][SerializeField] private float aimingRigTransitionDuration = 1.25f;
        
        [Header("DEBUG")]
        [SerializeField] private GameObject _debugDecalPf;

        #region Inputs
        private PlayerInput _playerInput;
        private InputAction _shoot;
        private InputAction _aim;
        private InputAction _reload;
        private InputAction _swapWeapon;
        #endregion

        private bool _isAiming = false;

        private bool _canShoot = true;
        private float _shootingCD = 0;

        private bool _isReloading = false;
        private float _reloadTimer = 0;

        private int _animationIDRecoilApplicationSpeed;

        private GameObject _muzzleFlashInstance = null;

        private ThirdPersonController _thirdPersonController; 
        private StatsManager _statsManager; 
        private CinemachineImpulseSource _impulseSource;

        private RigBuilderHelper _rigBuilderHelper;
        private Animator _rigAnimator;

        #region Public References
        public bool IsAiming { get => _isAiming; private set => _isAiming = value; }
        public bool IsReloading { get => _isReloading; private set => _isReloading = value; }
        public bool ReloadingCanBeCanceled { get => _reloadingCanBeCanceled; }
        public List<Weapon> Weapons { get => _weapons; }
        public Weapon EquippedWeapon { get => _equippedWeapon; private set => _equippedWeapon = value; }
        #endregion

        #region Enable / Disable
        private void OnEnable()
        {
            _aim.performed += context => Aim();
            _aim.canceled += context => CancelAim(true);

            _reload.performed += context => Reload();
            _swapWeapon.performed += context => SwapWeapon();

            _shoot.canceled += context => HandleNonAutoWeaponOnInputCanceled();
        }

        private void OnDisable()
        {
            _aim.performed -= context => Aim();
            _aim.canceled -= context => CancelAim(true);

            _reload.performed -= context => Reload();
            _swapWeapon.performed -= context => SwapWeapon();

            _shoot.canceled -= context => HandleNonAutoWeaponOnInputCanceled();
        }
        #endregion

        private void Awake()
        {
            #region Input storage
            _playerInput = GetComponent<PlayerInput>();
            _shoot = _playerInput.actions["Shoot"];
            _aim = _playerInput.actions["Aim"];
            _reload = _playerInput.actions["Reload"];
            _swapWeapon = _playerInput.actions["Swap Weapon"];
            #endregion

            InitialiazeWeapons();
        }

        void Start() => Init();

        void Update()
        {
            if (!GameManager.Instance.PlayerCanUseActions()) { return; }

            Shoot();

            ProcessShootingCooldown();
            ProcessReloading();

            HandleAimingRigWeight();
            HandleShootingWithoutAimingRigWeight();
        }

        private void LateUpdate() => HandleAimSensitivityAndFeedbackAtRuntime();

        #region Initialization
        void Init()
        {
            _statsManager = GetComponent<StatsManager>();
            _thirdPersonController = GetComponent<ThirdPersonController>();

            _impulseSource = GetComponent<CinemachineImpulseSource>();

            _followCameraPriority = _followCamera.Priority;
            _aimCameraPriority = _aimCamera.Priority;

            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_Aim, 0);

            AssignAnimationIDs();
            SetAnimationRecoilSpeed();
        }
        #region Summary
        /// <summary>
        /// This is call on Awake, once at the beginning of the game.
        /// </summary>
        #endregion
        private void InitialiazeWeapons()
        {
            // Get rig builder infos.
            _rigBuilderHelper = GetComponentInChildren<RigBuilderHelper>();
            _rigAnimator = _rigBuilderHelper.GetAnimator();

            // For each weapons the player has, init them and set their current ammo.
            for (int i = 0; i < _weapons.Count; i++)
            {
                _weapons[i].Init();
                _weapons[i].SetCurrentAmmo(_weapons[i].GetMaxMagAmmo());
            }

            // On initialization only equip the first weapon in the list by default...
            // ... Current equipped weapon is set.
            _currentWeaponIndex = 0;
            EquipWeapon(Weapons[_currentWeaponIndex]);

            // Set the fire rate, reloading time & aim speed.
            _shootingCD = EquippedWeapon.GetFireRate();
            _reloadTimer = EquippedWeapon.GetReloadingTimer();
            SetAimCameraTransitionSpeed(EquippedWeapon.GetAimSpeed());

            Actions.OnInitializingWeapon?.Invoke(EquippedWeapon);
        }

        private void AssignAnimationIDs()
        {
            _animationIDRecoilApplicationSpeed = Animator.StringToHash("RecoilApplicationSpeed");
        }
        #endregion

        #region Shoot
        private void Shoot()
        {
            // Notes : The pause state is already handled directly in Update.

            if (!_canShoot || IsReloading || !_shoot.IsPressed() || _thirdPersonController.IsRolling()) { return; }
                
            // Auto reload is enabled + no ammo left in magazine
            if (_autoReload && EquippedWeapon.GetCurrentAmmo() <= 0)
            {
                PlayMagEmptySound();
                Reload();
                return;
            }

            _shootingCD = EquippedWeapon.GetFireRate();

            Debug.Log("Shoot");

            // Set bursting infos
            _isBursting = EquippedWeapon.GetAmmoFiredPerShot() > 1;
            _burstRoundLeft = EquippedWeapon.GetAmmoFiredPerShot() - 1;

            // Set weapon local current ammo value
            _currentAmmo = EquippedWeapon.GetCurrentAmmo();

            FireBullet();

            _canShoot = false;
        }

        private void FireBullet()
        {
            _currentAmmo--;

            // Play the recoil animation based on if the character is aiming or not.
            if (IsAiming) { _rigAnimator.Play("Character_Shoot_Riffle_Aiming", 0); }
            else { _rigAnimator.Play("Character_Shoot_Riffle_NotAiming", 0); }

            Transform shotPointOrigin = _weaponHelper.ShotPoint;

            Ray ray = Helper.GetMainCamera().ViewportPointToRay(Vector3.one * .5f);

            // Random shoot direction application
            Vector3 direction = IsAiming ? ray.direction : GetShootingDirection(shotPointOrigin);

            // Hitting shootables - raycast hit
            if (Physics.Raycast(ray.origin, direction, out RaycastHit hitInfo, float.MaxValue))
            {
                if (Physics.Linecast(shotPointOrigin.localPosition, ray.direction, _shootables))
                {
                    Debug.Log("Shot on " + hitInfo.transform.name);

                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

                    _thirdPersonController.RotateCharacterTowardsTargetRotation(transform, Quaternion.Euler(0, _thirdPersonController.CinemachineTargetYaw, 0));

                    ApplyDamageOnValidTarget(hitInfo.transform, hitInfo);

                    Instantiate(_debugDecalPf, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    CreateBulletTrail(hitInfo.point);
                }
            }
            else
            {
                CreateBulletTrail(shotPointOrigin.forward * 50);
            }

            // Effects bloc - visual + sound ----------------------------------------------
            _impulseSource.GenerateImpulseWithForce(EquippedWeapon.GetImpulseForce());

            CreateMuzzleFlashFX(EquippedWeapon);
            CreateFiredBullet();

            if (EquippedWeapon.GetCurrentAmmo() > 0) { PlayFireSound(_weaponHelper.AudioSource); }
            // ----------------------------------------------------------------------------

            EquippedWeapon.SetCurrentAmmo(_currentAmmo);
            Actions.OnShooting?.Invoke(EquippedWeapon);

            //if (_autoReload && EquippedWeapon.GetCurrentAmmo() <= 0) 
            //{
            //    PlayMagEmptySound();
            //    Reload(); 
            //}

            HandleBurst();
        }

        private void ApplyDamageOnValidTarget(Transform target, RaycastHit hitInfo)
        {
            float damageToApply = 0;

            TPSCollider tpsColliderFound = hitInfo.collider.GetComponent<TPSCollider>();
            //Debug.Log("Body part touched : " + tpsColliderFound.ColliderType.ToString());

            if (tpsColliderFound)
            {
                // Decal
                tpsColliderFound.InstantiateHitEffect(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                // Play Hit Sound
                tpsColliderFound.PlayOnHitSound();

                // Update damage done weither if the shot hit the head or any other bodypart.
                damageToApply = 
                    tpsColliderFound.ColliderType == ColliderType.Head ? (EquippedWeapon.GetDamage() * 2) : EquippedWeapon.GetDamage();
            }

            StatsManager targetStats = target.GetComponent<StatsManager>();

            if (targetStats && !targetStats.IsCharacterDead())
            {
                targetStats.ApplyDamageToTarget(transform, target, damageToApply);

                ScoreGiver scoreGiver = target.GetComponent<ScoreGiver>();
                scoreGiver.GiveScoreToTarget(transform, scoreGiver.GetScoreData(ScoreRelatedActionName.OnHit));

                Actions.OnHittingValidTarget?.Invoke();
            }
        }

        private Vector3 GetShootingDirection(Transform refPoint)
        {
            Vector3 direction = refPoint.forward;

            direction += new Vector3(
                Random.Range(-_minRandomDirection.x, _maxRandomDirection.x),
                Random.Range(-_minRandomDirection.y, _maxRandomDirection.y),
                Random.Range(-_minRandomDirection.z, _maxRandomDirection.z));

            direction.Normalize();

            return direction;
        }

        private void CreateFiredBullet()
        {
            if (!EquippedWeapon.GetBulletPf())
            {
                Debug.LogError("No fire bullet prefab reference.");
                return;
            }

            GameObject firedBullet = Instantiate(
                EquippedWeapon.GetBulletPf(), 
                _weaponHelper.BulletExitPoint.position, 
                EquippedWeapon.GetBulletPf().transform.rotation);
        }

        private void CreateBulletTrail(Vector3 destination)
        {
            if (!EquippedWeapon.GetBulletTrailPf())
            {
                Debug.LogError("No bullet trail prefab reference.");
                return;
            }

            GameObject bulletTrailInstance = Instantiate(EquippedWeapon.GetBulletTrailPf(), _weaponHelper.ShotPoint.position, _weaponHelper.ShotPoint.localRotation);

            BulletTrail bulletTrail = bulletTrailInstance.GetComponent<BulletTrail>();
            bulletTrail.Setup(_weaponHelper.ShotPoint.position, destination);
            Debug.Log("Setup bullet trail");
        }

        private void CreateMuzzleFlashFX(Weapon weapon)
        {
            // No muzzle flash on the equipped weapon
            if (!weapon.GetMuzzleFlash()) 
            {
                Debug.LogError("No muzzle flash prefab reference.");
                return; 
            }

            // Toggle set active value
            if (_muzzleFlashInstance) { _muzzleFlashInstance.SetActive(false); }

            // If the muzzle flash FX is not instantiated then do it
            if (_weaponHelper.ShotPoint.childCount <= 0) 
            {
                _muzzleFlashInstance = Instantiate(weapon.GetMuzzleFlash(), _weaponHelper.ShotPoint); ;
            }

            // Toggle set active value
            if (_muzzleFlashInstance) { _muzzleFlashInstance.SetActive(true); }
        }

        private void HandleBurst()
        {
            // If its bursting re shoot
            if (!_isBursting) { return; }

            Invoke(nameof(FireBullet), EquippedWeapon.GetFireRate());
            _burstRoundLeft--;

            // CANCEL BURST
            if (_burstRoundLeft <= 0)
            {
                _burstRoundLeft = 0;
                _isBursting = false;
            }
        }

        private void ProcessShootingCooldown()
        {
            if (!GameManager.Instance.PlayerCanUseActions() || _canShoot || IsReloading || !IsWeaponTypeAutomatic()) { return; }

            Debug.Log("Cannot shoot for the moment !");

            _shootingCD -= Time.deltaTime;

            if (_shootingCD <= 0)
            {
                _shootingCD = 0;
                _canShoot = true;
            }
        }

        private void HandleNonAutoWeaponOnInputCanceled()
        {
            if (IsWeaponTypeAutomatic()) { return; }

            _canShoot = true;
        }

        private void PlayFireSound(AudioSource audioSource)
        {
            if (!audioSource) { return; }

            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                    EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, 
                    RelatedWeaponAction.Shoot);

            float ammoPercentage = (float)EquippedWeapon.GetCurrentAmmo() / (float)EquippedWeapon.GetMaxMagAmmo();

            // Randomize pitch...
            float pitchOverride = Random.Range(weaponAudioSetting.GetPitchMinValue(), weaponAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(audioSource, pitchOverride);

            // ... Randomize volume...
            float randomVolume = Mathf.Clamp(ammoPercentage, weaponAudioSetting.GetVolumeMinValue(), weaponAudioSetting.GetVolumeMaxValue());

            // ... And then play the sound.
            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), randomVolume);
        }
        #endregion

        #region Aiming - Aim / Cancel
        public void Aim()
        {
            bool canAim = GameManager.Instance.PlayerCanUseActions() && !_thirdPersonController.IsRolling();

            if (!canAim) { return; }

            Debug.Log("Aim");

            IsAiming = true;
            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 0);

            _thirdPersonController.SetSensitivity(_thirdPersonController.AimSensitivity);

            _followCamera.Priority = 9;
            _aimCamera.Priority = 12;

            _statsManager.GetStatByType(StatType.MovementSpeed).CurrentValue = _statsManager.GetStatByType(StatType.MovementSpeed).MaxValue / 2.5f;

            PlayAimSound();

            Actions.OnAiming?.Invoke();
        }

        public void CancelAim(bool playSFX = false)
        {
            if (!GameManager.Instance.PlayerCanUseActions()) { return; }

            Debug.Log("Cancel aiming");

            if (IsAiming && playSFX) { PlayUnAimSound(); }
            IsAiming = false;

            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_Aim, 0);
            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 1);

            if (!IsReloading) { _thirdPersonController.SetSensitivity(_thirdPersonController.LookingSensitivity); }
            
            _followCamera.Priority = _followCameraPriority;
            _aimCamera.Priority = _aimCameraPriority;

            Actions.OnCancelingAim?.Invoke();
        }

        private void HandleAimingRigWeight()
        {
            if (!GameManager.Instance.PlayerCanUseActions() || IsReloading) { return; }

            float evolvingValue = Mathf.Lerp(
                    _rigBuilderHelper.GetRigData(RigBodyPart.HeldObject_Shoot_Aim).GetRigWeight(),
                    1, Time.deltaTime * aimingRigTransitionDuration);

            if (IsAiming) 
            {
                _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_Aim, evolvingValue);
                return; 
            }

            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_Aim, 0);
        }

        private void HandleShootingWithoutAimingRigWeight()
        {
            if (!GameManager.Instance.PlayerCanUseActions() || IsReloading || _thirdPersonController.IsRolling()) { return; }

            if (_shoot.IsPressed() && !IsAiming)
            {
                _thirdPersonController.RotateCharacterTowardsTargetRotation(transform, Quaternion.Euler(0, _thirdPersonController.CinemachineTargetYaw, 0));

                _rigBuilderHelper.GetRigData(RigBodyPart.HeldObject_Shoot_NoAim).GetMultiAimConstraint().weight = 1;
                _rigBuilderHelper.GetRigData(RigBodyPart.Head).GetMultiAimConstraint().weight = .5f;

                return;
            }

            _rigBuilderHelper.GetRigData(RigBodyPart.HeldObject_Shoot_NoAim).GetMultiAimConstraint().weight = 0;
            _rigBuilderHelper.GetRigData(RigBodyPart.Head).GetMultiAimConstraint().weight = 0;
        }

        private void HandleAimSensitivityAndFeedbackAtRuntime()
        {
            if (!GameManager.Instance.PlayerCanUseActions() || IsReloading) { return; }

            if (IsAiming) 
            {
                _thirdPersonController.RotateCharacterTowardsTargetRotation(transform, Quaternion.Euler(0, _thirdPersonController.CinemachineTargetYaw, 0));
            }

            if (Physics.Raycast(
              Helper.GetMainCamera().transform.position, Helper.GetMainCamera().transform.forward, out RaycastHit hit, float.MaxValue))
            {
                //Debug.Log(hit.transform.name);

                GlobalCharacterParameters globalCharacterParameters = hit.transform.GetComponent<GlobalCharacterParameters>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactive element"))
                {
                    Actions.OnAimingInteractable?.Invoke(globalCharacterParameters.CharacterType);

                    float aimAssist = globalCharacterParameters.CharacterType == CharacterType.IA_Enemy ?
                        _aimAssist : 1;

                    // Reduce the sensitivity when we aim a valid target
                    _thirdPersonController.SetSensitivity(_thirdPersonController.AimSensitivity / aimAssist);

                    //Debug.Log("Hitting a valid target.");
                }
                // We call the event here to make sure that if we are still hitting something and the layer is incorect
                else  { ResetAimingRaycastToDefault(); }

                return;
            }

            // We're hitting nothing, call this event
            ResetAimingRaycastToDefault();
        }

        private void ResetAimingRaycastToDefault()
        {
            Actions.OnAimingInteractable?.Invoke(CharacterType.Unassigned);

            ResetSentisitivityToCorrectValue();
        }

        private void ResetSentisitivityToCorrectValue()
        {
            // Reset to the correct sensitivity value based on weither we are aiming or not.
            switch (IsAiming)
            {
                case true:
                    _thirdPersonController.SetSensitivity(_thirdPersonController.AimSensitivity);
                    break;
                case false:
                    _thirdPersonController.SetSensitivity(_thirdPersonController.LookingSensitivity);
                    break;
            }
        }

        private void PlayAimSound()
        {
            if (!_weaponHelper.AudioSource) { return; }

            Debug.Log("Play aim sound");

            AudioSource audioSource = _weaponHelper.AudioSource;
            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                   EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings,
                   RelatedWeaponAction.Aim);

            // Set pitch...
            AudioHelper.SetPitch(audioSource, weaponAudioSetting.GetPitchMaxValue());

            // Set volume
            AudioHelper.SetVolume(audioSource, weaponAudioSetting.GetVolumeMaxValue());

            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
        }

        private void PlayUnAimSound()
        {
            if (!_weaponHelper.AudioSource) { return; }

            Debug.Log("Play un-aim sound");

            AudioSource audioSource = _weaponHelper.AudioSource;
            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                    EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings,
                    RelatedWeaponAction.UnAim);

            // Set pitch...
            AudioHelper.SetPitch(audioSource, weaponAudioSetting.GetPitchMaxValue());

            // Set volume
            AudioHelper.SetVolume(audioSource, weaponAudioSetting.GetVolumeMaxValue());

            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
        }
        #endregion

        #region Reloading
        private void Reload()
        {
            bool canReload = GameManager.Instance.PlayerCanUseActions() || !_thirdPersonController.IsRolling();

            if (!canReload) { return; }

            if (EquippedWeapon.GetMaxAmmo() == 0) { PlayNoAmmoLeftSound(); }

            // If the player is already reloading then return.  
            if (IsReloading) { return; }

            //Ammo amount is the same as the max amount currently stored in the mag so there is no need to reload
            if (EquippedWeapon.GetCurrentAmmo() == EquippedWeapon.GetMaxMagAmmo()) { return; }

            //No max ammo left > no ammo at all so we can't reload for now

            Actions.OnReloadStarted?.Invoke();

            Debug.Log("Reload");

            // Reset the sensitivy. - notes : it is commented, because we use a method
            // where the sensitivity is reseted overtime in Processreloading() below.
            //_thirdPersonController.SetSensitivity(_thirdPersonController.LookingSensitivity);

            // Hide the muzzle flash.
            _muzzleFlashInstance.SetActive(false);

            //// Reset aiming state - we don't want to be aiming when we want to reload.
            //if (IsAiming){ CancelAim(); }
            //_rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 0);

            IsReloading = true;
            SetReloadingStateRigIKs(IsReloading);

            // Reset aiming state - we don't want to be aiming when we want to reload.
            //if (IsAiming) { CancelAim(); }
            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 0);

            _reloadTimer = EquippedWeapon.GetReloadingTimer();

            PlayReloadingAnimation(EquippedWeapon.GetReloadingTimer());
           
            Actions.OnReloadStartedSetWeaponData?.Invoke(EquippedWeapon);
        }

        // NOTES : Add a threshold in which the player still manage to reload event by canceling the reload animation...
        // ... The UI Gauge Image color resets to white.
        public void CancelReloading()
        {
            // Reset shoot sound pitch value if it has been decreased before.
            AudioHelper.SetPitch(
                _weaponHelper.AudioSource,
                WeaponAudioSetting.GetWeaponAudioSetting(
                    EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, 
                    RelatedWeaponAction.Shoot).GetPitchMaxValue());

            IsReloading = false;
            SetReloadingStateRigIKs(IsReloading);

            // Reset the sensitivy.
            _thirdPersonController.SetSensitivity(_thirdPersonController.LookingSensitivity);

            Actions.OnReloadEndedSetWeaponData?.Invoke(EquippedWeapon);
            Actions.OnReloadEnded?.Invoke();

            // Reset to the correct rig IK weither if the player is aiming or not.
            if (_aim.IsPressed())
            {
                Aim();
                return;
            }

            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 1);
        }

        // NOTES : When reaching a certain threshold the UI Gauge representing the timer...
        // ... changes its color to something else meaning that the reload can be canceled while being valid. 
        private void ProcessReloading()
        {
            if (!IsReloading) { return; }

            _reloadTimer -= Time.deltaTime;

            // Reset the sensitivy overtime.
            _thirdPersonController.SetSensitivityOvertime(_thirdPersonController.LookingSensitivity, 1);

            if (_reloadTimer <= 0)
            {
                _reloadTimer = 0;

                EquippedWeapon.SetCurrentAmmo(EquippedWeapon.GetMaxMagAmmo());
                _canShoot = true;

                CancelReloading();
            }
        }

        private void PlayReloadingAnimation(float reloadSpeed)
        {
            //AnimatorHelper.DebugAnimationsDuration(_rigAnimator);

            // AnimatorHelper.GetAnimationLength(_rigAnimator, 0) == 1.25f
            float reloadAnimationSpeed = AnimatorHelper.GetAnimationLength(_rigAnimator, 0) + (1 / reloadSpeed);
            reloadAnimationSpeed /= reloadSpeed;
            reloadAnimationSpeed /= AnimatorHelper.GetAnimationLength(_rigAnimator, 0);
            reloadAnimationSpeed = Mathf.Round(reloadAnimationSpeed * 10.00f) * 0.1f;

            //_rigAnimator.speed = reloadAnimationSpeed;
            AnimatorHelper.SetAnimatorFloat(_rigAnimator, "ReloadingSpeed", reloadAnimationSpeed);

            _rigAnimator.Play("Character_Reloading_Rifle", 0);
        }

        private void PlayNoAmmoLeftSound()
        {
            if (!_weaponHelper.AudioSource) { return; }

            Debug.Log("Play no ammo left sound");

            AudioSource audioSource = _weaponHelper.AudioSource;
            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                   EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings,
                   RelatedWeaponAction.NoAmmoLeft);

            // Set pitch...
            AudioHelper.SetPitch(audioSource, weaponAudioSetting.GetPitchMaxValue());

            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
        }

        private void PlayMagEmptySound()
        {
            if (!_weaponHelper.AudioSource) { return; }

            Debug.Log("Play mag empty sound");

            AudioSource audioSource = _weaponHelper.AudioSource;
            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                   EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings,
                   RelatedWeaponAction.MagEmpty);

            // Set pitch...
            AudioHelper.SetPitch(audioSource, weaponAudioSetting.GetPitchMaxValue());

            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
        }

        private void SetReloadingStateRigIKs(bool isReloading)
        {
            if (isReloading)
            {
                _rigBuilderHelper.EnableThisRigLayer(0);
                _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand).GetTwoBoneIKConstraint().weight = 0;
                _rigBuilderHelper.EnableThisRigLayer(0, _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand_Reload).GetRig());
                _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand_Reload).GetTwoBoneIKConstraint().CreateJob(_rigBuilderHelper.GetAnimator());

                _rigBuilderHelper.DisablePrincipalRigLayers(false);

                return;
            }

            _rigBuilderHelper.DisableThisLayer(0);
            _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand).GetTwoBoneIKConstraint().weight = 1;
            _rigBuilderHelper.DisableThisLayer(0, _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand_Reload).GetRig());
            _rigBuilderHelper.EnablePrincipalRigLayers();
        }
        #endregion

        #region Equip / Swap
        private void EquipWeapon(Weapon weapon)
        {
            EquippedWeapon = weapon;
            EquippedWeapon.Init();

            SetAimCameraTransitionSpeed(EquippedWeapon.GetAimSpeed());
            SetAnimationRecoilSpeed();

            if (EquippedWeapon.GetPrefab() && weaponPivot.transform.childCount <= 0)
            {
                GameObject weaponInstance = Instantiate(EquippedWeapon.GetPrefab(), weaponPivot);

                // Init holding & reloading IKs target and hint.
                _weaponHelper = weaponInstance.GetComponent<WeaponHelper>();
                _weaponHelper.InitHoldingIK(
                    _rigBuilderHelper.GetRigData(RigBodyPart.R_Hand).GetTwoBoneIKConstraint(),
                    _rigBuilderHelper.GetRigData(RigBodyPart.L_Hand).GetTwoBoneIKConstraint());

                _weaponHelper.InitReloadingIK(_rigBuilderHelper.GetRigData(RigBodyPart.L_Hand_Reload).GetTwoBoneIKConstraint());
            }

            PlaySwapWeaponSound();
            Actions.OnEquippingWeapon?.Invoke(weapon, _currentWeaponIndex);

            if (_aim.IsPressed()) { Aim(); }
        }

        private void SwapWeapon()
        {
            if (!GameManager.Instance.PlayerCanUseActions() || Weapons.Count <= 1) { return; }

            Debug.Log("Swap weapon");

            // In this bloc we cancel all the weapon actions -----------------------------
            if (IsAiming) { CancelAim(); }
            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_Aim, 0);
            _rigBuilderHelper.SetRigWeight(RigBodyPart.HeldObject_Shoot_NoAim, 0);
            // ----------------------------------------------------------------------------

            if (ReloadingCanBeCanceled) { CancelReloading(); }

            _currentWeaponIndex++;

            if (_currentWeaponIndex >= _weaponLimit) { _currentWeaponIndex = 0; }

            EquipWeapon(Weapons[_currentWeaponIndex]);
        }

        private void PlaySwapWeaponSound()
        {
            if (!_weaponHelper.AudioSource) { return; }

            AudioSource audioSource = _weaponHelper.AudioSource;
            WeaponAudioSetting weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(
                EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, 
                RelatedWeaponAction.Swap);

            // Set pitch...
            AudioHelper.SetPitch(audioSource, weaponAudioSetting.GetPitchMaxValue());

            AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
        }
        #endregion

        private bool IsWeaponTypeAutomatic()
        {
            return EquippedWeapon.GetWeaponType() == WeaponType.Automatic;
        }

        public void SetAimCameraTransitionSpeed(float value)
        {
            CinemachineBrain cinemachineBrain = Helper.GetMainCamera().GetComponent<CinemachineBrain>();
            cinemachineBrain.m_DefaultBlend.m_Time = value;
        }

        private void SetAnimationRecoilSpeed()
        {
            _rigAnimator.SetFloat(_animationIDRecoilApplicationSpeed, EquippedWeapon.GetRecoilForce());
        }

        public bool IsAimInputPressed() { return _aim.IsPressed(); }
    }
}