using System;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace TMS.Feedback.Audio
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/Generic/Audio/AudioSettings")]
    public class AudioSettingsConfigSO : ScriptableObject
    {
        [Header("Volume Settings: ")]
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _masterVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _musicVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _sfxVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _uiSoundVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _crossfadeDuration = 1f;

        [SerializeField] private AudioMixerGroup _musicMixerGroup;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public float UISoundVolume => _uiSoundVolume;
        public float CrossfadeDuration => _crossfadeDuration;
    }
}