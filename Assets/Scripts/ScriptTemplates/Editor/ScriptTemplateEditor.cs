using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptTemplateEditor : Editor
{
    // Simple shortcut menu items for most common templates
    [MenuItem("Assets/Create/Scripts/MonoBehaviour Class %&M", priority = 51)] // Ctrl+Alt+M
    public static void CreateMonoBehaviourClass()
    {
        CreateScriptFromTemplateName("MonoBehaviour Class", "NewMonoBehaviour");
    }

    [MenuItem("Assets/Create/Scripts/MonoBehaviour (with Namespace) %&#M", priority = 52)] // Ctrl+Alt+Shift+M
    public static void CreateMonoBehaviourClassWithNamespace()
    {
        CreateScriptFromTemplateName("MonoBehaviour Class (Namespace)", "NewMonoBehaviour");
    }

    [MenuItem("Assets/Create/Scripts/C# Class %&C", priority = 53)] // Ctrl+Alt+C
    public static void CreateClass()
    {
        CreateScriptFromTemplateName("C# Class", "NewClass");
    }

    [MenuItem("Assets/Create/Scripts/ScriptableObject %&S", priority = 54)] // Ctrl+Alt+S
    public static void CreateScriptableObjectClass()
    {
        CreateScriptFromTemplateName("ScriptableObject", "NewScriptableObject");
    }

    [MenuItem("Assets/Create/Scripts/Interface", priority = 55)]
    public static void CreateInterface()
    {
        CreateScriptFromTemplateName("Interface", "INewInterface");
    }

    [MenuItem("Assets/Create/Scripts/Singleton Class", priority = 56)]
    public static void CreateSingletonClass()
    {
        CreateScriptFromTemplateName("Singleton Class", "NewSingleton");
    }

    // Separator
    [MenuItem("Assets/Create/Scripts/ ", priority = 99)]
    public static void Separator() { }

    // Main template selection window
    [MenuItem("Assets/Create/Scripts/Browse All Templates... %&T", priority = 100)] // Ctrl+Alt+T
    public static void ShowTemplateSelectionWindow()
    {
        ScriptTemplateSelectionWindow.ShowWindow();
    }

    [MenuItem("Assets/Create/Scripts/Manage Templates/Setup Default Templates", priority = 200)]
    public static void SetupDefaultTemplates()
    {
        TemplateCreator.CreateDefaultTemplates();
        EditorUtility.DisplayDialog("Success",
            "Default script templates have been created!\n\nYou can find them in Assets/ScriptTemplates/",
            "OK");
    }

    [MenuItem("Assets/Create/Scripts/Manage Templates/Refresh Templates", priority = 201)]
    public static void RefreshTemplates()
    {
        ScriptTemplateManager.GetAllTemplates();
        Debug.Log("✓ Script templates refreshed!");
        EditorUtility.DisplayDialog("Templates Refreshed",
            "Script templates have been refreshed successfully!",
            "OK");
    }

    [MenuItem("Assets/Create/Scripts/Manage Templates/Open Templates Folder", priority = 202)]
    public static void OpenTemplatesFolder()
    {
        string templatePath = "Assets/ScriptTemplates";

        if (!Directory.Exists(templatePath))
        {
            if (EditorUtility.DisplayDialog("Folder Not Found",
                "Script templates folder doesn't exist. Would you like to create default templates?",
                "Yes", "No"))
            {
                TemplateCreator.CreateDefaultTemplates();
            }
            return;
        }

        Object templateFolder = AssetDatabase.LoadAssetAtPath<Object>(templatePath);
        Selection.activeObject = templateFolder;
        EditorGUIUtility.PingObject(templateFolder);
    }

    [MenuItem("Assets/Create/Scripts/Manage Templates/Clear All Templates", priority = 203)]
    public static void ClearTemplates()
    {
        TemplateCreator.ClearTemplates();
    }

    // Helper method to create scripts from template name
    private static void CreateScriptFromTemplateName(string templateName, string defaultFileName)
    {
        var templates = ScriptTemplateManager.GetAllTemplates();

        // Auto-create templates if none exist
        if (templates.Count == 0)
        {
            Debug.Log("No templates found. Creating default templates...");
            TemplateCreator.CreateDefaultTemplates();
            templates = ScriptTemplateManager.GetAllTemplates();
        }

        ScriptTemplate template = null;

        // Try to find template by name
        foreach (var t in templates)
        {
            if (t.templateName == templateName)
            {
                template = t;
                break;
            }
        }

        if (template == null)
        {
            Debug.LogWarning($"Template '{templateName}' not found. Creating default template.");
            template = CreateDefaultTemplate(templateName, defaultFileName);
        }

        CreateScriptFromTemplate(template, defaultFileName);
    }

    private static ScriptTemplate CreateDefaultTemplate(string templateName, string defaultFileName)
    {
        ScriptTemplate template = ScriptableObject.CreateInstance<ScriptTemplate>();
        template.templateName = templateName;
        template.defaultFileName = defaultFileName;

        // Set default content based on template name
        switch (templateName)
        {
            case "MonoBehaviour Class":
                template.templateContent = @"using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}";
                break;

            case "MonoBehaviour Class (Namespace)":
                template.templateContent = @"using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}";
                template.addToNamespace = true;
                template.namespaceName = "FIO";
                break;

            case "C# Class":
                template.templateContent = @"using System;

[Serializable]
public class #SCRIPTNAME#
{
    
}";
                break;

            case "Interface":
                template.templateContent = @"public interface #SCRIPTNAME#
{
    
}";
                break;

            case "Singleton Class":
                template.templateContent = @"public class #SCRIPTNAME# : Singleton<#SCRIPTNAME#>
{
    public override void Awake()
    {
        base.Awake();
    }
}";
                break;

            case "ScriptableObject":
                template.templateContent = @"using UnityEngine;

[CreateAssetMenu(fileName = ""#SCRIPTNAME#"", menuName = ""ScriptableObjects/#SCRIPTNAME#"")]
public class #SCRIPTNAME# : ScriptableObject
{
    
}";
                break;
        }

        return template;
    }

    public static void CreateScriptFromTemplate(ScriptTemplate template, string defaultFileName = null)
    {
        string fileName = defaultFileName ?? template.defaultFileName;
        string selectedPath = GetSelectedFolderPath();
        string finalFileName = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(selectedPath, fileName + template.fileExtension));
        string className = Path.GetFileNameWithoutExtension(finalFileName);

        ScriptTemplateManager.CreateScriptFromTemplate(template, className, finalFileName);

        // Show success message
        Debug.Log($"✓ Created {template.templateName}: {className}{template.fileExtension}");
    }

    private static string GetSelectedFolderPath()
    {
        string path = "Assets";

        // Try to get active folder using reflection
        try
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (getActiveFolderPath != null)
            {
                string activePath = (string)getActiveFolderPath.Invoke(null, null);
                if (!string.IsNullOrEmpty(activePath))
                {
                    return activePath;
                }
            }
        }
        catch
        {
            // Fallback to manual detection
        }

        // Check selected objects
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(assetPath))
            {
                path = assetPath;
                break;
            }
            else if (File.Exists(assetPath))
            {
                path = Path.GetDirectoryName(assetPath);
                break;
            }
        }

        return path;
    }
}