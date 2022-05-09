using UnityEngine;

namespace Khynan_Coding
{
    public class CombatSystem : MonoBehaviour
    {
        private float _attackRecover = 2;

        private float _attackCooldown;
        private bool _canAttack = true;

        float _attackAnimationDuration;
        float _attackAnimationTransitionOffset = .15f;

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

        public void SetupAttack(Transform target)
        {
            if (!_canAttack) { return; }

            Debug.Log("Combat System : Attack on " + target.name);

            _IAController.Animator.SetFloat(_animIDAttackSpeed, _statsManager.GetStatByType(StatType.AttackSpeed).CurrentValue);

            _attackAnimationDuration = 
                AnimatorHelper.GetAnimationLength(_IAController.Animator, 2) * 1 / _statsManager.GetStatByType(StatType.AttackSpeed).CurrentValue;

            //Set cooldown
            _attackRecover = _attackAnimationDuration + _attackAnimationTransitionOffset;
            _attackCooldown = _attackRecover;

            AnimatorHelper.PlayThisAnimationOnThisLayer(_IAController.Animator, 1, 1, "Attack", true);

            _canAttack = false;
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
            //Attack
            StatsManager targetStats = _interactionHandler.CurrentTarget.GetComponent<StatsManager>();
            targetStats.ApplyDamageToTarget(
                transform, _interactionHandler.CurrentTarget, _statsManager.GetStatByType(StatType.AttackDamage).CurrentValue);
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
    }
}