using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class AudioSetting
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private string _name = "[New weapon audio setting]";
        [SerializeField] protected AudioClip _audioClip;

        [Space]
        [Header("PITCH SETTINGS")]
        [Range(0, 3)] [SerializeField] protected float _pitchMinValue = 0.5f;
        [Range(0, 3)] [SerializeField] protected float _pitchMaxValue = 0.5f;

        [Space]
        [Header("VOLUME SETTINGS")]
        [Range(0, 1)] [SerializeField] protected float _volumeMinValue = .65f;
        [Range(0, 1)] [SerializeField] protected float _volumeMaxValue = .65f;

        #region AudioClip - Get / Set
        public AudioClip GetAudioClip()
        {
            return _audioClip;
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            _audioClip = audioClip;
        }
        #endregion

        #region Pitch Settings
        public float GetPitchMinValue()
        {
            return _pitchMinValue;
        }

        public float GetPitchMaxValue()
        {
            return _pitchMaxValue;
        }

        public void SetPitchSettings(float pitchMinValue, float pitchMaxValue)
        {
            _pitchMinValue = pitchMinValue;
            _pitchMaxValue = pitchMaxValue;
        }
        #endregion

        #region Volume Settings
        public float GetVolumeMinValue()
        {
            return _volumeMinValue;
        }

        public float GetVolumeMaxValue()
        {
            return _volumeMaxValue;
        }

        public void SetVolumeSettings(float volumeMinValue, float volumeMaxValue)
        {
            _volumeMinValue = volumeMinValue;
            _volumeMaxValue = volumeMaxValue;
        }
        #endregion
    }
}