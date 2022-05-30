using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum AudioDataType
    {
        Unassigned, None, Ambient, Music, //...
    }

    [System.Serializable]
    public class AudioData
    {
        public string Name = "new Audio Data";
        public AudioDataType AudioDataType = AudioDataType.Unassigned;
        public AudioSource AudioSource;
        public bool PlayOnAwake = false;
    }

    // NOTES : adds another list of audios (serializable) to better find the audio we want to play in different situation(s).
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<AudioData> _audioDatas = new();

        void Start() => Init();

        void Init()
        {
            CheckIfPlayOnAwake();
        }

        private void CheckIfPlayOnAwake()
        {
            if (_audioDatas.Count == 0) { return; }

            for (int i = 0; i < _audioDatas.Count; i++)
            {
                _audioDatas[i].AudioSource.playOnAwake = _audioDatas[i].PlayOnAwake;
            }
        }

        private AudioData GetAudioData(AudioDataType audioDataType)
        {
            if (_audioDatas.Count == 0) { return null; }

            for (int i = 0; i < _audioDatas.Count; i++)
            {
                if (_audioDatas[i].AudioDataType == audioDataType) { continue; }

                return _audioDatas[i];
            }

            return null;
        }
    }
}