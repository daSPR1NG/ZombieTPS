using UnityEngine;

namespace Khynan_Coding
{
    public class IAStatsManager : StatsManager
    {
        [Header("HIT EFFECT")]
        [SerializeField] private bool _usesHitEffect = false;
        [SerializeField] private GameObject _hitEffectRendererGo;
        [SerializeField] private Material _hitEffectMaterial;
        [SerializeField] private float _displayDuration = .05f;
        private float _currentDisplayTimer;

        [Header("UI HEALTHBAR")]
        [SerializeField] private UIEnemyHealthBar _enemyHealthBar;

        //[Header("ELEMENTAL EFFECTS")]
        //[SerializeField] private GameObject _fireFXObject;
        //[SerializeField] private GameObject _frostFXObject;

        private RandomizeAspect _randomizeAspect;

        protected override void Update()
        {
            base.Update();
            ProcessHitEffectDisplayDuration();
        }

        protected override void Init()
        {
            base.Init();

            _randomizeAspect = GetComponent<RandomizeAspect>();

            _currentDisplayTimer = _displayDuration;
            HideHitEffectRenderer();

            _enemyHealthBar.InitHealthBarFill(
                GetStat(StatAttribute.Health).GetCurrentValue(),
                GetStat(StatAttribute.Health).GetMaxValue());
        }

        public override void ApplyDamageToTarget(Transform provider, Transform target, float damageAmount)
        {
            base.ApplyDamageToTarget(provider, target, damageAmount);

            // Reset display timer and display the hit effect renderer.
            _currentDisplayTimer = _displayDuration;
            DisplayHitEffectRenderer();

            _enemyHealthBar.SetHealthBar(
                GetStat(StatAttribute.Health).GetCurrentValue(),
                GetStat(StatAttribute.Health).GetMaxValue(),
                HealthInteraction.Damage);
        }

        public override void HealTarget(Transform provider, Transform target, float healAmount)
        {
            base.HealTarget(provider, target, healAmount);

            _enemyHealthBar.SetHealthBar(
                GetStat(StatAttribute.Health).GetCurrentValue(),
                GetStat(StatAttribute.Health).GetMaxValue(),
                HealthInteraction.Heal);
        }

        public override void OnDeath(Transform killer)
        {
            _enemyHealthBar.PlayDeathEffect();

            base.OnDeath(killer);
        }

        #region Hit effect handle
        private void ProcessHitEffectDisplayDuration()
        {
            if (!_usesHitEffect || !_hitEffectRendererGo.activeInHierarchy) { return; }

            _currentDisplayTimer -= Time.deltaTime;

            if (_currentDisplayTimer <= 0)
            {
                _currentDisplayTimer = 0;

                HideHitEffectRenderer();
            }
        }

        private void DisplayHitEffectRenderer()
        {
            _hitEffectRendererGo.SetActive(true);

            _randomizeAspect.SetAspectMaterial(_hitEffectMaterial);
        }

        private void HideHitEffectRenderer()
        {
            _hitEffectRendererGo.SetActive(false);

            _randomizeAspect.SetAspectDefaultMaterials();
        }
        #endregion

        //public GameObject GetFireFXObject() { return _fireFXObject; }
        //public GameObject GetFrostFXObject() { return _frostFXObject; }
    }
}