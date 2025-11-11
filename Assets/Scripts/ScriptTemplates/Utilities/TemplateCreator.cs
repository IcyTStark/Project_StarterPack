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

        // Create MonoBehaviour Template (with common methods)
        CreateTemplate(templatePath, "MonoBehaviour Class", "NewMonoBehaviour", 51,
            @"using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}");

        // Create MonoBehaviour with Namespace Template
        CreateTemplateWithNamespace(templatePath, "MonoBehaviour Class (Namespace)", "NewMonoBehaviour", 52, "FIO",
            @"using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
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
    protected override void Awake()
    {
        base.Awake();
    }
}");

        // Create ScriptableObject Template
        CreateTemplate(templatePath, "ScriptableObject", "NewScriptableObject", 56,
            @"using UnityEngine;

[CreateAssetMenu(fileName = ""#SCRIPTNAME#"", menuName = ""ScriptableObjects/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : ScriptableObject
{
    
}");

        // Create Editor Script Template
        CreateTemplate(templatePath, "Editor Script", "NewEditor", 57,
            @"using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(#SCRIPTNAME#))]
public class #SCRIPTNAME#Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        #SCRIPTNAME# script = (#SCRIPTNAME#)target;
        
        // Add custom inspector GUI here
    }
}");

        // Create Static Utility Class Template
        CreateTemplate(templatePath, "Static Utility Class", "NewUtility", 58,
            @"using UnityEngine;

public static class #SCRIPTNAME#
{
    
}");

        // Create Enum Template
        CreateTemplate(templatePath, "Enum", "NewEnum", 59,
            @"public enum #SCRIPTNAME#
{
    None = 0,
}");

        // Create Struct Template
        CreateTemplate(templatePath, "Struct", "NewStruct", 60,
            @"using System;

[Serializable]
public struct #SCRIPTNAME#
{
    
}");

        AssetDatabase.Refresh();

        Debug.Log($"✓ Created {10} script templates in {templatePath}");

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
                "Are you sure you want to delete all script templates?\n\nThis action cannot be undone.",
                "Yes, Delete All", "Cancel"))
            {
                Directory.Delete(templatePath, true);
                if (File.Exists(templatePath + ".meta"))
                {
                    File.Delete(templatePath + ".meta");
                }
                AssetDatabase.Refresh();
                Debug.Log("✓ Script templates cleared.");

                EditorUtility.DisplayDialog("Templates Cleared",
                    "All script templates have been deleted.",
                    "OK");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No Templates",
                "No script templates folder found.",
                "OK");
        }
    }

    [MenuItem("Tools/Script Templates/Add Custom Template")]
    public static void CreateCustomTemplate()
    {
        string templatePath = "Assets/ScriptTemplates";

        if (!Directory.Exists(templatePath))
        {
            if (EditorUtility.DisplayDialog("Templates Folder Missing",
                "Script templates folder doesn't exist. Create it first?",
                "Yes", "Cancel"))
            {
                CreateDefaultTemplates();
            }
            return;
        }

        // Create a blank custom template
        ScriptTemplate template = ScriptableObject.CreateInstance<ScriptTemplate>();
        template.templateName = "Custom Template";
        template.menuPath = "Create/Scripts/";
        template.defaultFileName = "NewScript";
        template.fileExtension = ".cs";
        template.templateContent = @"using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    
}";
        template.menuPriority = 100;
        template.addToNamespace = false;

        string fileName = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(templatePath, "CustomTemplate.asset"));

        AssetDatabase.CreateAsset(template, fileName);
        AssetDatabase.Refresh();

        // Select and highlight the new template
        Selection.activeObject = template;
        EditorGUIUtility.PingObject(template);

        Debug.Log($"✓ Created custom template: {fileName}");
        EditorUtility.DisplayDialog("Custom Template Created",
            "A new custom template has been created.\n\nEdit it in the Inspector to customize the template content.",
            "OK");
    }
}
#endif