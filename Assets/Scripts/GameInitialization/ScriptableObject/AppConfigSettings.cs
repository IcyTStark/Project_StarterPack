using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AppConfigSettings", menuName = "ScriptableObjects/Parameters/AppConfigSettings")]
public class AppConfigSettings : ScriptableObject
{
    [BoxGroup("App Settings")] [SerializeField] private int targetFrameRate = 60;
    [BoxGroup("App Settings")] [SerializeField] private bool shouldRunInBackground = false;
    [BoxGroup("App Settings")] [SerializeField] private int sleepTimeout;

    public int TargetFrameRate => targetFrameRate;
    public bool ShouldRunInBackground => shouldRunInBackground;
    public int SleepTimeout => sleepTimeout;
}