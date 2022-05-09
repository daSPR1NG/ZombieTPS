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
        }

        public override void ApplyDamageToTarget(Transform provider, Transform target, float damageAmount)
        {
            base.ApplyDamageToTarget(provider, target, damageAmount);

            // Reset display timer and display the hit effect renderer.
            _currentDisplayTimer = _displayDuration;
            DisplayHitEffectRenderer();
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
    }
}