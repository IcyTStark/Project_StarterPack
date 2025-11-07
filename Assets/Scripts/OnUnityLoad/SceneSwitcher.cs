#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SceneSwitcher
{
    [MenuItem("Tools/Scenes/Splash" + " &1")]
    public static void SplashScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Bootstrap.unity");
    }

    [MenuItem("Tools/Scenes/Menu" + " &0")]
    public static void MenuScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene(Application.dataPath + "/_Game_/Scenes/MenuScene.unity");
    }

    [MenuItem("Tools/Scenes/Gameplay" + " &2")]
    public static void GameplayScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/GameScene.unity");
    }

    [MenuItem("Tools/Scenes/Test" + " &`")]
    public static void TestScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/TestScene.unity");
    }

    [MenuItem("Tools/Scenes/Environment" + " &E")]
    public static void OpenEnvironmentScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/Environment.unity");
    }

    [MenuItem("Nukebox/Scenes/Open And Play" + " &5")]
    public static void OpenSplashAndPlay()
    {
        if (EditorApplication.isPlaying)
            return;

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/00_Splash.unity");
        EditorApplication.isPlaying = true;
    }
}
#endif