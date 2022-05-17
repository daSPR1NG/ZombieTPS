using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public class PlayerRoll : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private bool _hideHandsWhileRolling = false;
        [SerializeField] private float _rollForce = 5;
        [SerializeField] private float _rollSpeed = 1;
        [SerializeField] private float _rollCancelTimer = 0.75f;
        [SerializeField] private float _rotationUpdateSpeed = 5;
        [SerializeField] private AnimationCurve _rollCurve;

        private float _rollTimer;

        private PlayerInput _inputs;
        private InputAction _roll;

        private Animator _animator;
        private ThirdPersonController _thirdPersonController;
        private WeaponSystem _weaponSystem;

        private void OnEnable()
        {
            if (_thirdPersonController.CanRoll) { _roll.performed += context => StartCoroutine(Roll()); }
        }

        private void OnDisable()
        {
            if (_thirdPersonController.CanRoll) { _roll.performed -= context => StopCoroutine(Roll()); }
        }

        private void Awake()
        {
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _weaponSystem = GetComponent<WeaponSystem>();

            if (!_thirdPersonController.CanRoll) { return; }

            _inputs = GetComponent<PlayerInput>();
            _roll = _inputs.actions["Roll"];
        }

        void Start() => Init();

        void Init()
        {
            _animator = transform.GetChild(0).GetComponent<Animator>();

            SetRollCurveLength();

            Debug.Log(_rollTimer);
        }

        private IEnumerator Roll()
        {
            bool canRoll = _thirdPersonController.GetCharacterController().velocity != Vector3.zero 
                && !_thirdPersonController.IsRolling();

            if (!canRoll) { yield break; }

            if (_weaponSystem.IsReloading && _weaponSystem.ReloadingCanBeCanceled)
            {
                _weaponSystem.CancelReloading();
            }

            if (_weaponSystem.IsAiming) { _weaponSystem.CancelAim(); }

            SetRollCurveLength();
            AnimatorHelper.HandleThisAnimation(_animator, "Roll", true, 0, 1);

            _thirdPersonController.GetRigBuilderHelper().FreeHandsWhileInAir(_hideHandsWhileRolling);

            float timer = 0;

            _thirdPersonController.SetRollingState(true);

            //AnimatorHelper.DebugAnimationsDuration(_animator);

            RotateCharacterBeforePerformingRoll();

            float executionPercentage;

            while (!(timer >= _rollTimer))
            {
                float rollSpeed = _rollCurve.Evaluate(timer);
                Vector3 rollDirection = 
                    (transform.forward * rollSpeed) + (Vector3.up * _thirdPersonController.VerticalVelocityValue);

                _thirdPersonController.GetCharacterController().Move(_rollForce * Time.deltaTime * rollDirection);

                timer += Time.deltaTime;
                executionPercentage = timer / _rollTimer;

                Debug.Log("ROLL EXECUTION PERCENTAGE : " + executionPercentage);

                if (executionPercentage >= _rollCancelTimer)
                {
                    _thirdPersonController.SetRollingState(false);
                    AnimatorHelper.SetAnimatorBoolean(_animator, "Roll", false);
                    if (_weaponSystem.IsAimInputPressed()) { _weaponSystem.Aim(); }

                    yield break;
                }

                _thirdPersonController.GetRigBuilderHelper().ReAssignHandsOnTouchingGround();

                yield return null;
            }
        }

        private void SetRollCurveLength()
        {
            AnimatorHelper.SetAnimatorFloat(_animator, "Roll Speed", _rollSpeed);

            Keyframe rollLastFrame = _rollCurve[index: _rollCurve.length - 1];
            rollLastFrame.time = AnimatorHelper.GetAnimationLength(_animator, 8) * 1 / _rollSpeed;
            _rollTimer = rollLastFrame.time;
        }

        private void RotateCharacterBeforePerformingRoll()
        {
            Vector3 inputDirection = new(
                _thirdPersonController.GetInputMovementValue().x,
                0,
                _thirdPersonController.GetInputMovementValue().y);

            transform.forward = Helper.GetMainCamera().transform.TransformDirection(inputDirection);
        }
    }
}