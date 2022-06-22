using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public class UIEffectBox : MonoBehaviour
    {
        private Effect _heldEffect;
        private float _currentTimer;

        void LateUpdate() => ProcessDuration();

        public void Setup(Effect effect)
        {
            _heldEffect = effect;
            _currentTimer = effect.GetCurrentDuration();
        }

        public void GetRemoved(List<UIEffectBox> list)
        {
            list.Remove(this);
            Destroy(gameObject);
        }

        private void ProcessDuration()
        {
            _currentTimer -= Time.deltaTime;

            if (_currentTimer <= 0)
            {
                _currentTimer = 0;
            }
        }

        public Effect GetEffect() { return _heldEffect; }
    }
}