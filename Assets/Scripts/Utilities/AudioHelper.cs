using UnityEngine;

namespace Khynan_Coding
{
    public static class AudioHelper
    {
        #region States - isPlaying / canPlay
        public static bool CanThisAudioSourcePlaySound(AudioSource audioSource)
        {
            if (audioSource.isActiveAndEnabled && audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Loaded)
            {
                return true;
            }

            return false;
        }

        public static bool IsThisAudioSourcePlaying(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region AudioSource - Play
        public static void PlaySound(AudioSource audioSource, AudioClip audioClip)
        {
            if (!audioSource) { return; }

            if (!audioSource.clip) { audioSource.clip = audioClip; }

            audioSource.Play();
        }

        public static void PlayOneShot(AudioSource audioSource, AudioClip audioClip, float volume = 1)
        {
            if (!audioSource) { return; }

            audioSource.PlayOneShot(audioClip, volume);
        }

        public static void PlayWithDelay(AudioSource audioSource, AudioClip audioClip, float delay)
        {
            if (!audioSource) { return; }

            if (!audioSource.clip) { audioSource.clip = audioClip; }

            audioSource.PlayDelayed(delay);
        }
        #endregion

        #region AudioSource - Stop / Un.Pause / Mute
        public static void Stop(AudioSource audioSource)
        {
            if (IsThisAudioSourcePlaying(audioSource))
            {
                audioSource.Stop();
            }
        }

        public static void Pause(AudioSource audioSource)
        {
            if (IsThisAudioSourcePlaying(audioSource) && !audioSource.ignoreListenerPause)
            {
                audioSource.Pause();
            }
        }

        public static void Unpause(AudioSource audioSource)
        {
            audioSource.UnPause();
        }

        public static void Mute(AudioSource audioSource)
        {
            audioSource.mute = true;
        }

        public static void Unmute(AudioSource audioSource)
        {
            audioSource.mute = false;
        }
        #endregion

        #region Getter - Playback position
        public static float GetPlaybackPosition(AudioSource audioSource)
        {
            return audioSource.time;
        }
        #endregion

        #region Setter - Pitch / Volume
        public static void SetPitch(AudioSource audioSource, float value)
        {
            audioSource.pitch = value;
        }

        public static void SetVolume(AudioSource audioSource, float volume)
        {
            audioSource.volume = volume;
        }
        #endregion

        #region Toggle - Play on awake / isLooping states
        public static void SetPlayOnAwake(AudioSource audioSource, bool value)
        {
            audioSource.playOnAwake = value;
        }

        public static void SetLoop(AudioSource audioSource, bool value)
        {
            audioSource.loop = value;
        }
        #endregion
    }
}