using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class DamageOvertime : Effect
    {
        private bool _hasBeenTriggered = false;
        private float _tickDuration = 0;
        private float _currentTickTimer = 0;

        public DamageOvertime(int iD, Transform sender, Transform target, float duration, float tick, int stackLimit, float damage)
        {
            ID = iD;

            Sender = sender;
            Target = target;

            Duration = duration;
            _currentTimer = duration;
            Tick = tick;

            StackLimit = stackLimit;

            Damage = damage;
        }

        public DamageOvertime() : base(0, null, null, 1, 1, 1, 0) { }

        protected override void Apply()
        {
            base.Apply();

            _hasBeenTriggered = true;
        }

        protected override void Remove()
        {
            base.Remove();

            if (_currentInstanceNumber <= 1) 
            { 
                _hasBeenTriggered = false;
                _currentTickTimer = 0;
            }
        }

        public override void Process()
        {
            base.Process();

            if (!_hasBeenTriggered) { return; }

            _currentTickTimer += Time.deltaTime;

            if (_currentTickTimer >= _tickDuration)
            {
                _currentTickTimer = 0;
                Debug.Log(_currentTimer + " " + _tickDuration);

                StatsManager statsManager = Target.GetComponent<StatsManager>();
                if (statsManager) statsManager.ApplyDamageToTarget(Sender, Target, Damage);
            }
        }

        public override void Stack()
        {
            _tickDuration = Duration / Tick;
            _currentTickTimer = _tickDuration;

            Debug.Log("On Stack" + _tickDuration + " " + _currentTickTimer);

            base.Stack();
        }
    }
}