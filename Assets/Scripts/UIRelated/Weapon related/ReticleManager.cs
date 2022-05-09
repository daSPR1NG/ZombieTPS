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
        [SerializeField] private Image reticleImage;
        [SerializeField] private GameObject reticleReloadGO;

        [Header("RELOAD")]
        [SerializeField] private GameObject reticleIconHolderGO;
        [SerializeField] private GameObject reticleComponentsHolderGO;

        [Header("HIT MARKER")]
        [SerializeField] private float hitMarkerLifetime = .25f;
        [SerializeField] private GameObject reticleHitMarkerGO;
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
            //[SerializeField] private Sprite look = null;
            [Range(5f, 100f)][SerializeField] private float height = 25;
            [Range(5f, 100f)] [SerializeField] private float width = 25;
            [Range(0f, 360f)] [SerializeField] private float rotation = 0;

            public ReticleType Type { get => _type; }
            public Color Color { get => color; }
            //public Sprite Look { get => look; private set => look = value; }
            public float Height { get => height; }
            public float Width { get => width; }
            public float Rotation { get => rotation; }

            //public void SetLook(Sprite newSprite)
            //{
            //    Look = newSprite;
            //}
        }

        private void OnEnable()
        {
            Actions.OnGamePaused += SetDefaultReticle;

            Actions.OnAimingInteractable += SetContextualReticle;
            //Actions.OnEquippingWeapon += SetAimingReticleLookOnEquipping; // old method

            //Actions.OnReloadStarted += SetReloadReticle; // old method
            Actions.OnReloadStarted += DisplayReloadReticle;

            Actions.OnReloadStartedSetWeaponData += DisplayReloadFeedback;

            Actions.OnReloadEnded += HideReloadFeedback;
            Actions.OnReloadEnded += HideReloadReticle;

            Actions.OnHittingValidTarget += DisplayHitMarker;
        }

        private void OnDisable()
        {
            Actions.OnGamePaused -= SetDefaultReticle;

            Actions.OnAimingInteractable -= SetContextualReticle;
            //Actions.OnEquippingWeapon -= SetAimingReticleLookOnEquipping; // old method

            //Actions.OnReloadStarted -= SetReloadReticle; // old method
            Actions.OnReloadStarted -= DisplayReloadReticle;

            Actions.OnReloadStartedSetWeaponData -= DisplayReloadFeedback;

            Actions.OnReloadEnded -= HideReloadFeedback;
            Actions.OnReloadEnded -= HideReloadReticle;

            Actions.OnHittingValidTarget -= DisplayHitMarker;
        }

        void Start() => Init();

        void Update() => ProcessHitMarkerLifetime();

        void Init()
        {
            SetDefaultReticle();

            HideHitMarker();
            HideReloadFeedback();
            HideReloadReticle();
        }

        private void SetDefaultReticle()
        {
            if (_currentUsedReticleSetting == reticleSettings[0]) { return; }

            SetReticle(reticleSettings[0]);
        }

        private void SetReticle(ReticleSetting reticleSetting)
        {
            Transform reticleIconParent = reticleImage.transform.parent;

            // Set sprite // old method
            //reticleImage.sprite = reticleSetting.Look;

            // Set size & rotation // old method
            //RectTransform rect = reticleImage.GetComponent<RectTransform>();
            //rect.sizeDelta = new Vector2(reticleSetting.Height, reticleSetting.Width);
            //rect.localEulerAngles = new(0, 0, reticleSetting.Rotation);

            // Set color // old method
            //reticleImage.color = reticleSetting.Color; 

            Image dotIcon = reticleIconParent.GetChild(1).GetComponent<Image>();
            dotIcon.color = reticleSetting.Color;

            ReticleComponentsHandler reticleComponentsHandler = reticleComponentsHolderGO.GetComponent<ReticleComponentsHandler>();
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

        private void SetAimingReticleLookOnEquipping(Weapon weapon, int index)
        {
            //reticleSettings[0].SetLook(weapon.ReticleSprite);
            //reticleSettings[1].SetLook(weapon.ReticleSprite);
            //reticleSettings[2].SetLook(weapon.ReticleSprite);
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
            reticleHitMarkerGO.SetActive(true);
            _currentLifetimeTimer = hitMarkerLifetime;
        }

        private void HideHitMarker()
        {
            reticleHitMarkerGO.SetActive(false);
        }

        private void ProcessHitMarkerLifetime()
        {
            if (!reticleHitMarkerGO.activeInHierarchy) { return; }

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

            reticleReloadGO.SetActive(true);

            ReticleReload reticleReload = reticleReloadGO.GetComponent<ReticleReload>();
            reticleReload.Init(weapon.GetReloadingTimer());
        }

        private void HideReloadFeedback()
        {
            reticleReloadGO.SetActive(false);
        }

        private void SetReloadReticle()
        {
            Debug.Log("Set reload reticle.");

            if (_currentUsedReticleSetting == GetReticleSetting(ReticleType.Reload)) { return; }
            SetReticle(GetReticleSetting(ReticleType.Reload));
        }

        private void DisplayReloadReticle()
        {
            reticleComponentsHolderGO.SetActive(false);
            reticleIconHolderGO.SetActive(true);
        }

        private void HideReloadReticle()
        {
            reticleIconHolderGO.SetActive(false);
            reticleComponentsHolderGO.SetActive(true);
        }
        #endregion
    }
}