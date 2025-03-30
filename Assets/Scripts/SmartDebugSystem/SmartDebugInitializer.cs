// Optional initializer component
using UnityEngine;

public class SmartDebugInitializer : MonoBehaviour
{
    [SerializeField] private SmartDebug.LogLevel logLevel = SmartDebug.LogLevel.Debug;
    [SerializeField] private bool logToFile = false;
    [SerializeField] private bool includeTimestamps = true;
    [SerializeField] private string[] enabledChannels = { "DEFAULT", "SYSTEM" };

    private void Awake()
    {
        // Initialize on startup
        SmartDebug.Configure(logLevel, logToFile, includeTimestamps);

        //// Enable specified channels
        foreach (string channel in enabledChannels)
        {
            SmartDebug.EnableChannel(channel);
        }

        SmartDebug.Info("SmartDebug initialized via component", "SYSTEM");

        // Optional: Make this object persist between scenes
        //DontDestroyOnLoad(gameObject);
    }
}