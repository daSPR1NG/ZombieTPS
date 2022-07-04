using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private List<Effect> _effects = new ();

        [Space(2), Header("DEBUG")]
        [Min(0)] public int Stack = 1;
        public DamageOvertime DamageOvertime = new();

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
                DamageOvertime.Build(/*HasOtherInstance(DamageOvertime)*/);

                Debug.Log("Has other instance : " + HasOtherInstance(DamageOvertime));
            }

            HandleEffectDuration();
        }

        #region Add / Remove 
        public void AddEffectToList(Effect effect)
        {
            if (_effects.Count >= 1 
                && GetSameEffect(effect)._currentInstanceNumber > GetSameEffect(effect).StackLimit) 
            {
                return; 
            }

            if (HasOtherInstance(GetSameEffect(effect)))
            {
                GetSameEffect(effect).Stack();
                return;
            }

            _effects.Add(effect);
        }

        public void RemoveEffectFromList(Effect effect)
        {
            _effects.Remove(effect);
        }
        #endregion

        private void HandleEffectDuration()
        {
            if (_effects.Count == 0) { return; }

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].Process();
            }
        }

        public bool HasOtherInstance(Effect effect)
        {
            if (_effects.Count == 0) { return false; }

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (AreEffectTheSame(_effects[i], effect) || effect.StackLimit == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AreEffectTheSame(Effect a, Effect b)
        {
            if (a.ID == b.ID && a.Sender == b.Sender)
            {
                return true;
            }

            return false;
        }

        private Effect GetSameEffect(Effect effect)
        {
            if (_effects.Count == 0) { return null; }

            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (AreEffectTheSame(_effects[i], effect) || effect.StackLimit == 1)
                {
                    return _effects[i];
                }
            }

            return null;
        }
    }
}