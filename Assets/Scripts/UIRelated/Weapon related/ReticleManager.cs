using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public enum ReticleType
    {
        Unassigned, None, Default, Ally, Enemy, Interaction, Reload, //...
    }

    public class ReticleManager : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private GameObject _uiReticleGO;

        [Header("RELOAD")]
        [SerializeField] private GameObject _reloadIconHolderGO;
        [SerializeField] private GameObject _dotIconHolderGO;
        [SerializeField] private GameObject _reticleComponentsHolderGO;

        [Header("HIT MARKER")]
        [SerializeField] private float _hitMarkerLifetime = .25f;
        [SerializeField] private GameObject _hitMarkerGO;
        [SerializeField] private Color _hitMarkerColor;
        private float _currentLifetimeTimer = 0;

        [Header("RETICLE SETTINGS")]
        [SerializeField] private List<ReticleSetting> reticleSettings = new();

        private ReticleSetting _currentUsedReticleSetting;

        [System.Serializable]
        public class ReticleSetting
        {
            // 02/05/2022 : look variables have been commented, the sprite is no longer changed at runtime.
            // Check Reticle Component Handler script to recall it in details.

            [SerializeField] private string _name = "New Setting";
            [SerializeField] private ReticleType _type = ReticleType.Unassigned;
            [SerializeField] private Color color = Color.white;
            [Range(5f, 100f)][SerializeField] private float height = 25;
            [Range(5f, 100f)] [SerializeField] private float width = 25;
            [Range(0f, 360f)] [SerializeField] private float rotation = 0;

            public ReticleType Type { get => _type; }
            public Color Color { get => color; }
            public float Height { get => height; }
            public float Width { get => width; }
            public float Rotation { get => rotation; }
        }

        private void OnEnable()
        {
            Actions.OnGamePaused += SetDefaultReticle;

            Actions.OnAimingInteractable += SetContextualReticle;

            Actions.OnReloadStarted += DisplayReloadReticle;

            Actions.OnReloadStartedSetWeaponData += DisplayReloadFeedback;

            Actions.OnReloadEnded += HideReloadFeedback;
            Actions.OnReloadEnded += HideReloadReticle;

            Actions.OnHittingValidTarget += DisplayHitMarker;

            Actions.OnAiming += DisplayCrosshairReticleComponents;
            Actions.OnCancelingAim += HideCrosshairReticleComponents; 
        }

        private void OnDisable()
        {
            Actions.OnGamePaused -= SetDefaultReticle;

            Actions.OnAimingInteractable -= SetContextualReticle;

            Actions.OnReloadStarted -= DisplayReloadReticle;

            Actions.OnReloadStartedSetWeaponData -= DisplayReloadFeedback;

            Actions.OnReloadEnded -= HideReloadFeedback;
            Actions.OnReloadEnded -= HideReloadReticle;

            Actions.OnHittingValidTarget -= DisplayHitMarker;

            Actions.OnAiming -= DisplayCrosshairReticleComponents;
            Actions.OnCancelingAim -= HideCrosshairReticleComponents;
        }

        void Start() => Init();

        void Update() => ProcessHitMarkerLifetime();

        void Init()
        {
            SetDefaultReticle();

            HideHitMarker();
            HideReloadFeedback();
            HideReloadReticle();

            //HideCrosshairReticleComponents();
        }

        private void SetDefaultReticle()
        {
            if (_currentUsedReticleSetting == reticleSettings[0]) { return; }

            SetReticle(reticleSettings[0]);
        }

        private void SetReticle(ReticleSetting reticleSetting)
        {
            Image dotIcon = _dotIconHolderGO.GetComponent<Image>();
            dotIcon.color = reticleSetting.Color;

            ReticleComponentsHandler reticleComponentsHandler = _reticleComponentsHolderGO.GetComponent<ReticleComponentsHandler>();
            reticleComponentsHandler.SetComponentsColor(reticleSetting.Color);

            _currentUsedReticleSetting = reticleSetting;
        }

        private void SetContextualReticle(CharacterType characterType)
        {
            //Debug.Log("Set contextual reticle");

            switch (characterType)
            {
                case CharacterType.Unassigned:
                    SetDefaultReticle();
                    //Debug.Log("Set contextual reticle : CharacterType.Unassigned");
                    break;
                case CharacterType.IA_Enemy:
                    SetReticle(GetReticleSetting(ReticleType.Enemy));
                    //Debug.Log("Set contextual reticle : CharacterType.IA_Enemy");
                    break;
                case CharacterType.IA_Ally:
                    SetReticle(GetReticleSetting(ReticleType.Ally));
                    //Debug.Log("Set contextual reticle : CharacterType.IA_Ally");
                    break;
                case CharacterType.InteractiveObject:
                    SetReticle(GetReticleSetting(ReticleType.Interaction));
                    //Debug.Log("Set contextual reticle : CharacterType.InteractiveObject");
                    break;
            }
        }

        private ReticleSetting GetReticleSetting(ReticleType reticleType)
        {
            if (reticleSettings.Count == 0) { return null; }

            for (int i = 0; i < reticleSettings.Count; i++)
            {
                if (reticleSettings[i].Type != reticleType) { continue; }

                return reticleSettings[i];
            }

            return null;
        }

        #region Hit marker
        private void DisplayHitMarker()
        {
            _hitMarkerGO.SetActive(true);
            _currentLifetimeTimer = _hitMarkerLifetime;

            Image hitMarkerIcon = _hitMarkerGO.transform.GetChild(0).GetComponent<Image>();
            hitMarkerIcon.color = _hitMarkerColor;
        }

        private void HideHitMarker()
        {
            _hitMarkerGO.SetActive(false);
        }

        private void ProcessHitMarkerLifetime()
        {
            if (!_hitMarkerGO.activeInHierarchy) { return; }

            _currentLifetimeTimer -= Time.deltaTime;

            if (_currentLifetimeTimer <= 0)
            {
                _currentLifetimeTimer = 0;
                HideHitMarker();
            }
        }
        #endregion  

        #region Reload reticle
        private void DisplayReloadFeedback(Weapon weapon)
        {
            HideHitMarker();

            _uiReticleGO.SetActive(true);

            ReticleReload reticleReload = _uiReticleGO.GetComponent<ReticleReload>();
            reticleReload.Init(weapon.GetReloadingTimer());
        }

        private void HideReloadFeedback()
        {
            _uiReticleGO.SetActive(false);
        }

        private void DisplayReloadReticle()
        {
            _reloadIconHolderGO.SetActive(true);

            HideCrosshairReticleComponents();
            _dotIconHolderGO.SetActive(false);
        }

        private void HideReloadReticle()
        {
            _reloadIconHolderGO.SetActive(false);

            //DisplayCrosshairReticleComponents();
            _dotIconHolderGO.SetActive(true);
        }
        #endregion

        #region Crosshair reticle components - Display / Hide
        private void DisplayCrosshairReticleComponents()
        {
            _reticleComponentsHolderGO.SetActive(true);
        }

        private void HideCrosshairReticleComponents()
        {
            _reticleComponentsHolderGO.SetActive(false);
        }
        #endregion
    }
}