using UnityEngine;
using Lofelt.NiceVibrations;
using deVoid.Utils;

using Sirenix.OdinInspector;
using System;

using TMS.Feedback.Haptics;
using TMS.Feedback.Audio;

namespace TMS.Feedback
{
    /// <summary>
    /// Central manager for all feedback systems including audio and haptics
    /// </summary>
    public class FeedbackManager : IFeedbackManager, IDisposable
    {
        private FeedbackSaveData _feedbackSaveData;

        public bool IsMusicOn => _feedbackSaveData?.isMusicOn ?? true;
        public bool IsSFXOn => _feedbackSaveData?.isSFXOn ?? true;
        public bool IsHapticOn => _feedbackSaveData?.isHapticOn ?? true;

        //Audio
        private AudioSettingsConfigSO _audioSettings;
        private IAudio _audioManager;
        private AudioLibrary _audioLibrary; // Added AudioLibrary reference

        //Haptics
        private HapticSettingsConfigSO _hapticSettings;
        private IHaptics _hapticsManager;

        public FeedbackManager()
        {
            LoadAudio();
            LoadHaptics();

            SmartDebug.Log("FeedbackManager loaded");
            
            Signals.Get<OnDataLoadedSignal>().AddListener(OnDataLoaded);
        }

        private void LoadAudio()
        {
            _audioSettings = Resources.Load<AudioSettingsConfigSO>("AudioSettings");
            _audioLibrary = Resources.Load<AudioLibrary>("AudioLibrary"); // Load the AudioLibrary

            if (_audioLibrary == null)
            {
                SmartDebug.Warning("AudioLibrary could not be loaded from Resources", "FEEDBACK");
            }

            _audioManager ??= new AudioManager(_audioSettings);
            
            SmartDebug.Log("AudioManager", "Loading Audio Settings");
        }

        private void LoadHaptics()
        {
            _hapticSettings = Resources.Load<HapticSettingsConfigSO>("HapticSettings");
            _hapticsManager ??= new HapticsManager(_hapticSettings);
            
            SmartDebug.Log("HapticsManager", "Loading Haptics");
        }

        #region Load and Save
        private void OnDataLoaded()
        {
            _feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData ?? new FeedbackSaveData();

            // Apply loaded settings to managers
            _audioManager.ToggleMusic(_feedbackSaveData.isMusicOn);
            _audioManager.ToggleSFX(_feedbackSaveData.isSFXOn);
            _hapticsManager.ToggleHaptics(_feedbackSaveData.isHapticOn);

            SmartDebug.DevOnly($"Feedback settings loaded - Music: {_feedbackSaveData.isMusicOn}, SFX: {_feedbackSaveData.isSFXOn}, Haptic: {_feedbackSaveData.isHapticOn}", "FEEDBACK");
        }

        public async void Save()
        {
            SaveManager.Instance.SaveData.feedbackSaveData = _feedbackSaveData;
            await SaveManager.Instance.SaveAsync();

            SmartDebug.DevOnly("Feedback settings saved", "FEEDBACK");
        }
        #endregion

        public void Dispose()
        {
            Signals.Get<OnDataLoadedSignal>().RemoveListener(OnDataLoaded);

            if (_audioManager is IDisposable audioDisposable)
            {
                audioDisposable.Dispose();
            }

            if (_hapticsManager is IDisposable hapticsDisposable)
            {
                hapticsDisposable.Dispose();
            }
        }

        #region Audio Controls

        public void ToggleMusic()
        {
            _feedbackSaveData.isMusicOn = !_feedbackSaveData.isMusicOn;
            _audioManager.ToggleMusic(_feedbackSaveData.isMusicOn);
            Save();
        }

        public void ToggleSFX()
        {
            _feedbackSaveData.isSFXOn = !_feedbackSaveData.isSFXOn;
            _audioManager.ToggleSFX(_feedbackSaveData.isSFXOn);
            Save();
        }

