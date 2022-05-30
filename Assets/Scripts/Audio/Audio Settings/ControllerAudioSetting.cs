using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    [System.Serializable]
    public class ControllerAudioSetting : AudioSetting
    {
        [SerializeField] private RelatedControllerAction _relatedControllerAction = RelatedControllerAction.Unassigned;

        public ControllerAudioSetting(
            AudioClip audioClip,
            RelatedControllerAction relatedControllerAction, 
            float pitchMinValue, float pitchMaxValue,
            float volumeMinValue, float volumeMaxValue)
        {
            _audioClip = audioClip;
            _relatedControllerAction = relatedControllerAction;

            _pitchMinValue = pitchMinValue;
            _pitchMaxValue = pitchMaxValue;

            _volumeMinValue = volumeMinValue;
            _volumeMaxValue = volumeMaxValue;
        }

        public static ControllerAudioSetting GetControllerAudioSetting(List<ControllerAudioSetting> audioSettings, RelatedControllerAction relatedControllerAction)
        {
            if (audioSettings.Count == 0) { new ControllerAudioSetting(relatedControllerAction); }

            for (int i = 0; i < audioSettings.Count; i++)
            {
                if (audioSettings[i].GetRelatedControllerAction() == relatedControllerAction)
                {
                    return audioSettings[i];
                }
            }

            return new ControllerAudioSetting(relatedControllerAction);
        }

        private RelatedControllerAction GetRelatedControllerAction()
        {
            return _relatedControllerAction;
        }

        #region Constructors
        public ControllerAudioSetting(RelatedControllerAction relatedControllerAction) : this(null, relatedControllerAction, 1, 1, 1, 1) { }
        #endregion
    }
}