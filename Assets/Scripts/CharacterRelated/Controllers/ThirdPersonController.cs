﻿using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Khynan_Coding
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class ThirdPersonController : DefaultController
	{
		[Header("GENERAL SETTINGS")]
		public bool CanJump = false;
		public bool CanRoll = false;

		[Header("MOVEMENT SETTINGS")]
		public bool HoldToSprint = true;
		public bool AnalogMovement = false;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Header("AIM SENSITIVITY SETTINGS")]
		public float LookingSensitivity = 1f;
		public float AimSensitivity = 1f;
		private float _currentSensitivity;

		[Header("CINEMACHINE")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

		[Header("DAMPING SETTINGS")]
		public Vector3 RunDamping = new(0.1f, 0.25f, 0.25f);
		public Vector3 SprintDamping = new(0.1f, 0.25f, 0.65f);
		public Vector3 RollDamping = new(0.1f, 0.25f, 0f);

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		// Speed
		private float _runSpeed;
		private float _sprintSpeed;
		float _targetSpeed;
		bool _sprintInputPerformed;

		// Flags
		private bool _isRolling = false;
		private bool _isJumping = false;

		private Vector2 _inputMovement;
		private float _speed;
		private float _targetRotation = 0.0f;
		private float _verticalVelocity;

		// animation IDs
		private int _animIDInputX;
		private int _animIDInputY;
		private int _animIDMotionSpeed;

		private StatsManager _statsManager;
		private CharacterController _characterController;
		private PlayerInteractionHandler _playerInteractionHandler;
		private GameObject _mainCamera;
		private RigBuilderHelper _rigBuilderHelper;

		private PlayerRoll _playerRoll;

		private PlayerInput _input;
		private InputAction _sprint;

		private readonly float _threshold = 0.03f;

        #region Public references
        public float CinemachineTargetYaw { get => _cinemachineTargetYaw; private set => _cinemachineTargetYaw = value; }
        public float CinemachineTargetPitch { get => _cinemachineTargetPitch; private set => _cinemachineTargetPitch = value; }
        public float VerticalVelocityValue { get => _verticalVelocity; set => _verticalVelocity = value; }
        #endregion

        protected override void Awake()
		{
			base.Awake();

			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = Helper.GetMainCamera().gameObject;
			}

			_input = GetComponent<PlayerInput>();

			_sprint = _input.actions["Sprint"];
		}

        #region OnEnable / OnDisable
        private void OnEnable()
        {
			_sprint.performed += context => HandleSprintInputAction();
		}

        private void OnDisable()
        {
			_sprint.performed -= context => HandleSprintInputAction();
		}
		#endregion

		private void Start() => Init();

		protected override void Update()
		{
			if (!GameManager.Instance.PlayerCanUseActions()) { return; }

			Move();

			base.Update();
		}

		private void LateUpdate() => CameraRotation();

        #region Initialisation
        private void Init()
        {
			_playerRoll = GetComponent<PlayerRoll>();

			_playerInteractionHandler = GetComponent<PlayerInteractionHandler>();
			_statsManager = GetComponent<StatsManager>();
			_characterController = GetComponent<CharacterController>();
			_rigBuilderHelper = GetComponent<GlobalCharacterParameters>().GetRigBuilderHelper();

			AssignAnimationIDs();

			SetTargetSpeedValues(_statsManager.GetStat(StatAttribute.MovementSpeed).GetMaxValue());

			SetDefaultStateAtStart(Idle());

			SetSensitivity(LookingSensitivity);

			NavMeshAgent.enabled = false;
		}

        protected virtual void AssignAnimationIDs()
		{
			_animIDInputX = Animator.StringToHash("InputX");
			_animIDInputY = Animator.StringToHash("InputY");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		}
		#endregion

		#region Movement + speed value
		private void Move()
		{
			if (_isRolling) 
			{ 
				ResetSettingsToRunningState();
				if (GetActiveCamera3rdPersonFollow().Damping != RollDamping) 
				{ 
					GetActiveCamera3rdPersonFollow().Damping = RollDamping; 
				}
				return; 
			}

			// Check input movement value
			_inputMovement = _input.actions["Move"].ReadValue<Vector2>();

			CharacterIsMoving = _inputMovement != Vector2.zero;
			//Debug.Log("Input movement " + _inputMovement);

			SetTargetSpeed();

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_inputMovement == Vector2.zero)
			{
				if (!_playerInteractionHandler.IsInteracting) { SwitchState(Idle()); }
				_statsManager.GetStat(StatAttribute.MovementSpeed).SetCurrentValue(0.0f);
				_targetSpeed = 0.0f;
			}

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = AnalogMovement ? _inputMovement.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < _targetSpeed - speedOffset || currentHorizontalSpeed > _targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				_statsManager.GetStat(StatAttribute.MovementSpeed).SetCurrentValue(
					Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate));

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
				_statsManager.GetStat(StatAttribute.MovementSpeed).SetCurrentValue(Mathf.Round(_speed * 1000f) / 1000f);
			}
			else
			{
				_speed = _targetSpeed;
				_statsManager.GetStat(StatAttribute.MovementSpeed).SetCurrentValue(_targetSpeed);
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_inputMovement.x, 0.0f, _inputMovement.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_inputMovement != Vector2.zero)
			{
				_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				//float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

				//// rotate to face input direction relative to camera position
				//transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

				transform.rotation = Quaternion.Euler(0, CinemachineTargetYaw, 0.0f);

				SwitchState(Moving());
			}

			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// move the player
			_characterController.Move(
			targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, VerticalVelocityValue, 0.0f) * Time.deltaTime);

			// update animator if using character
			if (Animator)
			{
				Animator.SetFloat(_animIDInputX, _inputMovement.x);
				Animator.SetFloat(_animIDInputY, _inputMovement.y);

				float overridenInputMagnitude = _inputMovement == Vector2.zero ? 1 : inputMagnitude;
				Animator.SetFloat(_animIDMotionSpeed, Mathf.Clamp(overridenInputMagnitude, 0.75f, 1));
			}

			//Debug.Log("Target speed " + _targetSpeed);
		}

		private void HandleSprintInputAction()
		{
			//Check if wether we press or perform the input action
			if (HoldToSprint) { return; }

			if (_sprintInputPerformed)
			{
				_sprintInputPerformed = false;
				return;
			}

			_sprintInputPerformed = true;
		}
		
		private void SetTargetSpeed()
        {
			bool inputMovementHasCorrectValueToSprint = _inputMovement.y > 0f && _inputMovement.x >= -.65f && _inputMovement.x <= .65f;
			//Debug.Log("inputMovementHasCorrectValueToSprint : " + inputMovementHasCorrectValueToSprint);

			WeaponSystem weaponSystem = GetComponent<WeaponSystem>();

			if (weaponSystem && weaponSystem.IsAiming 
				&& GetActiveCamera3rdPersonFollow().Damping != RunDamping)
            {
                //Debug.Log("Sprint while rolling " + _isRolling);

                ResetSettingsToRunningState();
                return;
            }

            GetActiveCamera3rdPersonFollow().Damping = _targetSpeed == _sprintSpeed ? SprintDamping : RunDamping;

			// Set target speed based on move speed, sprint speed and if sprint is pressed
			if (HoldToSprint)
			{
				_targetSpeed = _input.actions["Sprint"].IsPressed() && inputMovementHasCorrectValueToSprint ? _sprintSpeed : _runSpeed;
				return;
			}

			_targetSpeed = _sprintInputPerformed ? _sprintSpeed : _runSpeed;
		}

        private void ResetSettingsToRunningState()
        {
            GetActiveCamera3rdPersonFollow().Damping = RunDamping;
            _targetSpeed = _runSpeed;
        }

		public void SetTargetSpeedValues(float value)
        {
			_runSpeed = value / 1.75f;
			_sprintSpeed = value;
		}

        public Vector2 GetInputMovementValue()
        {
			return _inputMovement;
        }

		private Cinemachine3rdPersonFollow GetActiveCamera3rdPersonFollow()
        {
			CinemachineBrain cinemachineBrain = Helper.GetMainCamera().GetComponent<CinemachineBrain>();
			CinemachineVirtualCamera virtualCamera = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

			return virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
		}
		#endregion

		#region Camera + character transform rotations
		private void CameraRotation()
		{
			Vector2 lookMagnitude = _input.actions["Look"].ReadValue<Vector2>();

			// if there is an input and camera position is not fixed
			if (lookMagnitude.sqrMagnitude >= _threshold && !LockCameraPosition)
			{
				CinemachineTargetYaw += lookMagnitude.x * Time.deltaTime * _currentSensitivity;
				CinemachineTargetPitch += lookMagnitude.y * Time.deltaTime * _currentSensitivity;

				if (_inputMovement == Vector2.zero) { transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0.0f); }
			}

			// clamp our rotations so our values are limited 360 degrees
			CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
			CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, BottomClamp, TopClamp);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride, CinemachineTargetYaw, 0.0f);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		public void SetSensitivity(float value)
		{
			if (_currentSensitivity == value) { return; }

			_currentSensitivity = value;
		}

		public void SetSensitivityOvertime(float value, float multiplier)
		{
			if (_currentSensitivity == value) { return; }

			_currentSensitivity = Mathf.Lerp(_currentSensitivity, value, Time.deltaTime * multiplier);

			//Debug.Log("Set sensitivity overtime.");
		}

		public void RotateCharacterTowardsTargetRotation(Transform affectedTransform, Quaternion rotation)
		{
			if (affectedTransform.rotation == rotation) { return; }

			float angle = Quaternion.Angle(transform.rotation, rotation);
			//Debug.Log(angle);

			affectedTransform.rotation = Quaternion.RotateTowards(affectedTransform.rotation, rotation, angle);
		}
		#endregion

		#region Controller Flags - Get / Set
		public bool IsJumping() { return _isJumping; }
		public bool IsRolling() { return _isRolling; }

		public void SetJumpingState(bool value) { _isJumping = value; }
		public void SetRollingState(bool value) { _isRolling = value; }
		#endregion

		public CharacterController GetCharacterController()
        {
			return _characterController;
        }

		public RigBuilderHelper GetRigBuilderHelper()
		{
			return _rigBuilderHelper;
		}
	}
}