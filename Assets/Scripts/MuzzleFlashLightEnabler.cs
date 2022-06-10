using UnityEngine;

namespace Khynan_Coding
{
    public class MuzzleFlashLightEnabler : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private GameObject _muzzleFlashLightGo;
        private UnityEngine.Light _lightComponent;
        private float _muzzleFlashIntensity;

        [Header("SETTINGS")]
        [SerializeField] private float _displayDuration = 0.25f;
        private float _currentDisplayTimer;

        private bool _needToUpdateTimer = false;

        //private void Awake()
        //{
        //    _lightComponent = _muzzleFlashLightGo.GetComponent<Light>();
        //}

        private void OnEnable()
        {
            ResetDisplayTimer();
            _needToUpdateTimer = true;
        }

        private void OnDisable()
        {
            _needToUpdateTimer = false;
        }

        private void Start() => Init();

        void LateUpdate() => ProcessDisplayTimer();

        void Init()
        {
            //_muzzleFlashIntensity = _lightComponent.intensity;
        }

        private void ProcessDisplayTimer()
        {
            if (!_needToUpdateTimer) { return; }

            _currentDisplayTimer -= Time.deltaTime;

            // 100% : 0.025 - 0% : 0
            // 100% : 1 - 0 - 0% : 0
            //float newLightIntensity = Mathf.Lerp(_lightComponent.intensity, 0, Time.deltaTime);

            //SetMuzzleFlashIntensity(newLightIntensity);

            if (_currentDisplayTimer <= 0)
            {
                _currentDisplayTimer = 0;
                _needToUpdateTimer = false;
                _muzzleFlashLightGo.SetActive(false);
            }
        }

        private void ResetDisplayTimer()
        {
            _currentDisplayTimer = _displayDuration;

            if (!_muzzleFlashLightGo.activeInHierarchy) { _muzzleFlashLightGo.SetActive(true); }

            //SetMuzzleFlashIntensity(_muzzleFlashIntensity);
        }

        private void SetMuzzleFlashIntensity(float intensity)
        {
            _lightComponent.intensity = intensity;
        }
    }
}