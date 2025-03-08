using System;
using UnityEngine;

[Serializable] 
public class AudioSettings
{
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiSoundVolume = 1f;
    public float crossfadeDuration = 1f;

    [Header("Audio Clips")]
    public AudioClip[] sfxClips = new AudioClip[] { };

    [Header("Audio Holder: ")]
    public Transform audioHolder;
}
