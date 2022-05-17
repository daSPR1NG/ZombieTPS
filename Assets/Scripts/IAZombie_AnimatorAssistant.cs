using UnityEngine;

namespace Khynan_Coding
{
    public class IAZombie_AnimatorAssistant : MonoBehaviour
    {
        private Animator _animator;
        private CombatSystem _combatSystem;

        #region Public References

        #endregion

        void Awake() => Init();

        void Init()
        {
            _animator = GetComponent<Animator>();
            _combatSystem = transform.parent.GetComponent<CombatSystem>();
        }

        public void SetAttackBooleanToFalse()
        {
            AnimatorHelper.SetAnimatorBoolean(_animator, "Attack", false);
        }

        public void ApplyDamageInAnimation()
        {
            _combatSystem.ApplyDamageToTargetWhileInCombat();
        }
    }
}