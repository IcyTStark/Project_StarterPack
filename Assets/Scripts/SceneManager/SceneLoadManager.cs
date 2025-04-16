using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    [Header("Editor bools: ")]
    [SerializeField] private bool shouldLoadGameScene = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnDataLoaded(OnDataLoadedSignal signal)
    {
        if (shouldLoadGameScene)
        {
            LoadGameScene();
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadSplashScene()
    {
        SceneManager.LoadScene(0);
    }
}
