using UnityEngine;

[CreateAssetMenu(fileName = "DebugSettings", menuName = "ScriptableObjects/DebugSettings")]
public class DebugSettings : ScriptableObject
{
    [SerializeField] private SmartDebug.LogLevel logLevel = SmartDebug.LogLevel.Debug;
    [SerializeField] private bool logToFile = false;
    [SerializeField] private bool includeTimestamps = true;
    [SerializeField] private string[] enabledChannels = { "DEFAULT", "SYSTEM" };

    public SmartDebug.LogLevel LogLevel => logLevel;
    public bool LogToFile => logToFile;
    public bool IncludeTimestamps => includeTimestamps;
    public string[] EnabledChannels => enabledChannels;
}