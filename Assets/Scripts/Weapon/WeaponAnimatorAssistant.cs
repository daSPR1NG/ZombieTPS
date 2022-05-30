using UnityEngine;

namespace Khynan_Coding
{
    public class WeaponAnimatorAssistant : MonoBehaviour
    {
        public void PlayReloadStateSound(int index)
        {
            WeaponSystem weaponSystemReference = transform.parent.GetComponentInParent<WeaponSystem>();
            AudioSource audioSource = weaponSystemReference.GetComponent<AudioSource>();
            WeaponAudioSetting weaponAudioSetting;

            // Reload Start - Mid - End
            if (index == 0)
            {
                weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(weaponSystemReference.EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, RelatedWeaponAction.Reload_Start);

                AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
                return;
            }

            if (index == 1)
            {
                weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(weaponSystemReference.EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, RelatedWeaponAction.Reload_Mid);

                AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
                return;
            }

            if (index == 2)
            {
                weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(weaponSystemReference.EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, RelatedWeaponAction.Reload_End);

                AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
                return;
            }

            // Grab Magazine
            if (index == 3)
            {
                weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(weaponSystemReference.EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, RelatedWeaponAction.GrabMagazine);

                AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
                return;
            }

            // Tap On Weapon
            if (index == 4)
            {
                weaponAudioSetting = WeaponAudioSetting.GetWeaponAudioSetting(weaponSystemReference.EquippedWeapon.WeaponAudioSettingList.WeaponAudioSettings, RelatedWeaponAction.TapOnWeapon);

                AudioHelper.PlayOneShot(audioSource, weaponAudioSetting.GetAudioClip(), weaponAudioSetting.GetVolumeMaxValue());
            }
        }

        public void GrabMagazine()
        {

        }

        public void ReleaseMagazine()
        {

        }
    }
}