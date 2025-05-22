using Lofelt.NiceVibrations;

using UnityEngine;

/// <summary>
/// Interface for the feedback manager, providing access to audio and haptic feedback systems
/// </summary>
public interface IFeedbackManager
{
    //Audio
    bool IsMusicOn { get; }
    bool IsSFXOn { get; }
    bool IsHapticOn { get; }

    // Audio Controls
    void ToggleMusic();
    void ToggleSFX();

    // Audio Methods (overloaded to accept either direct AudioClip or AudioType)
    void PlayMusic(AudioClip clip);
    void PlayMusic(AudioType audioType);
    void PauseMusic();
    void ResumeMusic();
    void StopMusic();
    void PlaySFX(AudioClip clip, float volumeScale = 1f);
    void PlaySFX(AudioType audioType, float volumeScale = 1f);
    void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f);
    void PlaySFXAtPosition(AudioType audioType, Vector3 position, float volumeScale = 1f);
    void PlayUISound(AudioClip clip, float volumeScale = 1f);
    void PlayUISound(AudioType audioType, float volumeScale = 1f);

    // Volume Control
    void SetMasterVolume(float volume);
    void SetMusicVolume(float volume);
    void SetSFXVolume(float volume);

    //Haptics
    void ToggleHaptics();
    void SetHapticStrength(float strength);
    void PlayHapticsFromPreset(HapticPatterns.PresetType preset);
    void PlayEmphasisHaptics(float amplitude, float frequency);
    void PlayConstantHaptics(float amplitude, float frequency, float duration);
    void PlayHapticClip(HapticClip hapticClip, HapticPatterns.PresetType fallbackPreset = HapticPatterns.PresetType.Selection);
    void Dispose();
}