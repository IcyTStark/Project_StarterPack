using UnityEngine;

public class AppConfigurator : PersistantSingleton<AppConfigurator>
{
    private AppConfigSettings _appConfigSettings;

    protected override void Awake()
    {
        base.Awake();

        _appConfigSettings = Resources.Load<AppConfigSettings>("AppConfigSettings");

        Initialize();
    }

    private void Initialize()
    {
        SetVSyncCount();

        SetRefreshRateBasedOnPhone();

        SetAppBackgroundMode();

        SetApplicationSleepMode();
    }

    private void SetVSyncCount()
    {
        QualitySettings.vSyncCount = 0;
    }

    private void SetRefreshRateBasedOnPhone()
    {
        var currentResolution = Screen.currentResolution;
        var refreshRateRatio = currentResolution.refreshRateRatio;

        Application.targetFrameRate = Mathf.Max(_appConfigSettings.TargetFrameRate, (int)refreshRateRatio.value);
    }

    private void SetAppBackgroundMode()
    {
        Application.runInBackground = _appConfigSettings.ShouldRunInBackground;
    }

    private void SetApplicationSleepMode()
    {
        Screen.sleepTimeout = _appConfigSettings.SleepTimeout;
    }
}
