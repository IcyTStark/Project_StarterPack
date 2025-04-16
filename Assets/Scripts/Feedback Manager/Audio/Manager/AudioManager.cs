using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMS.Feedback.Audio
{
    public class AudioManager : IAudio, IDisposable
    {
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        private AudioSettingsConfigSO audioSettings;

        public AudioManager(AudioSettingsConfigSO inAudioSettings)
        {
            audioSettings = inAudioSettings;
        }

        public void SetVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void ToggleMusic(bool musicState)
        {
            throw new NotImplementedException();
        }

        public void ToggleSFX(bool sfxState)
        {
            throw new NotImplementedException();
        }

        public void Play(AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1)
        {
            throw new NotImplementedException();
        }

        public void PlayDelayed(AudioClip clip, float delayTime)
        {
            throw new NotImplementedException();
        }

        public void PlayOneShot(AudioClip clip, float volumeScale = 1)
        {
            throw new NotImplementedException();
        }

        public void PlayScheduled(float scheduledTime)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}