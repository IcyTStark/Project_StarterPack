using UnityEngine;

public class SmartDebugInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeDebugSettings()
    {
        var debugSettings = Resources.Load<DebugSettings>("DebugSettings");

        // Initialize on startup
        SmartDebug.Configure(debugSettings.LogLevel, debugSettings.LogToFile, debugSettings.IncludeTimestamps);

        // Enable specified channels
        foreach (string channel in debugSettings.EnabledChannels)
        {
            SmartDebug.EnableChannel(channel);
        }

        SmartDebug.DevOnly("SmartDebug initialized", "DEBUG");
    }
}