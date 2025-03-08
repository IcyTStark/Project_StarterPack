using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindFirstObjectByType(typeof(T));

                if (instance == null)
                {
                    SetupInstance();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        RemoveDuplicates();
    }

    private void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static void SetupInstance()
    {
        instance = (T)FindFirstObjectByType(typeof(T));

        if (instance == null)
        {
            GameObject gameObject = new();

            gameObject.name = typeof(T).Name;
            instance = gameObject.AddComponent<T>();

            DontDestroyOnLoad(gameObject);
        }
    }
}