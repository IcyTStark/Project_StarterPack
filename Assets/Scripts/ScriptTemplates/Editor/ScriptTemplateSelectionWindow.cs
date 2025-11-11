using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptTemplateSelectionWindow : EditorWindow
{
    private List<ScriptTemplate> templates;
    private Vector2 scrollPosition;
    private Vector2 previewScrollPosition;
    private string searchFilter = "";
    private ScriptTemplate selectedTemplate;
    private string customFileName = "";
    private bool useCustomNamespace = false;
    private string customNamespace = "FIO";
    private string targetFolderPath = "Assets";

    // UI State
    private bool showPreview = true;
    private int selectedIndex = -1;
    private List<ScriptTemplate> recentTemplates = new List<ScriptTemplate>();

    // Categories
    private Dictionary<string, List<ScriptTemplate>> categorizedTemplates;
    private string[] categories;
    private bool[] categoryFoldouts;

    // Colors and styles
    private GUIStyle headerStyle;
    private GUIStyle templateBoxStyle;
    private GUIStyle selectedBoxStyle;
    private GUIStyle previewStyle;
    private bool stylesInitialized = false;

    [MenuItem("Assets/Create/Scripts/From Template... %&T", priority = 100)] // Ctrl+Alt+T shortcut
    public static void ShowWindow()
    {
        ScriptTemplateSelectionWindow window = GetWindow<ScriptTemplateSelectionWindow>("Script Templates");
        window.minSize = new Vector2(650, 500);

        // Capture the selected folder path BEFORE showing the window
        window.targetFolderPath = GetSelectedFolderPathStatic();

        window.Show();
    }

    private void OnEnable()
    {
        LoadTemplates();
        LoadRecentTemplates();

        // Capture folder path if not already set
        if (string.IsNullOrEmpty(targetFolderPath))
        {
            targetFolderPath = GetSelectedFolderPathStatic();
        }
    }

    private void InitializeStyles()
    {
        if (stylesInitialized) return;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            margin = new RectOffset(0, 0, 10, 10)
        };

        templateBoxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(10, 10, 8, 8),
            margin = new RectOffset(0, 0, 2, 2)
        };

        selectedBoxStyle = new GUIStyle(templateBoxStyle);
        selectedBoxStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.5f, 0.8f, 0.3f));

        previewStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = false,
            richText = false,
            font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font
        };

        stylesInitialized = true;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void LoadTemplates()
    {
        templates = ScriptTemplateManager.GetAllTemplates();

        // Auto-create default templates if none exist
        if (templates.Count == 0)
        {
            if (EditorUtility.DisplayDialog("No Templates Found",
                "No script templates found. Would you like to create the default templates?",
                "Yes, Create Defaults", "No"))
            {
                TemplateCreator.CreateDefaultTemplates();
                templates = ScriptTemplateManager.GetAllTemplates();
            }
        }

        CategorizeTemplates();
    }

    private void CategorizeTemplates()
    {
        categorizedTemplates = new Dictionary<string, List<ScriptTemplate>>();

        foreach (var template in templates)
        {
            string category = GetCategory(template.templateName);
            if (!categorizedTemplates.ContainsKey(category))
            {
                categorizedTemplates[category] = new List<ScriptTemplate>();
            }
            categorizedTemplates[category].Add(template);
        }

        categories = categorizedTemplates.Keys.ToArray();
        categoryFoldouts = new bool[categories.Length];

        // Expand first category by default
        if (categoryFoldouts.Length > 0)
            categoryFoldouts[0] = true;
    }

    private string GetCategory(string templateName)
    {
        if (templateName.Contains("MonoBehaviour")) return "Unity Components";
        if (templateName.Contains("ScriptableObject")) return "Unity Components";
        if (templateName.Contains("Singleton")) return "Design Patterns";
        if (templateName.Contains("Interface")) return "C# Basics";
        if (templateName.Contains("Class")) return "C# Basics";
        return "Other";
    }

    private void LoadRecentTemplates()
    {
        string recent = EditorPrefs.GetString("ScriptTemplate_Recent", "");
        if (!string.IsNullOrEmpty(recent))
        {
            string[] recentNames = recent.Split(',');
            recentTemplates.Clear();
            foreach (var name in recentNames.Take(3))
            {
                var template = templates.FirstOrDefault(t => t.templateName == name);
                if (template != null)
                    recentTemplates.Add(template);
            }
        }
    }

    private void SaveRecentTemplate(ScriptTemplate template)
    {
        recentTemplates.Remove(template);
        recentTemplates.Insert(0, template);
        if (recentTemplates.Count > 3)
            recentTemplates = recentTemplates.Take(3).ToList();

        string recent = string.Join(",", recentTemplates.Select(t => t.templateName));
        EditorPrefs.SetString("ScriptTemplate_Recent", recent);
    }

    private void OnGUI()
    {
        InitializeStyles();

        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

        DrawHeader();
        DrawToolbar();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

        // Left panel - Template list
        DrawTemplateList();

        // Right panel - Preview and settings
        if (showPreview)
        {
            DrawPreviewPanel();
        }

        EditorGUILayout.EndHorizontal();

        // Bottom panel - Selected template actions
        if (selectedTemplate != null)
        {
            DrawActionPanel();
        }

        EditorGUILayout.EndVertical();

        HandleKeyboardShortcuts();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Create Script from Template", headerStyle);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Target Folder: {targetFolderPath}", EditorStyles.miniLabel);
        if (GUILayout.Button("Change", GUILayout.Width(60)))
        {
            string newPath = EditorUtility.OpenFolderPanel("Select Target Folder", targetFolderPath, "");
            if (!string.IsNullOrEmpty(newPath))
            {
                targetFolderPath = GetRelativePath(newPath);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Search
        GUILayout.Label("Search:", GUILayout.Width(50));
        string newSearchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarTextField);
        if (newSearchFilter != searchFilter)
        {
            searchFilter = newSearchFilter;
            selectedIndex = -1;
        }

        if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            searchFilter = "";
            GUI.FocusControl(null);
        }

        GUILayout.FlexibleSpace();

        // Toggle preview
        showPreview = GUILayout.Toggle(showPreview, "Preview", EditorStyles.toolbarButton, GUILayout.Width(70));

        // Refresh button
        if (GUILayout.Button("↻ Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            LoadTemplates();
            LoadRecentTemplates();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTemplateList()
    {
        float listWidth = showPreview ? position.width * 0.45f : position.width - 20;

        EditorGUILayout.BeginVertical(GUILayout.Width(listWidth));

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Recent templates
        if (recentTemplates.Count > 0 && string.IsNullOrEmpty(searchFilter))
        {
            GUILayout.Label("Recent", EditorStyles.boldLabel);
            foreach (var template in recentTemplates)
            {
                DrawTemplateItem(template, true);
            }
            GUILayout.Space(10);
            DrawSeparator();
            GUILayout.Space(10);
        }

        // Categorized templates
        if (string.IsNullOrEmpty(searchFilter))
        {
            DrawCategorizedTemplates();
        }
        else
        {
            DrawFilteredTemplates();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void DrawCategorizedTemplates()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            string category = categories[i];
            categoryFoldouts[i] = EditorGUILayout.Foldout(categoryFoldouts[i], category, true, EditorStyles.foldoutHeader);

            if (categoryFoldouts[i])
            {
                foreach (var template in categorizedTemplates[category])
                {
                    DrawTemplateItem(template, false);
                }
                GUILayout.Space(5);
            }
        }
    }

    private void DrawFilteredTemplates()
    {
        bool anyFound = false;
        foreach (var template in templates)
        {
            if (template.templateName.ToLower().Contains(searchFilter.ToLower()))
            {
                DrawTemplateItem(template, false);
                anyFound = true;
            }
        }

        if (!anyFound)
        {
            EditorGUILayout.HelpBox($"No templates found matching '{searchFilter}'", MessageType.Info);
        }
    }

    private void DrawTemplateItem(ScriptTemplate template, bool isRecent)
    {
        bool isSelected = selectedTemplate == template;
        GUIStyle boxStyle = isSelected ? selectedBoxStyle : templateBoxStyle;

        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.BeginHorizontal();

        // Template info
        EditorGUILayout.BeginVertical();

        GUILayout.Label(template.templateName, EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"File: {template.defaultFileName}{template.fileExtension}", EditorStyles.miniLabel);
        if (!string.IsNullOrEmpty(template.namespaceName) && template.addToNamespace)
        {
            GUILayout.Label($"• Namespace: {template.namespaceName}", EditorStyles.miniLabel);
        }
        if (isRecent)
        {
            GUILayout.Label("★", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        // Buttons
        if (GUILayout.Button(isSelected ? "Selected ✓" : "Select", GUILayout.Width(80)))
        {
            SelectTemplate(template);
        }

        if (GUILayout.Button("Quick Create", GUILayout.Width(90)))
        {
            QuickCreateScript(template);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.Space(2);
    }

    private void DrawPreviewPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));

        GUILayout.Label("Preview", EditorStyles.boldLabel);

        if (selectedTemplate != null)
        {
            previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition, "box");

            string previewContent = GetPreviewContent();
            EditorGUILayout.TextArea(previewContent, previewStyle, GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("Select a template to see preview", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private string GetPreviewContent()
    {
        if (selectedTemplate == null) return "";

        string fileName = string.IsNullOrEmpty(customFileName) ? selectedTemplate.defaultFileName : customFileName;
        string content = selectedTemplate.templateContent.Replace("#SCRIPTNAME#", fileName);

        if (useCustomNamespace && !string.IsNullOrEmpty(customNamespace))
        {
            content = WrapInNamespace(content, customNamespace);
        }
        else if (selectedTemplate.addToNamespace && !string.IsNullOrEmpty(selectedTemplate.namespaceName))
        {
            content = WrapInNamespace(content, selectedTemplate.namespaceName);
        }

        return content;
    }

    private void DrawActionPanel()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Create Script", EditorStyles.boldLabel);

        // File name
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("File Name:", GUILayout.Width(80));
        customFileName = EditorGUILayout.TextField(customFileName);
        GUILayout.Label(selectedTemplate.fileExtension, GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        // Namespace settings
        EditorGUILayout.BeginHorizontal();
        useCustomNamespace = EditorGUILayout.Toggle("Use Custom Namespace", useCustomNamespace, GUILayout.Width(200));
        if (useCustomNamespace)
        {
            customNamespace = EditorGUILayout.TextField(customNamespace);
        }
        else if (selectedTemplate.addToNamespace)
        {
            EditorGUILayout.LabelField($"(Will use: {selectedTemplate.namespaceName})", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Action buttons
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = new Color(0.5f, 0.8f, 0.5f);
        if (GUILayout.Button("Create Script", GUILayout.Height(30)))
        {
            if (ValidateAndCreateScript())
            {
                Close();
            }
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(30)))
        {
            selectedTemplate = null;
            customFileName = "";
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawSeparator()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
    }

    private void SelectTemplate(ScriptTemplate template)
    {
        selectedTemplate = template;
        customFileName = template.defaultFileName;
        useCustomNamespace = template.addToNamespace;
        customNamespace = string.IsNullOrEmpty(template.namespaceName) ? "FIO" : template.namespaceName;
    }

    private void QuickCreateScript(ScriptTemplate template)
    {
        SaveRecentTemplate(template);
        ScriptTemplateEditor.CreateScriptFromTemplate(template);
        Close();
    }

    private bool ValidateAndCreateScript()
    {
        if (string.IsNullOrEmpty(customFileName))
        {
            EditorUtility.DisplayDialog("Invalid Name", "Please enter a valid file name.", "OK");
            return false;
        }

        // Check for invalid characters
        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        if (customFileName.IndexOfAny(invalidChars) >= 0)
        {
            EditorUtility.DisplayDialog("Invalid Name", "File name contains invalid characters.", "OK");
            return false;
        }

        CreateScriptWithCustomSettings();
        SaveRecentTemplate(selectedTemplate);
        return true;
    }

    private void HandleKeyboardShortcuts()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            // Enter to create
            if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
            {
                if (selectedTemplate != null && !string.IsNullOrEmpty(customFileName))
                {
                    if (ValidateAndCreateScript())
                    {
                        Close();
                    }
                    e.Use();
                }
            }
            // Escape to cancel/clear selection
            else if (e.keyCode == KeyCode.Escape)
            {
                if (selectedTemplate != null)
                {
                    selectedTemplate = null;
                    customFileName = "";
                    e.Use();
                }
                else
                {
                    Close();
                }
            }
        }
    }

    private void CreateScriptWithCustomSettings()
    {
        string selectedPath = targetFolderPath;
        string finalFileName = AssetDatabase.GenerateUniqueAssetPath(
            System.IO.Path.Combine(selectedPath, customFileName + selectedTemplate.fileExtension));
        string className = System.IO.Path.GetFileNameWithoutExtension(finalFileName);

        CreateScriptFromTemplateWithNamespace(selectedTemplate, className, finalFileName,
            useCustomNamespace, customNamespace);
    }

    private void CreateScriptFromTemplateWithNamespace(ScriptTemplate template, string fileName,
        string targetPath, bool addNamespace, string namespaceName)
    {
        if (template == null)
        {
            Debug.LogError("Template is null!");
            return;
        }

        string content = template.templateContent;
        content = content.Replace("#SCRIPTNAME#", fileName);

        if (addNamespace && !string.IsNullOrEmpty(namespaceName))
        {
            content = WrapInNamespace(content, namespaceName);
        }
        else if (template.addToNamespace && !string.IsNullOrEmpty(template.namespaceName))
        {
            content = WrapInNamespace(content, template.namespaceName);
        }

        string directory = System.IO.Path.GetDirectoryName(targetPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        System.IO.File.WriteAllText(targetPath, content);
        AssetDatabase.Refresh();

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(GetRelativePath(targetPath));
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        Debug.Log($"✓ Created script: {targetPath}");
    }

    private string WrapInNamespace(string content, string namespaceName)
    {
        if (content.Contains($"namespace {namespaceName}"))
            return content;

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
            contentLines.RemoveAt(0);
        while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[contentLines.Count - 1]))
            contentLines.RemoveAt(contentLines.Count - 1);

        List<string> result = new List<string>();
        result.AddRange(usingLines);

        if (usingLines.Count > 0)
            result.Add("");

        result.Add($"namespace {namespaceName}");
        result.Add("{");

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

    private static string GetSelectedFolderPathStatic()
    {
        string path = "Assets";

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
        catch { }

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);

            if (System.IO.Directory.Exists(assetPath))
            {
                path = assetPath;
                break;
            }
            else if (System.IO.File.Exists(assetPath))
            {
                path = System.IO.Path.GetDirectoryName(assetPath);
                break;
            }
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