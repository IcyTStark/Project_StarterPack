using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMS.ExtendedCoroutine;
using UnityEngine.UIElements;

namespace TMS.Feedback.Audio
{
    /// <summary>
    /// Audio manager implementation that uses AudioSourceSFXPool
    /// </summary>
    public class AudioManager : IAudio, IDisposable
    {
        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        private AudioSettingsConfigSO _audioSettings;

        // Audio sources
        private AudioSource _musicSource;
        private AudioSource _uiSource;
        private List<AudioSourceItem> _activeSfxSources = new List<AudioSourceItem>();

        // Object pooling for SFX audio sources
        private GameObject _audioSourcesContainer;
        private AudioSourceSFXPool _sfxPool;

        // State tracking
        private bool _isMusicOn = true;
        private bool _isSfxOn = true;

        // Current music tracking
        private AudioClip _currentMusicClip;
        private Coroutine _fadeCoroutine;

        public AudioManager(AudioSettingsConfigSO audioSettings)
        {
            _audioSettings = audioSettings;

            CreateAudioHolders();

            CreateMusicSource();

            CreateUISource();

            CreateSFXSource();

            SmartDebug.DevOnly("AudioManager initialized successfully", "AUDIO");
        }

        private void CreateAudioHolders()
        {
            // Create container for audio sources
            _audioSourcesContainer = new GameObject("Audio Sources Container");
            GameObject.DontDestroyOnLoad(_audioSourcesContainer);
        }

        private void CreateMusicSource()
        {
            // Initialize music source
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(_audioSourcesContainer.transform);
            _musicSource = musicObj.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _audioSettings.MusicMixerGroup;
            _musicSource.playOnAwake = true;
            _musicSource.loop = true;
            _musicSource.volume = _audioSettings.MasterVolume * _audioSettings.MusicVolume;
        }

        private void CreateUISource()
        {
            // Initialize UI source
            GameObject uiObj = new GameObject("UI Source");
            uiObj.transform.SetParent(_audioSourcesContainer.transform);
            _uiSource = uiObj.AddComponent<AudioSource>();
            _uiSource.outputAudioMixerGroup = _audioSettings.UISoundMixerGroup;
            _uiSource.playOnAwake = false;
            _uiSource.loop = false;
            _uiSource.volume = _audioSettings.MasterVolume * _audioSettings.UISoundVolume;
        }

        private void CreateSFXSource()
        {
            // Initialize SFX pool using your existing infrastructure
            InitializeSfxPool();
        }

        private void InitializeSfxPool()
        {
            // Create or load AudioSourceItem prefab
            AudioSourceItem prefab = Resources.Load<AudioSourceItem>("AudioSourcePrefab");

            // Create a prefab if none exists
            if (prefab == null)
            {
                GameObject sfxPrefab = new GameObject("SFX Source");
                prefab = sfxPrefab.AddComponent<AudioSourceItem>();
                var sfxAudioSource = sfxPrefab.AddComponent<AudioSource>();
                sfxAudioSource.outputAudioMixerGroup = _audioSettings.SfxMixerGroup;

                // Keep the prefab object inactive and hidden
                sfxPrefab.SetActive(false);
                sfxPrefab.hideFlags = HideFlags.HideAndDontSave;
            }

            GameObject sfxPrefabParent = new GameObject("SFX Parent");
            sfxPrefabParent.transform.SetParent(_audioSourcesContainer.transform);

            // Create SFX audio source pool using your existing ObjectPoolBase
            _sfxPool = new AudioSourceSFXPool(
                prefab,
                _audioSettings,
                sfxPrefabParent.transform,
                true,  // collectionCheck
                10,    // defaultCapacity 
                30     // maxPoolCapacity
            );

            SmartDebug.DevOnly("SFX Pool initialized with AudioSourceSFXPool", "AUDIO");
        }

