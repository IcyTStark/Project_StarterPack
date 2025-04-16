using System;
using TMS.ObjectPoolSystem;
using UnityEngine;

public class AudioSourcePool<T> : ObjectPoolBase<T> where T : MonoBehaviour
{
    public AudioSourcePool(T prefab, bool collectionCheck = true, int defaultCapacity = 20, int maxPoolCapacity = 100) : base(prefab, collectionCheck, defaultCapacity, maxPoolCapacity)
    {

    }

    public override T CreateObject()
    {
        return UnityEngine.Object.Instantiate(_prefab);
    }

    public override void GetObjectFromPool(T pooledObject)
    {
        base.GetObjectFromPool(pooledObject);
        AudioSource audioSource = pooledObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public override void ReturnObject(T pooledObject)
    {
        base.ReturnObject(pooledObject);
        AudioSource audioSource = pooledObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}