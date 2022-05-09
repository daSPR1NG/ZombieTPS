using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "ScriptableObject/Audio Settings/Controller", fileName = "ASL_Controller_")]
    public class ControllerAudioSettingList : ScriptableObject
    {
        [SerializeField] private List<ControllerAudioSetting> _controllerAudioSettings = new();

        public List<ControllerAudioSetting> ControllerAudioSettings { get => _controllerAudioSettings; }
    }
}