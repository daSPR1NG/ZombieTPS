using UnityEngine;
using UnityEngine.InputSystem;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GravityApplicator))]
    public class PlayerJump : MonoBehaviour
    {
        [Header("JUMP")]
        [SerializeField] private bool _hideHandsWhileInAir = false;
        [SerializeField] private float _jumpHeight = 5f;
        [SerializeField] private float _speedInAir = 5f;

        [Header("AUDIO")]
        [SerializeField] private ControllerAudioSetting _audioSetting;

        private int _animIDJump;

        private PlayerInput _input;
        private InputAction _jumpInput;

        private ThirdPersonController _thirdPersonController;
        private PlayerInteractionHandler _playerInteractionHandler;
        private CharacterController _controller;
        private GravityApplicator _gravityAppplicator;
        
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

        void Update() => HandleMovementInAir();

        void Init()
        {
            _gravityAppplicator = GetComponent<GravityApplicator>();

            _controller = _thirdPersonController.GetCharacterController();

            _playerInteractionHandler = GetComponent<PlayerInteractionHandler>();

            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDJump = Animator.StringToHash("Jump");
        }

        private void HandleMovementInAir()
        {
            if (_gravityAppplicator.IsGrounded()) 
            {
                _thirdPersonController.SetJumpingState(false);
                
                return; 
            }

            // Move in the air like you move while grounded, this creates a smoother movement
            Vector3 moveDirection = Helper.GetMainCamera().transform.forward * _thirdPersonController.GetInputMovementValue().y;
            moveDirection += Helper.GetMainCamera().transform.right * _thirdPersonController.GetInputMovementValue().x;
            moveDirection.y = 0;

            _controller.Move(moveDirection * _speedInAir * Time.deltaTime);
        }

        private void Jump()
        {
            if (_gravityAppplicator.IsGrounded() && !_playerInteractionHandler.IsInteracting)
            {
                _thirdPersonController.SetJumpingState(true);

                WeaponSystem weaponSystemReference = _thirdPersonController.GetComponent<WeaponSystem>();

                if (!weaponSystemReference.IsReloading)
                {
                    _thirdPersonController.GetRigBuilderHelper().FreeHandsWhileInAir(_hideHandsWhileInAir);
                }

                PlayJumpSound(GetComponent<AudioSource>());

                // Play the jump animation and apply the jump velocity
                _thirdPersonController.Animator.SetBool(_animIDJump, true);

                float jumpVelocity = 
                    _jumpHeight * 
                    _gravityAppplicator.GetStickToGroundValue() * _gravityAppplicator.GetGravityValue();

                _gravityAppplicator.SetVerticalVelocity(0, Mathf.Sqrt(jumpVelocity), 0);
                _thirdPersonController.VerticalVelocityValue = _gravityAppplicator.GetVerticalVelocity().y;

                Debug.Log("JUMP");
            }
        }

        private void PlayJumpSound(AudioSource audioSource)
        {
            if (!audioSource) { return; }

            // Randomize pitch...
            float pitchOverride = Random.Range(_audioSetting.GetPitchMinValue(), _audioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(audioSource, pitchOverride);

            // ... Randomize volume...
            float randomVolume = Random.Range(_audioSetting.GetVolumeMinValue(), _audioSetting.GetVolumeMaxValue());

            // ... And then play the sound.
            AudioHelper.PlayOneShot(audioSource, _audioSetting.GetAudioClip(), randomVolume);
        }
    }
}