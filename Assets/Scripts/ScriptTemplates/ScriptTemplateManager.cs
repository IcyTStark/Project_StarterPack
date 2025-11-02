using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
public class ScriptTemplateManager : MonoBehaviour
{
    private static List<ScriptTemplate> _templates;

    public static List<ScriptTemplate> GetAllTemplates()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ScriptTemplate");

        if (_templates == null || _templates.Count == 0 || guids.Length != _templates.Count)
        {
            LoadTemplates();
        }
        return _templates;
    }

    private static void LoadTemplates()
    {
        _templates = new List<ScriptTemplate>();

        // Find all ScriptTemplate assets in the project
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ScriptTemplate");

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            ScriptTemplate template = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptTemplate>(path);

            if (template != null)
            {
                _templates.Add(template);
            }
        }

        // Sort templates by menu priority
        _templates = _templates.OrderBy(t => t.menuPriority).ToList();
    }

    public static void CreateScriptFromTemplate(ScriptTemplate template, string fileName, string targetPath)
    {
        if (template == null)
        {
            Debug.LogError("Template is null!");
            return;
        }

        string content = template.templateContent;

        // Replace placeholder with actual script name
        content = content.Replace("#SCRIPTNAME#", fileName);

        // Handle namespace wrapping if needed
        if (template.addToNamespace && !string.IsNullOrEmpty(template.namespaceName))
        {
            content = WrapInNamespace(content, template.namespaceName);
        }

        // Ensure target directory exists
        string directory = Path.GetDirectoryName(targetPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write the file
        File.WriteAllText(targetPath, content);

        // Refresh the asset database
        UnityEditor.AssetDatabase.Refresh();

        // Select the created file
        UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetRelativePath(targetPath));
        UnityEditor.Selection.activeObject = asset;
        UnityEditor.EditorGUIUtility.PingObject(asset);

        Debug.Log($"Created script: {targetPath}");
    }

    private static string WrapInNamespace(string content, string namespaceName)
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

    private static string GetRelativePath(string absolutePath)
    {
        string dataPath = Application.dataPath;
        if (absolutePath.StartsWith(dataPath))
        {
            return "Assets" + absolutePath.Substring(dataPath.Length);
        }
        return absolutePath;
    }
}
#endif