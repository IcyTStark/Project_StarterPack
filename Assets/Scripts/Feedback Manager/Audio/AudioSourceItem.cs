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

    /// <summary>
    /// The wrapped audio source
    /// </summary>
    public AudioSource AudioSource => _audioSource;

    // Callback for returning to the pool
    private Action<AudioSourceItem> _returnToPool;

    // Tracking of active coroutines
    private Coroutine _returnCoroutine;

    /// <summary>
    /// Initialize the audio source item with settings
    /// </summary>
    /// <param name="audioSettings">Configuration for the audio</param>
    /// <param name="returnAction">Callback to return to pool</param>
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

    /// <summary>
    /// Play an audio clip with auto-return to pool
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volumeScale">Volume scale factor</param>
    public void Play(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || _audioSource == null) return;

        // Stop any existing return coroutine
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        // Set up the audio source
        _audioSource.clip = clip;
        _audioSource.volume *= volumeScale;
        _audioSource.Play();

        // Start coroutine to return to pool when finished
        _returnCoroutine = StartCoroutine(ReturnWhenFinished(clip.length));
    }

    /// <summary>
    /// Play an audio clip at a specific position with 3D spatial audio
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="position">World position for the sound</param>
    /// <param name="volumeScale">Volume scale factor</param>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        transform.position = position;

        if (_audioSource != null)
        {
            // Configure for 3D audio
            _audioSource.spatialBlend = 1.0f;
        }

        Play(clip, volumeScale);
    }

    /// <summary>
    /// Stop playing and return to pool immediately
    /// </summary>
    public void Stop()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        // Stop any running coroutine
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }

        // Return to pool
        ReturnToPool();
    }

    /// <summary>
    /// Coroutine that waits until audio finishes playing, then returns to pool
    /// </summary>
    private IEnumerator ReturnWhenFinished(float delay)
    {
        // Wait for clip duration plus a small buffer
        yield return new WaitForSeconds(delay + 0.1f);

        // Double-check that it's really done playing
        if (_audioSource != null && _audioSource.isPlaying)
        {
            yield return new WaitUntil(() => !_audioSource.isPlaying);
        }

        // Return to pool
        ReturnToPool();
        _returnCoroutine = null;
    }

    /// <summary>
    /// Return this item to the pool
    /// </summary>
    private void ReturnToPool()
    {
        _returnToPool?.Invoke(this);
    }

    /// <summary>
    /// Clean up when destroyed
    /// </summary>
    private void OnDestroy()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
    }
}