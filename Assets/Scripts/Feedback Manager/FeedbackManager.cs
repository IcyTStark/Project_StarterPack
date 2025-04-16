using UnityEngine;
using EnhancedSignals;
using Lofelt.NiceVibrations;

using Sirenix.OdinInspector;
using System;

using TMS.Feedback.Haptics;
using TMS.Feedback.Audio;

namespace TMS.Feedback
{
    public class FeedbackManager : IFeedbackManager, IDisposable
    {
        //Save Data
        private FeedbackSaveData _feedbackSaveData;

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

        public void Dispose()
        {
            Signals.Get<OnDataLoadedSignal>().RemoveListener(OnDataLoaded);

            if (_hapticsManager is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void ToggleSetting(ref bool setting, bool isHaptics = false)
        {
            setting = !setting;

            if (isHaptics)
                _hapticsManager.ToggleHaptics(_feedbackSaveData.isHapticOn);

            Save();
        }

        #region Haptic Calls

        public void ToggleHaptics() => ToggleSetting(ref _feedbackSaveData.isHapticOn, true);

        private bool IsHapticsOn() => _feedbackSaveData?.isHapticOn ?? true;

        public void SetHapticStrength(float strength) => _hapticsManager.SetHapticsStrength(strength);

        public void PlayEmphasisHaptics(float amplitude, float frequency)
        {
            if (IsHapticsOn())
            {
                _hapticsManager.PlayEmphasisHaptics(amplitude, frequency);
            }
        }

        public void PlayConstantHaptics(float amplitude, float frequency, float duration)
        {
            if (IsHapticsOn())
            {
                _hapticsManager.PlayConstantHaptics(amplitude, frequency, duration);
            }
        }

        public void PlayHapticsFromPreset(HapticPatterns.PresetType preset)
        {
            if (IsHapticsOn())
            {
                _hapticsManager.PlayHapticsFromPreset(preset);
            }
        }

        public void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset = HapticPatterns.PresetType.Selection)
        {
            if (IsHapticsOn())
            {
                _hapticsManager.PlayHapticClip(hapticClip, fallbackPreset);
            }
        }
        #endregion

        #region Load and Save
        private void OnDataLoaded()
        {
            _feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData ?? new FeedbackSaveData();
        }

        public async void Save()
        {
            SaveManager.Instance.SaveData.feedbackSaveData = _feedbackSaveData;
            await SaveManager.Instance.SaveAsync();
        }
        #endregion
    }
}