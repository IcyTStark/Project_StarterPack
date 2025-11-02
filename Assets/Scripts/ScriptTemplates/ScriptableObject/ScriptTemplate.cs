using UnityEngine;

[CreateAssetMenu(fileName = "New Script Template", menuName = "Tools/Script Template")]
public class ScriptTemplate : ScriptableObject
{
    [Header("Template Configuration")]
    public string templateName = "New Script";
    public string menuPath = "Create/Scripts/";
    public string defaultFileName = "NewScript";
    public string fileExtension = ".cs";

    [Header("Template Content")]
    [TextArea(15, 30)]
    public string templateContent = "";

    [Header("Settings")]
    public bool addToNamespace = false;
    public string namespaceName = "FIO";
    public int menuPriority = 0;
}
