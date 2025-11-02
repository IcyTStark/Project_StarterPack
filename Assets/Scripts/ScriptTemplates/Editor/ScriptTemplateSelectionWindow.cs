using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptTemplateSelectionWindow : EditorWindow
{
    private List<ScriptTemplate> templates;
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private ScriptTemplate selectedTemplate;
    private string customFileName = "";
    private bool useCustomNamespace = false;
    private string customNamespace = "FOI";

    public static void ShowWindow()
    {
        ScriptTemplateSelectionWindow window = GetWindow<ScriptTemplateSelectionWindow>("Script Templates");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnEnable()
    {
        templates = ScriptTemplateManager.GetAllTemplates();
        if (templates.Count == 0)
        {
            RefreshTemplates();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        // Header
        EditorGUILayout.LabelField("Create Script from Template", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Search filter
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter);
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            searchFilter = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Refresh button
        if (GUILayout.Button("Refresh Templates"))
        {
            RefreshTemplates();
        }

        EditorGUILayout.Space();

        // Templates list
        EditorGUILayout.LabelField("Available Templates:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (templates != null && templates.Count > 0)
        {
            foreach (var template in templates)
            {
                if (!string.IsNullOrEmpty(searchFilter) &&
                    !template.templateName.ToLower().Contains(searchFilter.ToLower()))
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal("box");

                // Template info
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(template.templateName, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Default: {template.defaultFileName}{template.fileExtension}", EditorStyles.miniLabel);
                if (!string.IsNullOrEmpty(template.namespaceName) && template.addToNamespace)
                {
                    EditorGUILayout.LabelField($"Namespace: {template.namespaceName}", EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                // Select button
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    selectedTemplate = template;
                    customFileName = template.defaultFileName;
                    useCustomNamespace = template.addToNamespace;
                    customNamespace = string.IsNullOrEmpty(template.namespaceName) ? "FOI" : template.namespaceName;
                }

                // Create button
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    ScriptTemplateEditor.CreateScriptFromTemplate(template);
                    Close();
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No templates found. Make sure you have ScriptTemplate assets in your project.", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();

        // Selected template section
        if (selectedTemplate != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Template:", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Template: {selectedTemplate.templateName}");

            // File Name field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name:", GUILayout.Width(70));
            customFileName = EditorGUILayout.TextField(customFileName);
            EditorGUILayout.LabelField(selectedTemplate.fileExtension, GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();

            // Namespace field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Namespace:", GUILayout.Width(80));
            customNamespace = EditorGUILayout.TextField(customNamespace);
            EditorGUILayout.EndHorizontal();

            // Use namespace checkbox
            useCustomNamespace = EditorGUILayout.Toggle("Use Namespace", useCustomNamespace);

            EditorGUILayout.Space();

            // Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Script"))
            {
                if (!string.IsNullOrEmpty(customFileName))
                {
                    CreateScriptWithCustomSettings();
                    Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name", "Please enter a valid file name.", "OK");
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                selectedTemplate = null;
                customFileName = "";
                useCustomNamespace = false;
                customNamespace = "FOI";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }

    private void RefreshTemplates()
    {
        templates = ScriptTemplateManager.GetAllTemplates();
        selectedTemplate = null;
        customFileName = "";
        useCustomNamespace = false;
        customNamespace = "FOI";
        Repaint();
    }

    private void CreateScriptWithCustomSettings()
    {
        // Get the selected folder path
        string selectedPath = GetSelectedFolderPath();

        // Create unique filename
        string finalFileName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(
            System.IO.Path.Combine(selectedPath, customFileName + selectedTemplate.fileExtension));

        // Extract just the filename without extension for the class name
        string className = System.IO.Path.GetFileNameWithoutExtension(finalFileName);

        // Create the script with custom namespace settings
        CreateScriptFromTemplateWithNamespace(selectedTemplate, className, finalFileName, useCustomNamespace, customNamespace);
    }

    private void CreateScriptFromTemplateWithNamespace(ScriptTemplate template, string fileName, string targetPath, bool addNamespace, string namespaceName)
    {
        if (template == null)
        {
            Debug.LogError("Template is null!");
            return;
        }

        string content = template.templateContent;

        // Replace placeholder with actual script name
        content = content.Replace("#SCRIPTNAME#", fileName);

        // Handle namespace wrapping based on custom settings
        if (addNamespace && !string.IsNullOrEmpty(namespaceName))
        {
            content = WrapInNamespace(content, namespaceName);
        }

        // Ensure target directory exists
        string directory = System.IO.Path.GetDirectoryName(targetPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        // Write the file
        System.IO.File.WriteAllText(targetPath, content);

        // Refresh the asset database
        UnityEditor.AssetDatabase.Refresh();

        // Select the created file
        UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetRelativePath(targetPath));
        UnityEditor.Selection.activeObject = asset;
        UnityEditor.EditorGUIUtility.PingObject(asset);

        Debug.Log($"Created script: {targetPath}");
    }

    private string WrapInNamespace(string content, string namespaceName)
    {
        // Check if content already contains namespace
        if (content.Contains($"namespace {namespaceName}"))
        {
            return content;
        }

        // Find using statements
        string[] lines = content.Split('\n');
        List<string> usingLines = new List<string>();
        List<string> contentLines = new List<string>();

        bool foundNonUsingLine = false;

        foreach (string line in lines)
        {
            if (line.Trim().StartsWith("using ") && !foundNonUsingLine)
            {
                usingLines.Add(line);
            }
            else
            {
                foundNonUsingLine = true;
                contentLines.Add(line);
            }
        }

        while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[0]))
        {
            contentLines.RemoveAt(0);
        }
        while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[contentLines.Count - 1]))
        {
            contentLines.RemoveAt(contentLines.Count - 1);
        }

        // Rebuild content with namespace
        List<string> result = new List<string>();
        result.AddRange(usingLines);

        if (usingLines.Count > 0)
            result.Add("");

        result.Add($"namespace {namespaceName}");
        result.Add("{");

        // Add indentation to content lines
        foreach (string line in contentLines)
        {
            if (!string.IsNullOrWhiteSpace(line))
                result.Add("    " + line);
            else
                result.Add(line);
        }

        result.Add("}");

        return string.Join("\n", result);
    }

    private string GetSelectedFolderPath()
    {
        string path = "Assets";

        // First, try to get the active folder from Unity's internal method
        try
        {
            var projectWindowUtilType = typeof(UnityEditor.ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (getActiveFolderPath != null)
            {
                string activePath = (string)getActiveFolderPath.Invoke(null, null);
                if (!string.IsNullOrEmpty(activePath))
                {
                    path = activePath;
                    Debug.Log($"Using active folder: {path}");
                    return path;
                }
            }
        }
        catch
        {
            // Fallback to manual detection if reflection fails
        }

        // Fallback: Check currently selected objects
        foreach (UnityEngine.Object obj in UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets))
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(obj);

            if (System.IO.Directory.Exists(assetPath))
            {
                path = assetPath;
                Debug.Log($"Using selected folder: {path}");
                break;
            }
            else if (System.IO.File.Exists(assetPath))
            {
                path = System.IO.Path.GetDirectoryName(assetPath);
                Debug.Log($"Using parent folder of selected file: {path}");
                break;
            }
        }

        // Final fallback
        if (path == "Assets")
        {
            Debug.Log("No specific folder selected, using Assets folder");
        }

        return path;
    }

    private string GetRelativePath(string absolutePath)
    {
        string dataPath = Application.dataPath;
        if (absolutePath.StartsWith(dataPath))
        {
            return "Assets" + absolutePath.Substring(dataPath.Length);
        }
        return absolutePath;
    }
}