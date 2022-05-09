using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(menuName = "ScriptableObject/Audio Settings/Collider", fileName = "ASL_Collider_")]
    public class ColliderAudioSettingList : ScriptableObject
    {
        [SerializeField] private List<ColliderAudioSetting> _colliderAudioSettings = new();

        public List<ColliderAudioSetting> ColliderAudioSettings { get => _colliderAudioSettings;}
    }
}