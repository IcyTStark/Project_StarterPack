using UnityEngine;
using EnhancedSignals;
using Lofelt.NiceVibrations;

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
        //Save Data
        private FeedbackSaveData _feedbackSaveData;

        // Properties for state access
        public bool IsMusicOn => _feedbackSaveData?.isMusicOn ?? true;
        public bool IsSFXOn => _feedbackSaveData?.isSFXOn ?? true;
        public bool IsHapticOn => _feedbackSaveData?.isHapticOn ?? true;

        //Audio
        private AudioSettingsConfigSO _audioSettings;
        private IAudio _audioManager;

        //Haptics
        private HapticSettingsConfigSO _hapticSettings;
        private IHaptics _hapticsManager;

        public FeedbackManager()
        {
            LoadAudio();
            LoadHaptics();

            Signals.Get<OnDataLoadedSignal>().AddListener(OnDataLoaded);
        }

        private void LoadAudio()
        {
            _audioSettings = Resources.Load<AudioSettingsConfigSO>("AudioSettings");
            _audioManager ??= new AudioManager(_audioSettings);
        }

        private void LoadHaptics()
        {
            _hapticSettings = Resources.Load<HapticSettingsConfigSO>("HapticSettings");
            _hapticsManager ??= new HapticsManager(_hapticSettings);
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

        public void PlayMusic(AudioClip clip)
        {
            if (_feedbackSaveData.isMusicOn)
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

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn)
            {
                _audioManager.PlayOneShot(clip, volumeScale);
            }
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn)
            {
                _audioManager.PlayClipAtPoint(clip, position, volumeScale);
            }
        }

        public void PlayUISound(AudioClip clip, float volumeScale = 1f)
        {
            if (_feedbackSaveData.isSFXOn)
            {
                _audioManager.PlayOneShot(clip, volumeScale);
            }
        }

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