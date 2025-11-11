using UnityEngine;
using System;

public class PersistantSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        SetupInstance();
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        RemoveDuplicates();
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[Singleton] {typeof(T)} instance set to DontDestroyOnLoad.");
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Another instance of {typeof(T)} already exists! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private static void SetupInstance()
    {
        GameObject singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
        _instance = singletonObject.AddComponent<T>();
        DontDestroyOnLoad(singletonObject);
        Debug.Log($"[Singleton] An instance of {typeof(T)} was created automatically.");
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    // Optional: Initialize with custom parameters
    public static T Initialize(Action<T> initAction = null)
    {
        T instance = Instance;
        initAction?.Invoke(instance);
        return instance;
    }

    // Optional: Reset the singleton state (useful for testing)
    public static void Reset()
    {
        _instance = null;
        _applicationIsQuitting = false;
    }
}