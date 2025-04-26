using System;
using TMS.Feedback.Audio;
using TMS.ObjectPoolSystem;
using UnityEngine;

public class AudioSourceSFXPool : ObjectPoolBase<AudioSourceItem>
{
    private AudioSettingsConfigSO _audioSettings;   
    private Transform _parentObject;

    public AudioSourceSFXPool(AudioSourceItem prefab, bool collectionCheck = true, int defaultCapacity = 20, int maxPoolCapacity = 100) 
        : base(prefab, collectionCheck, defaultCapacity, maxPoolCapacity)
    {

    }

    public AudioSourceSFXPool(AudioSourceItem prefab, AudioSettingsConfigSO audioSettings, Transform parentObject, bool collectionCheck = true, int defaultCapacity = 20, int maxPoolCapacity = 100)
        : base(prefab, collectionCheck, defaultCapacity, maxPoolCapacity)
    {
        _audioSettings = audioSettings;
        _parentObject = parentObject;
    }

    public override AudioSourceItem CreateObject()
    {
        AudioSourceItem audioSourceInstance = GameObject.Instantiate(_prefab);
        audioSourceInstance.Initialize(_audioSettings);
        audioSourceInstance.AudioSourcePool = this;
        audioSourceInstance.transform.SetParent(_parentObject);
        return audioSourceInstance;
    }

    public override void GetObjectFromPool(AudioSourceItem pooledObject)
    {
        base.GetObjectFromPool(pooledObject);
    }

    public override void ReturnObject(AudioSourceItem pooledObject)
    {
        base.ReturnObject(pooledObject);
    }
}