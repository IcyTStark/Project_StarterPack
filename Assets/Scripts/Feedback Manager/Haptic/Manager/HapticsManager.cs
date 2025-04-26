using System;
using UnityEngine;

using Lofelt.NiceVibrations;

namespace TMS.Feedback.Haptics
{
    public class HapticsManager : IHaptics, IDisposable
    {
        private bool _isHapticsSupported => DeviceCapabilities.isVersionSupported;
        public bool IsHapticsSupported => _isHapticsSupported;
        private bool _doesSupportAdvancedHaptics => DeviceCapabilities.meetsAdvancedRequirements;
        public bool DoesSupportAdvancedHaptics => _doesSupportAdvancedHaptics;

        private bool _amplitudeModulationSupported = DeviceCapabilities.hasAmplitudeModulation;
        public bool AmplitudeModulationSupported => _amplitudeModulationSupported;

        private bool _frequencyModulationSupported = DeviceCapabilities.hasFrequencyModulation;
        public bool FrequencyModulationSupported => _frequencyModulationSupported;

        public HapticsManager()
        {
            SmartDebug.DevOnly($"Is Haptics Supported: {_isHapticsSupported}", "HAPTICS");
            SmartDebug.DevOnly($"Does support Advanced Haptics: {_doesSupportAdvancedHaptics}", "HAPTICS");
            SmartDebug.DevOnly($"Is Amplitude Modulation Supported: {_amplitudeModulationSupported}", "HAPTICS");
            SmartDebug.DevOnly($"Is Frequency Modulation Supported: {_frequencyModulationSupported}", "HAPTICS");
        }

        public HapticsManager(HapticSettingsConfigSO settings)
        {
            SmartDebug.DevOnly("HapticsManager initialized with settings", "HAPTICS");

            SmartDebug.DevOnly($" Settings: {settings}");

            SetHapticsStrength(settings.HapticStrength);

            HapticController.PlaybackStopped += OnHapticsStopped;
        }

        private void OnHapticsStopped()
        {
            SmartDebug.DevOnly("Haptics playback stopped", "HAPTICS");
            //HapticController.Stop();
        }

        public void Dispose()
        {
            HapticController.PlaybackStopped -= OnHapticsStopped;
        }

        public void ToggleHaptics(bool hapticState)
        {
            HapticController.hapticsEnabled = hapticState;
        }

        public void SetHapticsStrength(float hapticStrength)
        {
            HapticController.outputLevel = hapticStrength;
        }

        public void PlayHapticsFromPreset(HapticPatterns.PresetType presetType)
        {
            if (!_isHapticsSupported)
            {
                SmartDebug.DevOnly("Haptics not supported on this device", "HAPTICS");
                return;
            }

            HapticPatterns.PlayPreset(presetType);
        }

        public void PlayEmphasisHaptics(float amplitude, float frequency)
        {
            if (!_isHapticsSupported || !_amplitudeModulationSupported || !_frequencyModulationSupported)
            {
                if (!_isHapticsSupported)
                    SmartDebug.DevOnly("Haptics not supported on this device", "HAPTICS");

                if (!_amplitudeModulationSupported)
                    SmartDebug.DevOnly("Amplitude modulation not supported on this device", "HAPTICS");

                if (!_frequencyModulationSupported)
                    SmartDebug.DevOnly("Frequency modulation not supported on this device", "HAPTICS");

                return;
            }

            HapticPatterns.PlayEmphasis(amplitude, frequency);
        }

        public void PlayConstantHaptics(float amplitude, float frequency, float duration)
        {
            if (!_isHapticsSupported || !_amplitudeModulationSupported || !_frequencyModulationSupported)
            {
                if (!_isHapticsSupported)
                    SmartDebug.DevOnly("Haptics not supported on this device", "HAPTICS");

                if (!_amplitudeModulationSupported)
                    SmartDebug.DevOnly("Amplitude modulation not supported on this device", "HAPTICS");

                if (!_frequencyModulationSupported)
                    SmartDebug.DevOnly("Frequency modulation not supported on this device", "HAPTICS");

                return;
            }

            HapticPatterns.PlayConstant(amplitude, frequency, duration);
        }

        public void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset = HapticPatterns.PresetType.Selection)
        {
            if (!_isHapticsSupported || !_doesSupportAdvancedHaptics)
            {
                if (!_isHapticsSupported)
                    SmartDebug.DevOnly("Haptics not supported on this device", "HAPTICS");

                if (!_doesSupportAdvancedHaptics)
                    SmartDebug.DevOnly("Advanced haptics not supported on this device", "HAPTICS");

                return;
            }

            HapticController.fallbackPreset = fallbackPreset;
            HapticController.Play(hapticClip);
        }
    }
}