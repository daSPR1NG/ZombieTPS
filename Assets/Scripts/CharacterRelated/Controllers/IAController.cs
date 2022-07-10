using UnityEngine;

namespace Khynan_Coding
{
    public class IAController : DefaultController
    {
        [Header("CONTROLLER SETTINGS")]
        [SerializeField] private bool _doesChasePlayer = true;

        private IAInteractionHandler _interactionHandler;

        // animation IDs
        private float _motionSpeedValue;
        private int _animIDMotionSpeed;

        #region Public References

        #endregion

        protected override void Awake() => base.Awake();

        private void Start() => Init();

        protected override void Update()
        {
            base.Update();
            SetMotionSpeed();
        }

        void Init()
        {
            SetDefaultStateAtStart(Idle());

            _interactionHandler = GetComponent<IAInteractionHandler>();
            if (_doesChasePlayer) { _interactionHandler.SetTarget(GameManager.Instance.ActivePlayer); }
            
            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        public float MotionSpeedOffset = 0;
        private void SetMotionSpeed()
        {
            StatsManager statsManager = GetComponent<StatsManager>();

            float offset = 
                statsManager.GetStat( StatAttribute.MovementSpeed ).GetCurrentValue() >= 0.5f 
                                    && statsManager.GetStat( StatAttribute.MovementSpeed ).GetCurrentValue() <= 1.5f
                ? MotionSpeedOffset : -MotionSpeedOffset;

            float moveSpeed = statsManager.GetStat( StatAttribute.MovementSpeed ).GetCurrentValue();

            _motionSpeedValue = CharacterIsMoving ? _motionSpeedValue += Time.deltaTime : 0;
            _motionSpeedValue = Mathf.Clamp( _motionSpeedValue, 0, moveSpeed + offset );

            Animator.SetFloat(_animIDMotionSpeed, _motionSpeedValue);
        }
    }
}