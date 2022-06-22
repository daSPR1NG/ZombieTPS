using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private List<Effect> _effects = new ();

        [Space(2), Header("DEBUG")]
        [Min(0), Max(4)] public int Index = 0;
        public BurningDebuff BurningDebuff = new();
        public FreezeDebuff FreezeDebuff = new();

        private void OnEnable()
        {
            Actions.OnAddingEffect += AddEffectToList;
            Actions.OnRemovingEffect += RemoveEffectFromList;
        }

        private void OnDisable()
        {
            Actions.OnAddingEffect -= AddEffectToList;
            Actions.OnRemovingEffect -= RemoveEffectFromList;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                //BurningDebuff.ManageEffect();
                new BurningDebuff("Burning Debuff - Test", false, transform, transform, 7, 5, 2, false, true, 2).ManageEffect();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //FreezeDebuff.ManageEffect();
                new FreezeDebuff("Freeze Debuff - Test", true, transform, transform, 7, false, true, 2).ManageEffect();
            }
        }

        private void LateUpdate() => HandleEffectDuration();

        #region Add / Remove 
        public void AddEffectToList(Effect effect)
        {
            if (HasEffectStackLimitBeenReached(effect))
            {
                // This means that the last one added only is refreshed, not the other previously applied
                effect.Refresh();
                return;
            }

            _effects.Add(effect);

            if (HasMoreThanOneInstance(effect)) { effect.StackUp(); }

            if (GetEffectAppliedAmount(effect) >= effect.GetStackLimit()) 
            {
                effect.OnStackSizeLimitReached(); 
            }
        }

        public void RemoveEffectFromList(Effect effect)
        {
            _effects.Remove(effect);
        }
        #endregion

        private void RefreshAllInstancesOfThisEffect(Effect effect)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i].GetName() != effect.GetName()) { continue; }

                if (_effects[i].GetName() == effect.GetName() && _effects[i].IsRefreshable()) { _effects[i].Refresh(); }
            }
        }

        private void HandleEffectDuration()
        {
            if (_effects.Count == 0) { return; }

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].ProcessDuration();

                //Debug.Log("Effect current timer " +
                //    _effects[i].GetCurrentTimer() +
                //    " | Will remains for : " +
                //    _effects[i].GetCurrentDuration());
            }
        }

        private int GetEffectAppliedAmount(Effect effect)
        {
            if (_effects.Count == 0) { return 0; }

            int appliedAmount = 0;

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i].GetName() != effect.GetName()) { continue; }

                appliedAmount++;

                //Debug.Log("This effect "
                //+ effect.GetName()
                //+ " has been applied "
                //+ appliedAmount
                //+ ".");
            }

            return appliedAmount;
        }

        private bool HasEffectStackLimitBeenReached(Effect effect)
        {
            return GetEffectAppliedAmount(effect) >= effect.GetStackLimit();
        }

        private bool HasMoreThanOneInstance(Effect effect)
        {
            return GetEffectAppliedAmount(effect) > 1;
        }
    }
}