using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class ColliderAudioSetting : AudioSetting
    {
        [SerializeField] private ColliderType _colliderType = ColliderType.Unassigned;

        public ColliderAudioSetting(
            AudioClip audioClip,
            ColliderType colliderType, 
            float pitchMinValue, float pitchMaxValue,
            float volumeMinValue, float volumeMaxValue)
        {
            _audioClip = audioClip;
            _colliderType = colliderType;

            _pitchMinValue = pitchMinValue;
            _pitchMaxValue = pitchMaxValue;

            _volumeMinValue = volumeMinValue;
            _volumeMaxValue = volumeMaxValue;
        }

        public static ColliderAudioSetting GetControllerAudioSetting(List<ColliderAudioSetting> audioSettings, ColliderType colliderType)
        {
            if (audioSettings.Count == 0) { new ColliderAudioSetting(colliderType); }

            for (int i = 0; i < audioSettings.Count; i++)
            {
                if (audioSettings[i].GetColliderType() == colliderType)
                {
                    return audioSettings[i];
                }
            }

            return new ColliderAudioSetting(colliderType);
        }

        private ColliderType GetColliderType()
        {
            return _colliderType;
        }

        #region Constructors
        public ColliderAudioSetting(ColliderType colliderType) : this(null, colliderType, 1, 1, 1, 1) { }
        #endregion
    }
}