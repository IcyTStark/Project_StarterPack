using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;

#if UNITY_EDITOR
public class TemplateCreator : Editor
{
    [MenuItem("Tools/Script Templates/Setup Script Templates")]
    public static void CreateDefaultTemplates()
    {
        string templatePath = "Assets/ScriptTemplates";

        // Create directory if it doesn't exist
        if (!Directory.Exists(templatePath))
        {
            Directory.CreateDirectory(templatePath);
        }

        // Create MonoBehaviour Template
        CreateTemplate(templatePath, "MonoBehaviour Class", "NewMonoBehaviour", 51,
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    
}");

        // Create MonoBehaviour with Namespace Template
        CreateTemplateWithNamespace(templatePath, "MonoBehaviour Class (Namespace)", "NewMonoBehaviour", 52, "FIO",
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    
}");

        // Create C# Class Template
        CreateTemplate(templatePath, "C# Class", "NewClass", 53,
            @"using System;

[Serializable]
public class #SCRIPTNAME#
{

}");

        // Create Interface Template
        CreateTemplate(templatePath, "Interface", "INewInterface", 54,
            @"public interface #SCRIPTNAME#
{

}");

        // Create Singleton Template
        CreateTemplate(templatePath, "Singleton Class", "NewSingleton", 55,
            @"public class #SCRIPTNAME# : Singleton<#SCRIPTNAME#>
{
    public override void Awake()
    {
        base.Awake();
    }
}");

        // Create ScriptableObject Template
        CreateTemplate(templatePath, "ScriptableObject", "NewScriptableObject", 56,
            @"using UnityEngine;

[CreateAssetMenu(fileName = ""#SCRIPTNAME#"", menuName = ""ScriptableObjects/Parameters/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : ScriptableObject
{

}");

        AssetDatabase.Refresh();

        Debug.Log($"Created {6} script templates in {templatePath}");

        // Select the templates folder
        Object templateFolder = AssetDatabase.LoadAssetAtPath<Object>(templatePath);
        Selection.activeObject = templateFolder;
        EditorGUIUtility.PingObject(templateFolder);
    }

    private static void CreateTemplate(string basePath, string templateName, string defaultFileName, int priority, string content)
    {
        ScriptTemplate template = ScriptableObject.CreateInstance<ScriptTemplate>();
        template.templateName = templateName;
        template.menuPath = "Create/Scripts/";
        template.defaultFileName = defaultFileName;
        template.fileExtension = ".cs";
        template.templateContent = content;
        template.menuPriority = priority;
        template.addToNamespace = false;

        string fileName = $"{templateName.Replace(" ", "").Replace("/", "")}_Template.asset";
        string fullPath = Path.Combine(basePath, fileName);

        AssetDatabase.CreateAsset(template, fullPath);
    }

    private static void CreateTemplateWithNamespace(string basePath, string templateName, string defaultFileName, int priority, string namespaceName, string content)
    {
        ScriptTemplate template = ScriptableObject.CreateInstance<ScriptTemplate>();
        template.templateName = templateName;
        template.menuPath = "Create/Scripts/";
        template.defaultFileName = defaultFileName;
        template.fileExtension = ".cs";
        template.templateContent = content;
        template.menuPriority = priority;
        template.addToNamespace = true;
        template.namespaceName = namespaceName;

        string fileName = $"{templateName.Replace(" ", "").Replace("/", "").Replace("(", "").Replace(")", "")}_Template.asset";
        string fullPath = Path.Combine(basePath, fileName);

        AssetDatabase.CreateAsset(template, fullPath);
    }

    [MenuItem("Tools/Script Templates/Clear Script Templates")]
    public static void ClearTemplates()
    {
        string templatePath = "Assets/ScriptTemplates";

        if (Directory.Exists(templatePath))
        {
            if (EditorUtility.DisplayDialog("Clear Templates",
                "Are you sure you want to delete all script templates?",
                "Yes", "Cancel"))
            {
                Directory.Delete(templatePath, true);
                File.Delete(templatePath + ".meta");
                AssetDatabase.Refresh();
                Debug.Log("Script templates cleared.");
            }
        }
        else
        {
            Debug.Log("No script templates folder found.");
        }
    }
}
#endif