        public void SetVolume(float volume)
        {
            _musicSource.volume = volume * _audioSettings.MusicVolume;
            _uiSource.volume = volume * _audioSettings.UISoundVolume;

            // Update all active SFX sources
            foreach (var sfxSource in _activeSfxSources)
            {
                if (sfxSource != null && sfxSource.AudioSource != null)
                {
                    sfxSource.AudioSource.volume = volume * _audioSettings.SfxVolume;
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
                foreach (var sfxSource in _activeSfxSources.ToArray())
                {
                    if (sfxSource != null && sfxSource.AudioSource != null && sfxSource.AudioSource.isPlaying)
                    {
                        sfxSource.AudioSource.Stop();
                        _sfxPool.Release(sfxSource);
                    }
                }

                _activeSfxSources.Clear();
            }

            SmartDebug.DevOnly($"SFX toggled: {sfxState}", "AUDIO");
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;

            _currentMusicClip = clip;
               
            // If we're already playing something, crossfade
            if (_musicSource.isPlaying && _musicSource.clip != null && _musicSource.clip != clip)
            {
                if (_fadeCoroutine != null)
                {
                    CoroutineRunner.Instance.StopCoroutine(_fadeCoroutine);
                }

                _fadeCoroutine = CoroutineRunner.Instance.StartCoroutine(
                    CrossfadeMusic(clip, _audioSettings.CrossfadeDuration));
            }
            else
            {
                // Just play normally
                _musicSource.clip = clip;
                _musicSource.Play();
            }

            _isPlaying = true;

            SmartDebug.DevOnly($"Playing music clip: {clip.name}", "AUDIO");
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volumeScale = 1)
        {
            if (clip == null || !_isSfxOn) return;

            // Get an audio source from the pool
            AudioSourceItem source = _sfxPool.Get();

            // Validate the source
            if (source == null || source.AudioSource == null)
            {
                SmartDebug.DevOnly("Failed to get audio source from pool", "AUDIO");
                return;
            }

            // Configure the source
            source.transform.position = position;
            source.AudioSource.clip = clip;
            source.AudioSource.outputAudioMixerGroup ??= _audioSettings.SfxMixerGroup; //If its null then assign the SFX mixer group
            source.AudioSource.volume = _audioSettings.MasterVolume * _audioSettings.SfxVolume * volumeScale;
            source.AudioSource.spatialBlend = 1.0f; // Make it fully 3D
            source.AudioSource.Play();

            // Add to active sources for tracking
            _activeSfxSources.Add(source);

            // Start coroutine to return to pool when done
            CoroutineRunner.Instance.StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));

            SmartDebug.DevOnly($"Playing SFX at point: {clip.name} at {position}", "AUDIO");
        }

        private IEnumerator ReturnToPoolWhenFinished(AudioSourceItem source, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (source != null)
            {
                _activeSfxSources.Remove(source);
                _sfxPool.Release(source);
            }
        }

        public void PlayDelayed(AudioClip clip, float delayTime)
        {
            if (clip == null) return;

            _currentMusicClip = clip;
            _musicSource.clip = clip;
            _musicSource.PlayDelayed(delayTime);
            _isPlaying = true;

            SmartDebug.DevOnly($"Playing music with delay ({delayTime}s): {clip.name}", "AUDIO");
        }

        public void PlayOneShot(AudioClip clip, float volumeScale = 1)
        {
            if (clip == null || !_isSfxOn) return;

            // Get an audio source from the pool
            AudioSourceItem source = _sfxPool.Get();

            // Validate the source
            if (source == null || source.AudioSource == null)
            {
                SmartDebug.DevOnly("Failed to get audio source from pool", "AUDIO");
                return;
            }

            // Configure the source
            source.AudioSource.clip = clip;
            source.AudioSource.outputAudioMixerGroup ??= _audioSettings.SfxMixerGroup; //If its null then assign the SFX mixer group
            source.AudioSource.volume = _audioSettings.MasterVolume * _audioSettings.SfxVolume * volumeScale;
            source.AudioSource.Play();

            // Add to active sources for tracking
            _activeSfxSources.Add(source);

            // Start coroutine to return to pool when done
            CoroutineRunner.Instance.StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));

            SmartDebug.DevOnly($"Playing OneShot: {clip.name}", "AUDIO");
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

        private IEnumerator CrossfadeMusic(AudioClip newClip, float fadeDuration)
        {
            // Create a temporary audio source for the new clip
            GameObject tempObj = new GameObject("Temp Music Source");
            tempObj.transform.SetParent(_audioSourcesContainer.transform);
            AudioSource tempSource = tempObj.AddComponent<AudioSource>();

            // Set up the new source with the same settings as our music source
            tempSource.clip = newClip;
            tempSource.loop = true;
            tempSource.volume = 0;
            tempSource.Play();

            float startVolume = _musicSource.volume;
            float timer = 0;

            // Fade out the old music and fade in the new
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeDuration;

                _musicSource.volume = Mathf.Lerp(startVolume, 0, t);
                tempSource.volume = Mathf.Lerp(0, startVolume, t);

                yield return null;
            }

            // Swap the clips and clean up
            _musicSource.Stop();
            _musicSource.clip = newClip;
            _musicSource.volume = startVolume;
            _musicSource.time = tempSource.time;
            _musicSource.Play();

            GameObject.Destroy(tempObj);
            _fadeCoroutine = null;
        }

        public void Dispose()
        {
            // Stop all current audio
            if (_musicSource != null) _musicSource.Stop();
            if (_uiSource != null) _uiSource.Stop();

            // Return all active SFX sources to the pool
            foreach (var source in _activeSfxSources.ToArray())
            {
                if (source != null && source.AudioSource != null)
                {
                    source.AudioSource.Stop();
                    _sfxPool.Release(source);
                }
            }

            _activeSfxSources.Clear();

            // Destroy container
            if (_audioSourcesContainer != null)
            {
                GameObject.Destroy(_audioSourcesContainer);
            }

            SmartDebug.DevOnly("AudioManager disposed", "AUDIO");
        }
    }
}
