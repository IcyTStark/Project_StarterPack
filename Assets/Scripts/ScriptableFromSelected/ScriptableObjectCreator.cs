using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System;

public class ScriptableObjectCreator
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Scriptable Object from Selected", false, 51)]
    public static void CreateScriptableObject()
    {
        // Get the currently selected asset
        var selected = Selection.activeObject;

        if (selected == null || !(selected is MonoScript))
        {
            EditorUtility.DisplayDialog("Error", "Please select a valid C# script that extends ScriptableObject.", "OK");
            return;
        }

        MonoScript script = (MonoScript)selected;
        Type scriptType = script.GetClass();

        // Check if the selected script is a ScriptableObject
        if (scriptType == null || !typeof(ScriptableObject).IsAssignableFrom(scriptType) || scriptType.IsAbstract)
        {
            EditorUtility.DisplayDialog("Error", "The selected script doesn't extend ScriptableObject or is abstract.", "OK");
            return;
        }

        // Create the scriptable object instance
        ScriptableObject instance = ScriptableObject.CreateInstance(scriptType);

        // Determine save path - default to script location
        string path = AssetDatabase.GetAssetPath(selected);
        if (!string.IsNullOrEmpty(path))
        {
            path = Path.GetDirectoryName(path);
        }
        else
        {
            path = "Assets";
        }

        // Get class name for the file
        string className = scriptType.Name;

        // Create a unique file name
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, className + ".asset"));

        // Create and save the asset
        AssetDatabase.CreateAsset(instance, assetPathAndName);
        AssetDatabase.SaveAssets();

        // Highlight the created asset in the project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = instance;
    }

    // This validates the menu item
    [MenuItem("Assets/Create/Scriptable Object from Selected", true)]
    public static bool ValidateCreateScriptableObject()
    {
        var selected = Selection.activeObject;

        if (selected == null || !(selected is MonoScript))
            return false;

        MonoScript script = (MonoScript)selected;
        Type scriptType = script.GetClass();

        return scriptType != null &&
               typeof(ScriptableObject).IsAssignableFrom(scriptType) &&
               !scriptType.IsAbstract;
    }
#endif
}