using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class Effect
    {
        // Id is based on ID + sender > we know an effect is this because of them
        public int ID;
        public Transform Sender;
        public Transform Target;
        public float Duration;
        public float Tick;
        public int StackLimit;
        public float Damage;

        public float _currentTimer = 0;
        public int _currentInstanceNumber = 0;

        public Effect(int iD, Transform sender, Transform target, float duration, float tick, int stackLimit, float damage)
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

        public Effect() : this(0, null, null, 1, 1, 1, 0) { }

        public virtual void Build(/*bool hasOtherInstance*/)
        {
            Debug.Log("Build");

            Actions.OnAddingEffect?.Invoke(this);
            _currentInstanceNumber = 1;
        }

        protected virtual void Apply()
        {
            Debug.Log("Apply");
        }

        protected virtual void Remove()
        {
            Debug.Log("Remove");

            _currentTimer = Duration;
            _currentInstanceNumber = 0;

            Actions.OnRemovingEffect?.Invoke(this);
        }

        public virtual void Process()
        {
            Debug.Log("Process");

            _currentTimer -= Time.deltaTime;

            CheckForTimerEnd();
        }

        private void CheckForTimerEnd()
        {
            if (_currentTimer > 0) { return; }

            _currentTimer = 0;
            Remove();
        }

        public virtual void Stack()
        {
            Debug.Log("Stack");
            _currentTimer = Duration;

            AddInstance(1);

            if (_currentInstanceNumber == StackLimit) { Apply(); }
        }

        private void AddInstance(int value)
        {
            _currentInstanceNumber += value;

            _currentInstanceNumber = Mathf.Clamp(_currentInstanceNumber, 0, StackLimit + 1);
        }

        public void SetTarget (Transform transform)
        {
            Target = transform;
        }
    }
}