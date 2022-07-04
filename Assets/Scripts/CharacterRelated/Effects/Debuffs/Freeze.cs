using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class Freeze : Effect
    {
        private float _targetBaseMovementSpeed;
        private float _targetBaseAttackSpeed;
        private float _targetAnimationBaseSpeed;

        private Animator _targetAnimator;

        public Freeze(int iD, Transform sender, Transform target, float duration, float tick)
        {
            ID = iD;

            Sender = sender;
            Target = target;

            Duration = duration;
            _currentTimer = duration;
            Tick = tick;

            StackLimit = 3;
        }

        public Freeze() : base(0, null, null, 1, 1, 1, 0) { }

        public override void Build()
        {
            if (!_targetAnimator) { StoreTargetActionSpeedValues(); }

            ModifyTargetActionSpeedValues(
                Target.GetChild(0).GetComponent<Animator>(),
                Target.GetComponent<StatsManager>(),
                new StatModifier(ModifierType.Percentage, -25, this, StatAttribute.MovementSpeed));

            base.Build();
        }

        protected override void Apply()
        {
            base.Apply();

            //ModifyTargetActionSpeedValues(
            //        Target.GetChild(0).GetComponent<Animator>(),
            //        Target.GetComponent<StatsManager>(),
            //        _applicationStatModifier);
        }

        protected override void Remove()
        {
            ResetTargetActionSpeedValues();

            base.Remove();
        }

        public override void Process()
        {
            base.Process();
        }

        public override void Stack()
        {
            base.Stack();
        }

        private void StoreTargetActionSpeedValues()
        {
            Animator animator = Target.GetChild(0).GetComponent<Animator>();
            _targetAnimator = animator;
            _targetAnimationBaseSpeed = animator.speed;

            StatsManager targetStatsManager = Target.GetComponent<StatsManager>();
            _targetBaseMovementSpeed = targetStatsManager.GetStat(StatAttribute.MovementSpeed).GetCurrentValue();
            _targetBaseAttackSpeed = targetStatsManager.GetStat(StatAttribute.AttackSpeed).GetCurrentValue();
        }

        private void ResetTargetActionSpeedValues()
        {
            _targetAnimator.speed = _targetAnimationBaseSpeed;
            _targetAnimationBaseSpeed = 0;

            StatsManager targetStatsManager = Target.GetComponent<StatsManager>();
            targetStatsManager.GetStat(StatAttribute.MovementSpeed).RemoveSourceModifier(this);
            targetStatsManager.GetStat(StatAttribute.AttackSpeed).RemoveSourceModifier(this);
        }

        private void ModifyTargetActionSpeedValues(Animator animator, StatsManager targetStatsManager, StatModifier statModifier)
        {
            Debug.Log(_targetAnimationBaseSpeed);

            // Reduce animator + action speed of the target
            targetStatsManager.GetStat(StatAttribute.MovementSpeed).AddModifier(statModifier);
            targetStatsManager.GetStat(StatAttribute.AttackSpeed).AddModifier(statModifier);
            animator.speed /= (statModifier.ModifierValue / 100);
        }
    }
}