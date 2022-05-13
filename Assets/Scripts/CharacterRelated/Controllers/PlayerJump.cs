using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    public class PlayerJump : MonoBehaviour
    {
        [Header("JUMP")]
        [SerializeField] private bool _hideHandsWhileInAir = false;
        [SerializeField] private float _jumpHeight = 5f;
       
        private Vector3 _velocity;

        [Header("GROUND CHECK")]
        [SerializeField] private Transform _groundCheckTransform;
        [SerializeField] private float _groundCheckRadius = .4f;
        [SerializeField] private LayerMask _groundMask;
        private bool _isGrounded;

        [Header("GRAVITY")]
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _stickToGroundValue = -.05f;

        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;

        private PlayerInput _input;
        private InputAction _jumpInput;

        private ThirdPersonController _thirdPersonController;
        private PlayerInteractionHandler _playerInteractionHandler;
        private CharacterController _controller;
        private RigBuilderHelper _rigBuilderHelper;

        private void Awake()
        {
            _thirdPersonController = GetComponent<ThirdPersonController>();

            if (!_thirdPersonController.CanJump) { return; }

            _input = GetComponent<PlayerInput>();
            _jumpInput = _input.actions["Jump"];
        }

        #region OnEnable / OnDisable
        private void OnEnable()
        {
            if (_thirdPersonController.CanJump) { _jumpInput.performed += context => Jump(); }
        }

        private void OnDisable()
        {
            if (_thirdPersonController.CanJump) { _jumpInput.performed -= context => Jump(); }
        }
        
        #endregion

        void Start() => Init();

        void Update()
        {
            CheckIfGrounded();
            ApplyGravity();

            HandleMovementInAir();
        }

        void Init()
        {
            _controller = GetComponent<CharacterController>();
            _groundCheckRadius = _controller.radius;
           
            _playerInteractionHandler = GetComponent<PlayerInteractionHandler>();
            _rigBuilderHelper = GetComponent<GlobalCharacterParameters>().GetRigBuilderHelper();

            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
        }

        private void HandleMovementInAir()
        {
            if (_isGrounded) { return; }

            // Move in the air like you move while grounded, this creates a smoother movement
            Vector3 moveDirection = Helper.GetMainCamera().transform.forward * _thirdPersonController.GetInputMovementValue().y;
            moveDirection += Helper.GetMainCamera().transform.right * _thirdPersonController.GetInputMovementValue().x;
            moveDirection.y = 0;

            _controller.Move(moveDirection * Time.deltaTime);
        }

        private void Jump()
        {
            if (_isGrounded && !_playerInteractionHandler.IsInteracting)
            {
                FreeHandsWhileInAir();

                // Play the jump animation and apply the jump velocity
                _thirdPersonController.Animator.SetBool(_animIDJump, true);

                float jumpVelocity = _jumpHeight * _stickToGroundValue * _gravity;

                _velocity.y = Mathf.Sqrt(jumpVelocity);
                _thirdPersonController.VerticalVelocityValue = _velocity.y;

                Debug.Log("JUMP");
            }
        }

        private void ApplyGravity()
        {
            if (_isGrounded) { return; }

            // Apply gravity
            _velocity.y += _gravity * Time.deltaTime;
            _thirdPersonController.VerticalVelocityValue = _velocity.y;

            _controller.Move(_velocity * Time.deltaTime);
        }

        private void CheckIfGrounded()
        {
            _isGrounded = Physics.CheckSphere(_groundCheckTransform.position, _groundCheckRadius, _groundMask);

            // On the ground
            if (_isGrounded && _velocity.y < 0)
            {
                // Set ground animations setup : grounded [true], jump [false], falling [false]
                _thirdPersonController.Animator.SetBool(_animIDGrounded, true);
                _thirdPersonController.Animator.SetBool(_animIDJump, false);
                _thirdPersonController.Animator.SetBool(_animIDFreeFall, false);

                ReAssignHandsOnTouchingGround();

                _velocity.y = -2f;
                _thirdPersonController.VerticalVelocityValue = _velocity.y;
                return;
            }

            // In the air
            _thirdPersonController.Animator.SetBool(_animIDGrounded, false);

            if (_thirdPersonController.Animator.GetBool(_animIDJump) && _velocity.y < 0 || _velocity.y < -2f)
            {
                _thirdPersonController.Animator.SetBool(_animIDFreeFall, true);
            }
        }

        private void FreeHandsWhileInAir()
        {
            if (!_hideHandsWhileInAir) { return; }

            // Set the hand free state
            _rigBuilderHelper.DisableAllRigLayers();
            _rigBuilderHelper.HideHeldObject();
        }

        private void ReAssignHandsOnTouchingGround()
        {
            if (!_hideHandsWhileInAir) { return; }

            // Display and re-enable the holding weapon state
            _rigBuilderHelper.EnableAllRigLayers();
            _rigBuilderHelper.DisplayHeldObject();
        }
    }
}