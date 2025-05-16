using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AppConfigSettings", menuName = "ScriptableObjects/Parameters/AppConfigSettings")]
public class AppConfigSettings : ScriptableObject
{
    [BoxGroup("App Settings")] [SerializeField] private int targetFrameRate = 60;
    [BoxGroup("App Settings")] [SerializeField] private bool shouldRunInBackground = false;
    [BoxGroup("App Settings")] 
    [InfoBox("Set the following values: \n" +
        "-1 for NeverSleep \n" +
        "-2 for System Settings")] 
    [SerializeField] private int sleepTimeout;

    public int TargetFrameRate => targetFrameRate;
    public bool ShouldRunInBackground => shouldRunInBackground;
    public int SleepTimeout => sleepTimeout;
}