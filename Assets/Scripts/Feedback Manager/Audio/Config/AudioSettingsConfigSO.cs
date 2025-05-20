using System;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace TMS.Feedback.Audio
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/Generic/Audio/AudioSettings")]
    public class AudioSettingsConfigSO : ScriptableObject
    {
        [Title("Volume Settings: ")]
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _masterVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _musicVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _sfxVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _uiSoundVolume = 1f;
        [PropertyRange(0f, 1f)] [MaxValue(1)] [SerializeField] private float _crossfadeDuration = 1f;

        [Title("Mixer Groups: ")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _masterMixerGroup;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;
        [SerializeField] private AudioMixerGroup _uiSoundMixerGroup;
        [SerializeField] private AudioMixerSnapshot _defaultSnapshot;

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public float UISoundVolume => _uiSoundVolume;
        public float CrossfadeDuration => _crossfadeDuration;
        public AudioMixer Mixer => _mixer;
        public AudioMixerGroup MasterMixerGroup => _masterMixerGroup;
        public AudioMixerGroup MusicMixerGroup => _musicMixerGroup;
        public AudioMixerGroup SfxMixerGroup => _sfxMixerGroup;
        public AudioMixerGroup UISoundMixerGroup => _uiSoundMixerGroup;
        public AudioMixerSnapshot DefaultSnapshot => _defaultSnapshot;

    }
}