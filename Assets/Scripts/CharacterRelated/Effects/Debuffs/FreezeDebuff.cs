using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class FreezeDebuff : Effect
    {
        protected override void Init()
        {
            base.Init();
        }

        #region Core - Apply, Process, Remove
        protected override void BuildUpEffect() => base.BuildUpEffect();

        protected override void ApplyEffect()
        {
            base.ApplyEffect();

            // Activate the freeze effect
            // Reduce target action speed
        }

        public override void RemoveEffect(Effect effect)
        {
            // Activate the freeze effect
            // Reset target action speed

            base.RemoveEffect(effect);
        }

        public override void ProcessDuration() => base.ProcessDuration();
        #endregion

        #region Stack handle
        public override void StackUp() => base.StackUp();

        public override void OnStackSizeLimitReached()
        {
            base.OnStackSizeLimitReached();

            SetCurrentDuration(GetCurrentDuration() * 2);
        }
        #endregion

        public override void Refresh() => base.Refresh();

        #region Constructors
        public FreezeDebuff(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target,
                    float duration, bool isRefreshable,
                        bool isStackable, int stackSize) : 
            
            base(name, isEffectBuiltUp, applicator, target, 0, 0, duration, isRefreshable, isStackable, stackSize) { }

        public FreezeDebuff() : base() { }
        #endregion
    }
}