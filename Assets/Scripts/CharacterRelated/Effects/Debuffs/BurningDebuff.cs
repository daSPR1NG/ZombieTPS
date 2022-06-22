using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class BurningDebuff : Effect
    {
        protected override void Init() => base.Init();

        #region Core - Apply, Process, Remove
        protected override void BuildUpEffect() => base.BuildUpEffect();

        protected override void ApplyEffect() => base.ApplyEffect();

        public override void RemoveEffect(Effect effect) => base.RemoveEffect(effect);

        public override void ProcessDuration()
        {
            base.ProcessDuration();

            // Each X seconds the target take X amount of damage over the duration

            ApplyDamageOvertime();
        }
        #endregion

        #region Stack handle
        public override void StackUp()
        {
            base.StackUp();

            //SetCurrentDuration(GetDuration() * 2);
            //
        }

        public override void OnStackSizeLimitReached()
        {
            base.OnStackSizeLimitReached();
        }
        #endregion

        public override void Refresh()
        {
            base.Refresh();

            //
        }

        #region Constructors
        public BurningDebuff(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target, int tick, float damage, 
                    float duration, bool isRefreshable, 
                        bool isStackable, int stackLimit) : 
            base(name, isEffectBuiltUp, applicator, target, tick, damage, duration, isRefreshable, isStackable, stackLimit) { }

        public BurningDebuff() : base() { }
        #endregion
    }
}