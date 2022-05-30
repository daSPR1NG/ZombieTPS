using UnityEngine;

namespace Khynan_Coding
{
    public class GravityApplicator : MonoBehaviour
    {
        [Header("GROUND CHECK")]
        [SerializeField] private Transform _groundCheckTransform;
        [SerializeField] private float _groundCheckRadius = .4f;
        [SerializeField] private LayerMask _groundMask;
        private bool _isGrounded;

        [Header("GRAVITY")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _stickToGroundValue = -.05f;

        private Vector3 _verticalVelocity;

        private bool _rigHaveBeenReassigned = false;

        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;

        private ThirdPersonController _thirdPersonController;
        private CharacterController _controller;

        void Start() => Init();

        void Update()
        {
            ApplyGravity();
            CheckIfGrounded();
        }

        void Init()
        {
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _controller = GetComponent<CharacterController>();

            AssignAnimationIDs();
        }

        private void ApplyGravity()
        {
            if (_isGrounded) { return; }

            // Apply gravity
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _thirdPersonController.VerticalVelocityValue = _verticalVelocity.y;

            _controller.Move(_verticalVelocity * Time.deltaTime);
        }

        private void CheckIfGrounded()
        {
            _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundMask);

            // On the ground
            if (_isGrounded && _verticalVelocity.y < 0)
            {
                // Set ground animations setup : grounded [true], jump [false], falling [false]
                _thirdPersonController.Animator.SetBool(_animIDGrounded, true);
                _thirdPersonController.Animator.SetBool(_animIDJump, false);
                _thirdPersonController.Animator.SetBool(_animIDFreeFall, false);

                WeaponSystem weaponSystemReference = _thirdPersonController.GetComponent<WeaponSystem>();

                if ((!_thirdPersonController.IsRolling() || !weaponSystemReference.IsReloading) && !_rigHaveBeenReassigned)
                {
                    _thirdPersonController.GetRigBuilderHelper().ReAssignHoldingPoseOnTouchingGround();
                    _rigHaveBeenReassigned = true;
                }

                SetVerticalVelocity(0, -2, 0);
                _thirdPersonController.VerticalVelocityValue = _verticalVelocity.y;
                return;
            }

            // In the air
            _thirdPersonController.Animator.SetBool(_animIDGrounded, false);
            _rigHaveBeenReassigned = false;

            if (_thirdPersonController.Animator.GetBool("Jump") && _verticalVelocity.y < 0 || _verticalVelocity.y < -2f)
            {
                _thirdPersonController.Animator.SetBool(_animIDFreeFall, true);
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
        }

        #region Get Gravity Datas
        public bool IsGrounded() { return _isGrounded; }
        public float GetGravityValue() { return _gravity; }
        public float GetStickToGroundValue() { return _stickToGroundValue; }
        public Vector3 GetVerticalVelocity() { return _verticalVelocity; }
        #endregion

        public void SetVerticalVelocity(float xValue, float yValue, float zValue)
        {
            _verticalVelocity = new (xValue, yValue, zValue);
        }
    }
}