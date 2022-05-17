using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class AmmoGauge : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Image _gaugeFilledImage;
        [SerializeField] private TMP_Text _ammoAmountText;

        [Header("COLOR SETTINGS")]
        [SerializeField] private bool _doesChangeColor = false;
        [SerializeField] private Gradient _colorGradient;

        private List<GameObject> _childrenGos = new();

        private Animator _animator;

        private void OnEnable()
        {
            Actions.OnInitializingWeapon += InitGauge;

            Actions.OnShooting += UpdateGaugeFillAmount;

            Actions.OnReloadStarted += HideAmmoGaugeImage;
            Actions.OnReloadEnded += DisplayAmmoGaugeImage;

            Actions.OnReloadEndedSetWeaponData += UpdateGaugeFillAmount;
        }

        private void OnDisable()
        {
            Actions.OnInitializingWeapon -= InitGauge;

            Actions.OnShooting -= UpdateGaugeFillAmount;

            Actions.OnReloadStarted -= HideAmmoGaugeImage;
            Actions.OnReloadEnded -= DisplayAmmoGaugeImage;

            Actions.OnReloadEndedSetWeaponData -= UpdateGaugeFillAmount;
        }

        private void Awake() => _animator = GetComponent<Animator>();

        void Start() => Init();

        void Init()
        {
            AssignChildren();
        }

        void AssignChildren()
        {
            foreach (Transform item in transform.GetComponentInChildren<Transform>())
            {
                _childrenGos.Add(item.gameObject);
            }
        }

        void InitGauge(Weapon weapon)
        {
            UpdateGaugeFillAmount(weapon);
        }

        private void UpdateGaugeFillAmount(Weapon weapon)
        {
            float current = weapon.GetCurrentAmmo();
            float max = weapon.GetMaxMagAmmo();

            SetGaugeFillAmount(current, max);

            _ammoAmountText.SetText(current.ToString());
            if (_doesChangeColor) { _ammoAmountText.color = _colorGradient.Evaluate(current / max); }

            _animator.Play("AmmoGauge_Text_BreathEffect");
        }

        private void SetGaugeFillAmount(float current, float max)
        {
            _gaugeFilledImage.fillAmount = current / max;
        }

        private void DisplayAmmoGaugeImage()
        {
            for (int i = _childrenGos.Count - 1; i >= 0; i--)
            {
                _childrenGos[i].SetActive(true);
            }
        }

        private void HideAmmoGaugeImage()
        {
            for (int i = _childrenGos.Count - 1; i >= 0; i--)
            {
                _childrenGos[i].SetActive(false);
            }
        }
    }
}