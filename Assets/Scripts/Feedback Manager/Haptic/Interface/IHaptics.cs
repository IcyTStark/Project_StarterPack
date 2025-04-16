using Lofelt.NiceVibrations;

namespace TMS.Feedback.Haptics
{
    public interface IHaptics
    {
        bool IsHapticsSupported { get; }
        bool DoesSupportAdvancedHaptics { get; }
        bool AmplitudeModulationSupported { get; }
        bool FrequencyModulationSupported { get; }

        void ToggleHaptics(bool hapticState);
        void SetHapticsStrength(float hapticStrength);
        void PlayHapticsFromPreset(HapticPatterns.PresetType presetType);
        void PlayEmphasisHaptics(float amplitude, float frequency);
        void PlayConstantHaptics(float amplitude, float frequency, float duration);
        void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset);
    }
}
