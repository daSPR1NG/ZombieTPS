using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class AmmoGauge : MonoBehaviour
    {
        [SerializeField] private Image gaugeFilledImage;

        #region Public References

        #endregion

        private void OnEnable()
        {
            Actions.OnInitializingWeapon += InitGauge;

            Actions.OnShooting += UpdateGaugeFillAmount;

            Actions.OnReloadStarted += HideGaugeImage;
            Actions.OnReloadEnded += DisplayGaugeImage;

            Actions.OnReloadEndedSetWeaponData += UpdateGaugeFillAmount;
        }

        private void OnDisable()
        {
            Actions.OnInitializingWeapon -= InitGauge;

            Actions.OnShooting -= UpdateGaugeFillAmount;

            Actions.OnReloadStarted -= HideGaugeImage;
            Actions.OnReloadEnded -= DisplayGaugeImage;

            Actions.OnReloadEndedSetWeaponData -= UpdateGaugeFillAmount;
        }

        void InitGauge(Weapon weapon)
        {
            UpdateGaugeFillAmount(weapon);
        }

        private void UpdateGaugeFillAmount(Weapon weapon)
        {
            float min = weapon.GetCurrentAmmo();
            float max = weapon.GetMaxMagAmmo();

            SetGaugeFillAmount(min, max);
        }

        private void SetGaugeFillAmount(float min, float max)
        {
            gaugeFilledImage.fillAmount = min / max;
        }

        private void DisplayGaugeImage()
        {
            Transform parent = gaugeFilledImage.transform.parent;

            parent.GetChild(0).gameObject.SetActive(true);
            parent.GetChild(1).gameObject.SetActive(true);
        }

        private void HideGaugeImage()
        {
            Transform parent = gaugeFilledImage.transform.parent;

            parent.GetChild(0).gameObject.SetActive(false);
            parent.GetChild(1).gameObject.SetActive(false);
        }
    }
}