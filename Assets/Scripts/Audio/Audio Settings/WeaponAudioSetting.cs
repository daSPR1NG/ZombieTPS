using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum RelatedWeaponAction
    {
        Unassigned, 
        Shoot, 
        Reload_Start, Reload_Mid, Reload_End, 
        MagEmpty, NoAmmoLeft,
        Swap,
        Aim, UnAim,
        GrabMagazine, TapOnWeapon
    }

    [System.Serializable]
    public class WeaponAudioSetting : AudioSetting
    {
        [SerializeField] private RelatedWeaponAction _relatedWeaponAction = RelatedWeaponAction.Unassigned;

        public WeaponAudioSetting(
            AudioClip audioClip,
            RelatedWeaponAction relatedWeaponAction,
            float pitchMinValue, float pitchMaxValue, 
            float volumeMinValue, float volumeMaxValue)
        {
            _audioClip = audioClip;
            _relatedWeaponAction = relatedWeaponAction;

            _pitchMinValue = pitchMinValue;
            _pitchMaxValue = pitchMaxValue;

            _volumeMinValue = volumeMinValue;
            _volumeMaxValue = volumeMaxValue;
        }

        public static WeaponAudioSetting GetWeaponAudioSetting(List<WeaponAudioSetting> audioSettings, RelatedWeaponAction relatedWeaponAction)
        {
            if (audioSettings.Count == 0) { new WeaponAudioSetting(relatedWeaponAction); }

            for (int i = 0; i < audioSettings.Count; i++)
            {
                if (audioSettings[i].GetRelatedWeaponAction() == relatedWeaponAction)
                {
                    return audioSettings[i];
                }
            }

            return new WeaponAudioSetting(relatedWeaponAction);
        }

        private RelatedWeaponAction GetRelatedWeaponAction()
        {
            return _relatedWeaponAction;
        }

        #region Constructors
        public WeaponAudioSetting(RelatedWeaponAction relatedWeaponAction) : this (null, relatedWeaponAction, 1, 1, 1, 1) { }
        #endregion
    }
}