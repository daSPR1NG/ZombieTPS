using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum HealthInteraction
    {
        None, Damage, Heal
    }

    [DisallowMultipleComponent]
    public class StatsManager : MonoBehaviour, IDamageable, IHealable, IKillable
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private bool _isInvulnerable = false;
        [SerializeField] private List<Stat> _stats = new();

        [Header("LOOK")]
        [SerializeField] private GameObject _popupPrefab;
        [SerializeField] private GameObject _deathVFX;
        [SerializeField] private GameObject _healVFX;

        [Header("AUDIO")]
        [SerializeField] private bool _emitsDeathSound = true;
        [SerializeField] private ControllerAudioSettingList _controllerAudioSettingList;

        private float _healthPercentage = 0;

        private DefaultController _defaultController;
        GlobalCharacterParameters _globalCharacterParameters;

        #region Public references
        public List<Stat> Stats { get => _stats; set => _stats = value; }
        #endregion

        private void Awake() => Init();

        private void Start() => InitHealth();

        #region Debug
#if UNITY_EDITOR
        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ApplyDamageToTarget(transform, transform, 15);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                HealTarget(transform, transform, 15);
            }
        }
#endif
    #endregion

        #region Stats methods - Init, Getter, IsNullCheck
        protected virtual void Init()
        {
            _defaultController = GetComponent<DefaultController>();
            _globalCharacterParameters = GetComponent<GlobalCharacterParameters>();
            InitStats();
        }

        private void InitStats()
        {
            if (Stats.Count == 0) { return; }

            for (int i = 0; i < Stats.Count; i++)
            {
                if (Stats[i].GetAttribute() == StatAttribute.Unassigned) { continue; }

                Stats[i].SetStatName(Stats[i].GetAttribute().ToString());
                Stats[i].MatchCurrentValueWithBaseValue();
            }

            InitNavMeshAgent();
            CalculateHealthPercentage();

            if (_healVFX) { _healVFX.SetActive(false); }
        }

        private void InitHealth()
        {
            switch (_globalCharacterParameters.CharacterType)
            {
                case CharacterType.Player:
                    Actions.OnPlayerHealthValueInitialized?.Invoke(
                    GetStat(StatAttribute.Health).GetCurrentValue(), GetStat(StatAttribute.Health).GetMaxValue());
                    break;
                case CharacterType.IA_Enemy:
                    break;
                case CharacterType.IA_Ally:
                    // Nothing for now
                    break;
            }
        }

        private void InitNavMeshAgent()
        {
            if (TryGetComponent(out UnityEngine.AI.NavMeshAgent navMeshAgent))
            {
                if (!DoesThisStatTypeExists(StatAttribute.MovementSpeed) || navMeshAgent.speed == GetStat(StatAttribute.MovementSpeed).GetCurrentValue()) { return; }

                navMeshAgent.speed = GetStat(StatAttribute.MovementSpeed).GetCurrentValue();
            }
        }

        public Stat GetStat(StatAttribute statType)
        {
            if (Stats.Count == 0 || !DoesThisStatTypeExists(statType)) 
            { 
                Debug.LogError("No stat found."); 
                return null; 
            }

            for (int i = 0; i < Stats.Count; i++)
            {
                if (Stats[i].GetAttribute() == statType)
                {
                    return Stats[i];
                }
            }

            Debug.LogError("The stat you were looking for does not match any existing types.");
            return null;
        }

        public bool DoesThisStatTypeExists(StatAttribute statType)
        {
            if (Stats.Count == 0)
            {
                Debug.LogError("The list stats is empty");
                return false;
            }

            for (int i = 0; i < Stats.Count; i++)
            {
                if (Stats[i].GetAttribute() == statType)
                {
                    return true;
                }
            }

            Debug.LogError("The stat type (" + statType + ") does not exists.");
            return false;
        }
        #endregion

        public virtual void ApplyDamageToTarget(Transform provider, Transform target, float damageAmount)
        {
            if (IsCharacterDead()) { return; }

            if (_isInvulnerable)
            {
                Debug.Log("Target is invulnerable so we create a damage popup with invulnerable string value.");

                DamagePopup.CreateDamagePopup(_popupPrefab, target, 0, PopupType.Invulnerable);
                return;
            }

            StatsManager statsManager = target.GetComponent<StatsManager>();

            if (!statsManager || !statsManager.DoesThisStatTypeExists(StatAttribute.Health))
            {
                Debug.Log("Stats manager is not present on this object or it is not using health", transform);
            }

            float newHealthValue = statsManager.GetStat(StatAttribute.Health).GetCurrentValue() - damageAmount;
            statsManager.GetStat(StatAttribute.Health).SetCurrentValue(newHealthValue);

            DamagePopup.CreateDamagePopup(_popupPrefab, target, damageAmount, PopupType.Damage);

            if (_globalCharacterParameters.CharacterType == CharacterType.Player)
            {
                Actions.OnPlayerHealthValueChanged?.Invoke(
                    GetStat(StatAttribute.Health).GetCurrentValue(), 
                    GetStat(StatAttribute.Health).GetMaxValue(), 
                    HealthInteraction.Damage);
            }

            if (statsManager.GetStat(StatAttribute.Health).GetCurrentValue() <= 0)
            {
                Debug.Log("Character is dead.");
                OnDeath(provider);
            }

            CalculateHealthPercentage();
        }

        public void HealTarget(Transform provider, Transform target, float healAmount)
        {
            if (IsCharacterDead() || _isInvulnerable || _healthPercentage == 100f) { return; }

            StatsManager statsManager = target.GetComponent<StatsManager>();

            if (!statsManager || !statsManager.DoesThisStatTypeExists(StatAttribute.Health))
            {
                Debug.Log("Stats manager is not present on this object or it is not using health", transform);
            }

            float newHealthValue = statsManager.GetStat(StatAttribute.Health).GetCurrentValue() + healAmount;
            statsManager.GetStat(StatAttribute.Health).SetCurrentValue(newHealthValue);

            if (_globalCharacterParameters.CharacterType == CharacterType.Player)
            {
                Actions.OnPlayerHealthValueChanged?.Invoke(
                    GetStat(StatAttribute.Health).GetCurrentValue(), 
                    GetStat(StatAttribute.Health).GetMaxValue(), 
                    HealthInteraction.Heal);
            }

            if (_healVFX) { _healVFX.SetActive(true); }

            DamagePopup.CreateDamagePopup(_popupPrefab, target, healAmount, PopupType.Heal);

            CalculateHealthPercentage();
        }

        private void CalculateHealthPercentage()
        {
            if (!DoesThisStatTypeExists(StatAttribute.Health)) { return; }

            _healthPercentage = Helper.GetPercentage(
                GetStat(StatAttribute.Health).GetCurrentValue(), 
                GetStat(StatAttribute.Health).GetMaxValue(), 100);
        }

        #region OnDeath
        public void OnDeath(Transform killer)
        {
            Debug.Log("On death event");

            if (_defaultController) { PlayOnDeathSound(_defaultController.AudioSource); }

            ScoreGiver scoreGiver = transform.GetComponent<ScoreGiver>();
            scoreGiver.GiveScoreToTarget(killer, scoreGiver.GetScoreData(ScoreRelatedActionName.OnDeath));

            DefaultController defaultController = GetComponent<DefaultController>();
            defaultController.SwitchState(defaultController.Death());

            Actions.OnEnemyDeath?.Invoke();
        }

        public bool IsCharacterDead()
        {
            if (DoesThisStatTypeExists(StatAttribute.Health) && GetStat(StatAttribute.Health).GetCurrentValue() <= 0)
            {
                return true;
            }

            return false;
        }

        public void InstantiateDeathFX()
        {
            if (_deathVFX) { Instantiate(_deathVFX, transform.position, transform.rotation); }

            Destroy(gameObject);
        }
        #endregion

        #region Audio
        public ControllerAudioSettingList GetControllerAudioSetting() { return _controllerAudioSettingList; }

        private void PlayOnDeathSound(AudioSource audioSource)
        {
            if (!_emitsDeathSound) { return; }

            ControllerAudioSetting controllerAudioSetting = ControllerAudioSetting.GetControllerAudioSetting(
                _controllerAudioSettingList.ControllerAudioSettings, 
                RelatedControllerAction.OnDeath);

            AudioHelper.PlayOneShot(audioSource, controllerAudioSetting.GetAudioClip(), controllerAudioSetting.GetVolumeMaxValue());
        }
        #endregion

        #region Editor - On Validate
        void OnValidate()
        {
            Init();
        }
        #endregion
    }
}