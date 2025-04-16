using UnityEngine;
using Lofelt.NiceVibrations;

namespace TMS.Feedback
{
    public interface IFeedbackManager
    {
        //Audio


        //======================================================================================================================================================
        //Haptics

        //Toggle Settings
        void ToggleHaptics();

        //Haptic Config Settings
        void SetHapticStrength(float strength);

        //Haptic Methods    
        void PlayHapticsFromPreset(HapticPatterns.PresetType preset);

        void PlayEmphasisHaptics(float amplitude, float frequency);

        void PlayConstantHaptics(float amplitude, float frequency, float duration);

        void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset = HapticPatterns.PresetType.Selection);
    }
}