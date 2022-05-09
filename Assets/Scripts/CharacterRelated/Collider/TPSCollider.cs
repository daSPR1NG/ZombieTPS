using System.Collections.Generic;
using UnityEngine;

namespace Khynan_Coding
{
    public enum ColliderType
    {
        Unassigned, None, Head, Body, Surface_Wood, Surface_Metal, Surface_Dirt, Surface_Glass // ...
    }

    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class TPSCollider : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private ColliderType _colliderType = ColliderType.Unassigned;

        [Header("AUDIO SETTINGS")]
        [SerializeField] private bool _emitsHitSound = false;
        [SerializeField] private ColliderAudioSettingList _colliderAudioSettingList;

        public ColliderType ColliderType { get => _colliderType; }

        private AudioSource _audioSource;

        void Start() => Init();

        void Init()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayOnHitSound()
        {
            if (!_emitsHitSound) { return; }

            ColliderAudioSetting controllerAudioSetting =
                ColliderAudioSetting.GetControllerAudioSetting(_colliderAudioSettingList.ColliderAudioSettings, ColliderType);

            float pitchOverride = Random.Range(controllerAudioSetting.GetPitchMinValue(), controllerAudioSetting.GetPitchMaxValue());
            AudioHelper.SetPitch(_audioSource, pitchOverride);

            float randomVolume = Random.Range(controllerAudioSetting.GetVolumeMinValue(), controllerAudioSetting.GetVolumeMaxValue());
            AudioHelper.SetVolume(_audioSource, randomVolume);

            AudioHelper.PlaySound(_audioSource, controllerAudioSetting.GetAudioClip());
        }
    }
}