using UnityEngine;

namespace Khynan_Coding
{
    public class WeaponAnimatorAssistant : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private Transform _characterTransform;
        GameObject _weaponMagazinePf;
        GameObject _grabbedWeaponMagazinePf;

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
            // Grab magazine on weapon.
            WeaponSystem weaponSystem = _characterTransform.GetComponent<WeaponSystem>();
            _weaponMagazinePf = weaponSystem.GetEquippedWeaponWeaponHelper().transform.GetChild(0).gameObject;

            // Grab left hand reference to grab the magazine attached to it.
            RigBuilderHelper rigBuilderHelper = GetComponentInParent<RigBuilderHelper>();
            GameObject leftHandTip = rigBuilderHelper.GetRigData(RigBodyPart.L_Hand).GetTwoBoneIKConstraint().data.tip.gameObject;
            _grabbedWeaponMagazinePf = leftHandTip.transform.GetChild(leftHandTip.transform.childCount - 1).gameObject;

            _weaponMagazinePf.SetActive(false);
            _grabbedWeaponMagazinePf.SetActive(true);
        }

        public void ReleaseMagazine()
        {
            // Instantiate one new mag here and
            // add a lifetime script to it

            GameObject releasedMagazine = Instantiate(
                _grabbedWeaponMagazinePf, 
                _grabbedWeaponMagazinePf.transform.position,
                _grabbedWeaponMagazinePf.transform.rotation);

            _grabbedWeaponMagazinePf.SetActive(false);

            Rigidbody rb = releasedMagazine.AddComponent<Rigidbody>();
            rb.useGravity = true;

            Lifetime lifetimeComponent = releasedMagazine.AddComponent<Lifetime>();
            lifetimeComponent.SetLifeTime(5f);
            lifetimeComponent.SetDestroyState(true);
        }

        public void ReActivateGrabbedWeapon() { _grabbedWeaponMagazinePf.SetActive(true); }


        public void ReActiveWeaponMagazine() { _weaponMagazinePf.SetActive(true); }
    }
}