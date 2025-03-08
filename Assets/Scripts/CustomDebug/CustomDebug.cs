#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif
using UnityEngine;

public static class CustomDebug
{
    private const string LOG_DEFINE = "ENABLE_LOGS";

    public enum LogType
    {
        MESSAGE,
        WARNING,
        ERROR
    }

    private static LogType currentLogLevel = LogType.MESSAGE;

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void Log(string featureName, string message, Object inObject = null)
    {
        PrintToConsole(LogType.MESSAGE, featureName, message, inObject);
    }

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void LogWarning(string featureName, string message, Object inObject = null)
    {
        PrintToConsole(LogType.WARNING, featureName, message, inObject);
    }

    [System.Diagnostics.Conditional("ENABLE_LOGS")]
    public static void LogError(string featureName, string message, Object inObject = null)
    {
        PrintToConsole(LogType.ERROR, featureName, message, inObject);
    }

    private static void PrintToConsole(LogType logType, string featureName, string message, Object inObject = null)
    {
        if (logType < currentLogLevel) return;

        string colorTag = GetColorTag(logType);
        string formattedMessage = $"{colorTag}[{featureName.ToUpper()}]:</color> {message}";

        Debug.Log(formattedMessage, inObject);
    }

    private static string GetColorTag(LogType type)
    {
        return type switch
        {
            LogType.MESSAGE => "<color=white>",
            LogType.WARNING => "<color=yellow>",
            LogType.ERROR => "<color=red>",
            _ => "<color=white>"
        };
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Debug/ToggleDebug")]
    private static void ToggleDebug()
    {
        NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        string defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

        if (defines.Contains(LOG_DEFINE))
        {
            defines = defines.Replace(LOG_DEFINE, "").Replace(";;", ";").Trim(';');
        }
        else
        {
            defines = string.IsNullOrEmpty(defines) ? LOG_DEFINE : $"{defines};{LOG_DEFINE}";
        }

        PlayerSettings.SetScriptingDefineSymbols(buildTarget, defines);
    }

    [MenuItem("Tools/Debug/LogLevel/Message")]
    private static void SetMessageLogLevel()
    {
        currentLogLevel = LogType.MESSAGE;
    }

    [MenuItem("Tools/Debug/LogLevel/Warning")]
    private static void SetWarningLogLevel()
    {
        currentLogLevel = LogType.WARNING;
    }

    [MenuItem("Tools/Debug/LogLevel/Error")]
    private static void SetErrorLogLevel()
    {
        currentLogLevel = LogType.ERROR;
    }
#endif
}