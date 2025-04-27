using UnityEngine;
using TMS.ObjectPoolSystem;
using TMS.Feedback.Audio;

public class AudioSourceSFXPool : ObjectPoolBase<AudioSourceItem>
{
    private AudioSettingsConfigSO _audioSettings;
    private Transform _parentTransform;

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

    public override AudioSourceItem CreateObject()
    {
        AudioSourceItem audioSourceItem = GameObject.Instantiate(_prefab, _parentTransform);

        audioSourceItem.Initialize(_audioSettings, Release);

        return audioSourceItem;
    }

    public override void GetObjectFromPool(AudioSourceItem item)
    {
        item.gameObject.SetActive(true);

        if (item.AudioSource != null)
        {
            item.AudioSource.spatialBlend = 0f;
            item.AudioSource.clip = null;

            item.AudioSource.volume = _audioSettings.MasterVolume * _audioSettings.SfxVolume;
        }
    }

    public override void ReturnObject(AudioSourceItem item)
    {
        if (item != null && item.AudioSource != null && item.AudioSource.isPlaying)
        {
            item.AudioSource.Stop();
        }

        if (item != null)
        {
            item.gameObject.SetActive(false);
        }
    }

    public override void DestroyPooledObject(AudioSourceItem item)
    {
        if (item != null)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}