using UnityEngine;
using TMS.ObjectPoolSystem;
using TMS.Feedback.Audio;

/// <summary>
/// Pool for audio source items, extending ObjectPoolBase
/// </summary>
public class AudioSourceSFXPool : ObjectPoolBase<AudioSourceItem>
{
    private AudioSettingsConfigSO _audioSettings;
    private Transform _parentTransform;

    /// <summary>
    /// Constructor that takes configuration settings
    /// </summary>
    /// <param name="prefab">The audio source item prefab</param>
    /// <param name="audioSettings">Audio configuration settings</param>
    /// <param name="parentTransform">Parent transform for pooled objects</param>
    /// <param name="collectionCheck">Whether to perform collection checks</param>
    /// <param name="defaultCapacity">Default pool capacity</param>
    /// <param name="maxPoolCapacity">Maximum pool capacity</param>
    public AudioSourceSFXPool(
        AudioSourceItem prefab,
        AudioSettingsConfigSO audioSettings,
        Transform parentTransform,
        bool collectionCheck = true,
        int defaultCapacity = 10,
        int maxPoolCapacity = 30) : base(prefab, collectionCheck, defaultCapacity, maxPoolCapacity)
    {
        _audioSettings = audioSettings;
        _parentTransform = parentTransform;
    }

    /// <summary>
    /// Creates a new audio source item
    /// </summary>
    /// <returns>A new audio source item</returns>
    public override AudioSourceItem CreateObject()
    {
        // Instantiate the prefab
        AudioSourceItem audioSourceItem = GameObject.Instantiate(_prefab, _parentTransform);

        // Initialize with settings and return-to-pool callback
        audioSourceItem.Initialize(_audioSettings, Release);

        return audioSourceItem;
    }

    /// <summary>
    /// Called when an object is taken from the pool
    /// </summary>
    /// <param name="item">The audio source item to prepare</param>
    public override void GetObjectFromPool(AudioSourceItem item)
    {
        // Enable the game object
        item.gameObject.SetActive(true);

        // Reset spatial blend to 2D by default (will be set to 3D if needed)
        if (item.AudioSource != null)
        {
            item.AudioSource.spatialBlend = 0f;
            item.AudioSource.clip = null;

            // Reset volume based on settings
            item.AudioSource.volume = _audioSettings.MasterVolume * _audioSettings.SfxVolume;
        }
    }

    /// <summary>
    /// Called when an object is returned to the pool
    /// </summary>
    /// <param name="item">The audio source item to reset</param>
    public override void ReturnObject(AudioSourceItem item)
    {
        // Stop audio if playing
        if (item != null && item.AudioSource != null && item.AudioSource.isPlaying)
        {
            item.AudioSource.Stop();
        }

        // Disable the game object
        if (item != null)
        {
            item.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when an object is destroyed (pool cleanup)
    /// </summary>
    /// <param name="item">The audio source item to destroy</param>
    public override void DestroyPooledObject(AudioSourceItem item)
    {
        if (item != null)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}