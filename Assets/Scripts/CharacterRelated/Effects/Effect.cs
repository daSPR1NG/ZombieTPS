using UnityEngine;

namespace Khynan_Coding
{
    public enum Category
    {
        Unassigned, None, Buff, Debuff
    }
    public enum SubType
    {
        Unassigned, None, Common, Rare, Epic, Legendary, Artifact, Heirloom,
    }

    // NOTES : SEE HOW THE BUILD UP FEATURE CAN BE DONE -> FOR THE MOMENT THE EFFECT IS APPLIED DIRECTLY.

    [System.Serializable]
    public class Effect
    {
        #region Initial Constructor
        public Effect(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target, int tick, float damage, 
                    float duration, bool isRefreshable, 
                        bool isStackable, int stackLimit)
        {
            _name = name;
            _isEffectBuiltUp = isEffectBuiltUp;

            _applicator = applicator;
            _target = target;
            _tick = tick;
            _damage = damage;

            _duration = duration;
            _currentDuration = duration;
            _isRefreshable = isRefreshable;

            _isStackable = isStackable;
            _stackLimit = stackLimit;
        }
        #endregion

        // !!!!!! PUT ALL VARIABLES IN PRIVATE WHEN DEBUGGING PHASE IS OVER !!!!!!

        // IDENTITY
        public string _name = "[New Effect]";
        public int _ID;
        public bool _isEffectBuiltUp = false;

        // APPLICATION SETTINGS
        public bool _isStackable = false;
        public int _stackLimit = 1;

        // TIMERS
        public bool _isRefreshable = false;
        public float _duration = 4f;
        private float _currentDuration = 4f;
        public float _currentTimer;

        // DAMAGE HANDLER
        public Transform _applicator;
        public Transform _target;
        public float _damage = 5;
        public int _tick = 5;
        protected float _dotTickTimer;
        protected float _currentDotTickTimer;

        #region Initialization
        protected virtual void Init()
        {
            SetCurrentTimer(_duration);
            SetID();
            SetDoTTimers();
        }

        private void SetID()
        {
            if (_ID == 0) { _ID = GetHashCode(); }
        }

        private void SetDoTTimers()
        {
            if (_duration == 0 || _tick == 0) { return; }

            _dotTickTimer = GetDuration() / GetTick();
            _currentDotTickTimer = _dotTickTimer;
        }
        #endregion

        #region Core - Handle, BuildUp, Apply, Process, Remove
        public void ManageEffect()
        {
            Debug.Log("Manage Effect " + GetName());

            Init();
            Actions.OnAddingEffect?.Invoke(this);

            if (_isEffectBuiltUp)
            {
                BuildUpEffect();
                return;
            }

            ApplyEffect();
        }

        protected virtual void BuildUpEffect()
        {
            Debug.Log("BuildUp Effect " + GetName());
        }

        protected virtual void ApplyEffect()
        {
            Debug.Log("Apply Effect " + GetName());

            ApplyEffectDamage();
        }

        public virtual void RemoveEffect(Effect effect)
        {
            Debug.Log("Remove Effect " + GetName());

            Actions.OnRemovingEffect?.Invoke(effect);
        }

        public virtual void ProcessDuration()
        {
            if (_duration == 0) { return; }

            SetCurrentTimer(_currentTimer - Time.deltaTime);

            if (_currentTimer <= 0)
            {
                SetCurrentTimer(0);
                RemoveEffect(this);
            }
        }
        #endregion

        #region Stack handle
        public virtual void StackUp()
        {
            if (!_isStackable) { return; }

            Debug.Log("Stack Effect " + GetName());
        }

        public virtual void OnStackSizeLimitReached()
        {
            if (!_isStackable) { return; }

            Debug.Log("Stack Size Limit Reached " + GetName());

            Actions.OnModifyingEffect?.Invoke(this);
        }
        #endregion

        public virtual void Refresh()
        {
            if (!_isRefreshable) { return; }

            SetCurrentTimer(_duration);

            Debug.Log("Refresh Effect " + GetName());

            Actions.OnModifyingEffect?.Invoke(this);
        }

        #region Application - Dot / Damage
        protected void ApplyDamageOvertime()
        {
            if (_tick == 0)
            {
                Debug.LogError("No stats found on this target or tick & damage variables might be equal to zero.", GetTarget());
                return;
            }

            _currentDotTickTimer -= Time.deltaTime;

            if (_currentDotTickTimer <= 0)
            {
                _currentDotTickTimer = _dotTickTimer;
                ApplyEffectDamage();
                Debug.Log("Apply Dot damage");
            }
        }

        protected virtual void ApplyEffectDamage()
        {
            StatsManager statsManager = GetTarget().GetComponent<StatsManager>();

            if (!statsManager || _tick == 0 || _damage == 0) { return; }

            statsManager.ApplyDamageToTarget(_applicator, _target, _damage);

            Debug.Log("Apply Effect Damage Of " + _name);
        }
        #endregion

        #region Identity - Get
        public string GetName() { return _name; }
        public int GetID() { return _ID; }
        #endregion

        #region Duration, Timer - Get / Set
        public float GetDuration() { return _duration; }

        public float GetCurrentDuration() { return _currentDuration; }
        public void SetCurrentDuration(float value) 
        {
            _currentDuration = value;
            SetCurrentTimer(value);
        }

        public bool IsRefreshable() { return _isRefreshable; }

        public float GetCurrentTimer() { return _currentTimer; }
        public void SetCurrentTimer(float value)
        {
            _currentTimer = value;
        }
        #endregion

        #region Stack Infos - Get / Set
        public bool IsStackable() { return _isStackable; }
        public int GetStackLimit() { return _stackLimit; }
        #endregion

        #region Damage application parameters
        public Transform GetApplicator() { return _applicator; }
        public Transform GetTarget() { return _target; }
        public float GetTick() { return _tick; }
        public float GetDamage() { return _damage; }
        #endregion

        #region Other Constructors
        // Effect not stackable
        public Effect(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target, int tick, float damage, 
                    float duration, bool isRefreshable) : 

            this (name, isEffectBuiltUp, applicator, target, tick, damage, duration, isRefreshable, false, 1) { }

        // Effect with no damage
        public Effect(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target,
                    float duration, bool isRefreshable, 
                        bool isStackable, int stackSize) :
            
            this(name, isEffectBuiltUp, applicator, target, 0, 0, duration, isRefreshable, isStackable, stackSize) { }

        // Effect with no damage and no duration
        public Effect(
            string name, bool isEffectBuiltUp,
                Transform applicator, Transform target) :

            this(name, isEffectBuiltUp, applicator, target, 0, 0, 0, false, false, 1) { }

        // Default constructor
        public Effect() : this ("", false, null, null, 0, 0, 0, false, false, 1) { }
        #endregion
    }
}