        // Get a clip from the AudioLibrary
        private AudioClip GetClipFromType(AudioType audioType)
        {
            if (_audioLibrary == null)
            {
                SmartDebug.Warning("AudioLibrary not loaded", "AUDIO");
                return null;
            }

            AudioClip clip = _audioLibrary.GetAudioClip(audioType);
            if (clip == null)
            {
                SmartDebug.Warning($"No audio clip found for type {audioType}", "AUDIO");
            }

            return clip;
        }

        #region Overloaded Audio Methods

        // Music methods
        public void PlayMusic(AudioClip clip)
        {
            if (_feedbackSaveData.isMusicOn && clip != null)
            {
                _audioManager.Play(clip);
            }
        }

        public void PlayMusic(AudioType audioType)
        {
            AudioClip clip = GetClipFromType(audioType);
            if (_feedbackSaveData.isMusicOn && clip != null)
            {
                _audioManager.Play(clip);
            }
        }

        public void PauseMusic()
        {
            _audioManager.Pause();
        }

        public void ResumeMusic()
        {
            if (_feedbackSaveData.isMusicOn)
            {
                _audioManager.Resume();
            }
        }

        public void StopMusic()
        {
            _audioManager.Stop();
        }

        // SFX methods
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                _audioManager.PlayOneShot(clip, volumeScale);
            }
        }

        public void PlaySFX(AudioType audioType, float volumeScale = 1f)
        {
            AudioClip clip = GetClipFromType(audioType);
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                PlaySFX(clip, volumeScale);
            }
        }

        // SFX at position methods
        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                _audioManager.PlayClipAtPoint(clip, position, volumeScale);
            }
        }

        public void PlaySFXAtPosition(AudioType audioType, Vector3 position, float volumeScale = 1f)
        {
            AudioClip clip = GetClipFromType(audioType);
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                PlaySFXAtPosition(clip, position, volumeScale);
            }
        }

        // UI sound methods
        public void PlayUISound(AudioClip clip, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                _audioManager.PlayOneShot(clip, volumeScale);
            }
        }

        public void PlayUISound(AudioType audioType, float volumeScale = 1f)
        {
            AudioClip clip = GetClipFromType(audioType);
            if (_feedbackSaveData.isSFXOn && clip != null)
            {
                PlayUISound(clip, volumeScale);
            }
        }

        #endregion

        public void SetMasterVolume(float volume)
        {
            _audioManager.SetVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            // This would require extending the IAudio interface
            // For now, we'll use the master volume setter
            SmartDebug.DevOnly("SetMusicVolume not fully implemented, adjust in settings asset", "AUDIO");
        }

        public void SetSFXVolume(float volume)
        {
            // This would require extending the IAudio interface
            // For now, we'll use the master volume setter
            SmartDebug.DevOnly("SetSFXVolume not fully implemented, adjust in settings asset", "AUDIO");
        }

        #endregion

        #region Haptic Calls

        public void ToggleHaptics()
        {
            _feedbackSaveData.isHapticOn = !_feedbackSaveData.isHapticOn;
            _hapticsManager.ToggleHaptics(_feedbackSaveData.isHapticOn);
            Save();
        }

        public void SetHapticStrength(float strength) => _hapticsManager.SetHapticsStrength(strength);

        public void PlayEmphasisHaptics(float amplitude, float frequency)
        {
            if (_feedbackSaveData.isHapticOn)
            {
                _hapticsManager.PlayEmphasisHaptics(amplitude, frequency);
            }
        }

        public void PlayConstantHaptics(float amplitude, float frequency, float duration)
        {
            if (_feedbackSaveData.isHapticOn)
            {
                _hapticsManager.PlayConstantHaptics(amplitude, frequency, duration);
            }
        }

        public void PlayHapticsFromPreset(HapticPatterns.PresetType preset)
        {
            if (_feedbackSaveData.isHapticOn)
            {
                _hapticsManager.PlayHapticsFromPreset(preset);
            }
        }

        public void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset = HapticPatterns.PresetType.Selection)
        {
            if (_feedbackSaveData.isHapticOn)
            {
                _hapticsManager.PlayHapticClip(hapticClip, fallbackPreset);
            }
        }
        #endregion
    }
}