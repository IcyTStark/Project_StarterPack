using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptTemplateEditor : Editor
{
    [MenuItem("Assets/Create/Scripts/Refresh Templates", priority = 1000)]
    public static void RefreshTemplates()
    {
        // Force reload templates
        ScriptTemplateManager.GetAllTemplates();
        Debug.Log("Script templates refreshed!");
    }

    // This method dynamically creates menu items for each template
    [InitializeOnLoadMethod]
    private static void InitializeTemplateMenus()
    {
        EditorApplication.delayCall += CreateTemplateMenus;
    }

    private static void CreateTemplateMenus()
    {
        var templates = ScriptTemplateManager.GetAllTemplates();

        foreach (var template in templates)
        {
            CreateMenuItemForTemplate(template);
        }
    }

    private static void CreateMenuItemForTemplate(ScriptTemplate template)
    {
        string menuPath = $"Assets/{template.menuPath}{template.templateName}";

        // We'll create a generic handler since we can't dynamically create MenuItem attributes
        // Instead, we'll use a context menu approach
    }

    [MenuItem("Assets/Create/Scripts/MonoBehaviour Class", priority = 51)]
    public static void CreateMonoBehaviourClass()
    {
        CreateScriptFromTemplateName("MonoBehaviour Class", "NewMonoBehaviour");
    }

    [MenuItem("Assets/Create/Scripts/MonoBehaviour Class (Namespace)", priority = 52)]
    public static void CreateMonoBehaviourClassWithNamespace()
    {
        CreateScriptFromTemplateName("MonoBehaviour Class (Namespace)", "NewMonoBehaviour");
    }

    [MenuItem("Assets/Create/Scripts/C# Class", priority = 53)]
    public static void CreateClass()
    {
        CreateScriptFromTemplateName("C# Class", "NewClass");
    }

    [MenuItem("Assets/Create/Scripts/Interface", priority = 54)]
    public static void CreateInterface()
    {
        CreateScriptFromTemplateName("Interface", "INewInterface");
    }

    [MenuItem("Assets/Create/Scripts/Singleton Class", priority = 55)]
    public static void CreateSingletonClass()
    {
        CreateScriptFromTemplateName("Singleton Class", "NewSingleton");
    }

    [MenuItem("Assets/Create/Scripts/ScriptableObject", priority = 56)]
    public static void CreateScriptableObjectClass()
    {
        CreateScriptFromTemplateName("ScriptableObject", "NewScriptableObject");
    }

    // Generic template selection menu
    [MenuItem("Assets/Create/Scripts/From Template...", priority = 100)]
    public static void ShowTemplateSelectionWindow()
    {
        ScriptTemplateSelectionWindow.ShowWindow();
    }

    private static void CreateScriptFromTemplateName(string templateName, string defaultFileName)
    {
        var templates = ScriptTemplateManager.GetAllTemplates();
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
                template.templateContent = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class #SCRIPTNAME# : MonoBehaviour
{
    
}";
                break;

            case "MonoBehaviour Class (Namespace)":
                template.templateContent = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIO
{
    public class #SCRIPTNAME# : MonoBehaviour
    {
    
    }
}";
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

[CreateAssetMenu(fileName = ""#SCRIPTNAME#"", menuName = ""ScriptableObjects/Parameters/#SCRIPTNAME#"")]
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

        // Get the selected folder path
        string selectedPath = GetSelectedFolderPath();

        // Create unique filename
        string finalFileName = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(selectedPath, fileName + template.fileExtension));

        // Extract just the filename without extension for the class name
        string className = Path.GetFileNameWithoutExtension(finalFileName);

        // Create the script
        ScriptTemplateManager.CreateScriptFromTemplate(template, className, finalFileName);
    }

    private static string GetSelectedFolderPath()
    {
        string path = "Assets";

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