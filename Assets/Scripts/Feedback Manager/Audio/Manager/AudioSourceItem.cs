using System;
using System.Collections;
using UnityEngine;
using TMS.Feedback.Audio;

/// <summary>
/// Component that wraps an AudioSource with pool functionality
/// </summary>
public class AudioSourceItem : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public AudioSource AudioSource => _audioSource;

    private Action<AudioSourceItem> _returnToPool;

    private Coroutine _returnCoroutine;

    public void Initialize(AudioSettingsConfigSO audioSettings, Action<AudioSourceItem> returnAction)
    {
        // The AudioSource is already assigned, just verify it exists
        if (_audioSource == null)
        {
            Debug.LogError($"AudioSource not assigned on {gameObject.name}. Check the prefab configuration.");
        }

        _returnToPool = returnAction;

        // Configure audio source with settings
        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.volume = audioSettings.MasterVolume * audioSettings.SfxVolume;

            // Keep default spatial settings as configured in the prefab
        }
    }

    private void OnDestroy()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }
}