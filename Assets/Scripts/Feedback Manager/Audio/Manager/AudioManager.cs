using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMS.ObjectPoolSystem;
using TMS.Feedback.Haptics;

namespace TMS.Feedback.Audio
{
    /// <summary>
    /// Implementation of the IAudio interface that handles all game audio functionality
    /// </summary>
    public class AudioManager : IAudio, IDisposable
    {
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        private AudioSettingsConfigSO _audioSettings;

        // Audio sources
        private AudioSource _musicSource;
        private AudioSource _uiSource;
        private List<AudioSource> _activeSfxSources = new List<AudioSource>();

        // Object pooling for SFX audio sources
        private GameObject _audioSourcesContainer;
        private AudioSourceSFXPool _sfxPool;

        // State tracking
        private bool _isMusicOn = true;
        private bool _isSfxOn = true;

        public AudioManager(AudioSettingsConfigSO audioSettings)
        {
            _audioSettings = audioSettings;

            // Create container for audio sources
            _audioSourcesContainer = new GameObject("Audio Sources Container");
            GameObject.DontDestroyOnLoad(_audioSourcesContainer);

            // Initialize music source
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(_audioSourcesContainer.transform);
            _musicSource = musicObj.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.volume = _audioSettings.MasterVolume * _audioSettings.MusicVolume;

            // Initialize UI source
            GameObject uiObj = new GameObject("UI Source");
            uiObj.transform.SetParent(_audioSourcesContainer.transform);
            _uiSource = uiObj.AddComponent<AudioSource>();
            _uiSource.playOnAwake = false;
            _uiSource.loop = false;
            _uiSource.volume = _audioSettings.MasterVolume * _audioSettings.UISoundVolume;

            // Initialize SFX pool
            InitializeSfxPool();

            SmartDebug.DevOnly("AudioManager initialized successfully", "AUDIO");
        }

        private void InitializeSfxPool()
        {
            // Create a prefab for pooled audio sources
            GameObject sfxParentObject = new GameObject("SFX Source Parent");

            AudioSourceItem audioSourcePrefab = Resources.Load<AudioSourceItem>("AudioSourcePrefab");
            _sfxPool = new AudioSourceSFXPool(audioSourcePrefab, _audioSettings, sfxParentObject.transform);
            // Create the pool with our prefab

            SmartDebug.DevOnly("SFX Pool initialized", "AUDIO");
        }

        public void SetVolume(float volume)
        {
            _musicSource.volume = volume * _audioSettings.MusicVolume;
            _uiSource.volume = volume * _audioSettings.UISoundVolume;

            // Update all active SFX sources
            foreach (var sfxSource in _activeSfxSources)
            {
                if (sfxSource != null)
                {
                    sfxSource.volume = volume * _audioSettings.SfxVolume;
                }
            }

            SmartDebug.DevOnly($"Master volume set to {volume}", "AUDIO");
        }

        public void ToggleMusic(bool musicState)
        {
            _isMusicOn = musicState;

            if (!_isMusicOn && _musicSource.isPlaying)
            {
                _musicSource.Pause();
            }
            else if (_isMusicOn && !_musicSource.isPlaying && _isPlaying)
            {
                _musicSource.UnPause();
            }

            SmartDebug.DevOnly($"Music toggled: {musicState}", "AUDIO");
        }

        public void ToggleSFX(bool sfxState)
        {
            _isSfxOn = sfxState;

            // If turning off, stop all active SFX
            if (!_isSfxOn)
            {
                foreach (var sfxSource in _activeSfxSources)
                {
                    if (sfxSource != null && sfxSource.isPlaying)
                    {
                        sfxSource.Stop();
                    }
                }
            }

            SmartDebug.DevOnly($"SFX toggled: {sfxState}", "AUDIO");
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;

            _musicSource.clip = clip;
            _musicSource.Play();
            _isPlaying = true;

            SmartDebug.DevOnly($"Playing music clip: {clip.name}", "AUDIO");
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1)
        {
            if (clip == null || !_isSfxOn) return;

            // Get a pooled audio source
            //AudioSourceComponent sourceComponent = _sfxPool.Get();
            //AudioSource source = sourceComponent.AudioSource;

            //// Configure and play
            //source.transform.position = position;
            //source.clip = clip;
            //source.volume = _audioSettings.MasterVolume * _audioSettings.SfxVolume * volumeScale;
            //source.Play();

            //// Add to active sources
            //_activeSfxSources.Add(source);

            // Start coroutine to return to pool
            //sourceComponent.StartCoroutine(ReturnToPoolWhenFinished(sourceComponent, source, clip.length));

            SmartDebug.DevOnly($"Playing SFX at point: {clip.name}", "AUDIO");
        }

        private IEnumerator ReturnToPoolWhenFinished(AudioSourceComponent component, AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);

            //_activeSfxSources.Remove(source);
            //_sfxPool.Release(component);
        }

        public void PlayDelayed(AudioClip clip, float delayTime)
        {
            if (clip == null) return;

            _musicSource.clip = clip;
            _musicSource.PlayDelayed(delayTime);
            _isPlaying = true;

            SmartDebug.DevOnly($"Playing music with delay ({delayTime}s): {clip.name}", "AUDIO");
        }

        public void PlayOneShot(AudioClip clip, float volumeScale = 1)
        {
            if (clip == null || !_isSfxOn) return;

            _uiSource.PlayOneShot(clip, volumeScale);

            SmartDebug.DevOnly($"Playing UI OneShot: {clip.name}", "AUDIO");
        }

        public void PlayScheduled(float scheduledTime)
        {
            if (_musicSource.clip == null) return;

            _musicSource.PlayScheduled(scheduledTime);
            _isPlaying = true;

            SmartDebug.DevOnly($"Scheduled music to play at {scheduledTime}", "AUDIO");
        }

        public void Pause()
        {
            _musicSource.Pause();
            _isPlaying = false;

            SmartDebug.DevOnly("Music paused", "AUDIO");
        }

        public void Resume()
        {
            if (_isMusicOn)
            {
                _musicSource.UnPause();
                _isPlaying = true;

                SmartDebug.DevOnly("Music resumed", "AUDIO");
            }
        }

        public void Stop()
        {
            _musicSource.Stop();
            _isPlaying = false;

            SmartDebug.DevOnly("Music stopped", "AUDIO");
        }

        public void Dispose()
        {
            // Clean up resources
            if (_audioSourcesContainer != null)
            {
                GameObject.Destroy(_audioSourcesContainer);
            }

            _activeSfxSources.Clear();

            SmartDebug.DevOnly("AudioManager disposed", "AUDIO");
        }
    }

    /// <summary>
    /// Helper component to attach to pooled audio sources
    /// </summary>
    public class AudioSourceComponent : MonoBehaviour
    {
        public AudioSource AudioSource { get; set; }
    }
}