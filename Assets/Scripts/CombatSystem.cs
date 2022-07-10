using UnityEngine;

namespace Khynan_Coding
{
    public class CombatSystem : MonoBehaviour
    {
        [Header( "SETTINGS" )]
        public bool IsRanged = false;
        [SerializeField] private float _attackAnimationTransitionOffset = .15f;
        [SerializeField] private float _attackAnimationReductionSpeed = 1.25f;

        private float _attackRecover = 2;

        private float _attackCooldown;
        private bool _canAttack = true;

        float _attackAnimationDuration;

        // Animation IDs
        private int _animIDAttackSpeed;

        private StatsManager _statsManager;
        private IAInteractionHandler _interactionHandler;
        private IAController _IAController;

        #region Public References

        #endregion

        void Start() => Init();

        void Update() => ProcessAttackCooldown();

        void Init()
        {
            _statsManager = GetComponent<StatsManager>();
            _interactionHandler = GetComponent<IAInteractionHandler>();
            _IAController = GetComponent<IAController>();

            AssignAnimationIDs();
        }

        public void SetupAttack( Transform target )
        {
            if (!_canAttack) { return; }

            //Debug.Log("Combat System : Attack on " + target.name);

            RotateTowardsTarget( target );

            float attackAnimationSpeed = _statsManager.GetStat(StatAttribute.AttackSpeed).GetCurrentValue() / _attackAnimationReductionSpeed;

            _IAController.Animator.SetFloat(_animIDAttackSpeed, attackAnimationSpeed);

            _attackAnimationDuration = 
                (AnimatorHelper.GetAnimationLength(_IAController.Animator, 2)) * 1 / _statsManager.GetStat(StatAttribute.AttackSpeed).GetCurrentValue();

            //Set cooldown
            _attackRecover = _attackAnimationDuration /*- _attackAnimationTransitionOffset*/;
            _attackCooldown = _attackRecover;

            if ( !IsRanged ) AnimatorHelper.HandleThisAnimation( _IAController.Animator, "Attack", true, 1, 1 );
            if ( IsRanged ) AnimatorHelper.HandleThisAnimation( _IAController.Animator, "RangedAttack", true, 1, 1 );

            _canAttack = false;
        }

        public float RotationSpeed = 15;
        private void RotateTowardsTarget( Transform target )
        {
            if ( !IsRanged ) return;

            transform.LookAt( new Vector3( target.position.x, transform.position.y, target.position.z ) );
        }

        private void ProcessAttackCooldown()
        {
            if (_canAttack) { return; }

            _attackCooldown -= Time.deltaTime;

            if (_attackCooldown <= 0)
            {
                _attackCooldown = 0;
                _canAttack = true;
            }
        }

        // This is called in animation
        public void ApplyDamageToTargetWhileInCombat()
        {
            if ( _interactionHandler._isTargetTooFar || IsRanged ) { return; }

            StatsManager targetStats = _interactionHandler.CurrentTarget.GetComponent<StatsManager>();
            Animator animator = _interactionHandler.CurrentTarget.GetComponent<ThirdPersonController>().Animator;

            if ( !targetStats.IsCharacterDead() ) { AnimatorHelper.HandleThisAnimation( animator, "GotHit", true, 1, 1 ); }

            targetStats.ApplyDamageToTarget(
                transform, 
                _interactionHandler.CurrentTarget, 
                _statsManager.GetStat(StatAttribute.AttackDamage).GetCurrentValue());
        }

        public void ResetAttackState()
        {
            _attackCooldown = 0;
            _canAttack = true;
        }

        private void AssignAnimationIDs()
        {
            _animIDAttackSpeed = Animator.StringToHash("AttackSpeed");
        }

        public bool CanAttack() { return _canAttack; }
    }
}