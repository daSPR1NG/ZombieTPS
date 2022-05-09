using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "ScriptableObject/Audio Settings/Weapon", fileName = "ASL_Controller_")]
    public class WeaponAudioSettingList : ScriptableObject
    {
        [SerializeField] private List<WeaponAudioSetting> _weaponAudioSettings = new();

        public List<WeaponAudioSetting> WeaponAudioSettings { get => _weaponAudioSettings; }
    }
}