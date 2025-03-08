using UnityEngine;

public class FrameRateManager : Singleton<FrameRateManager>
{
    protected override void Awake()
    {
        base.Awake();

        Initialize();
    }

    private void Initialize()
    {
        QualitySettings.vSyncCount = 0;

        var currentResolution = Screen.currentResolution;
        var refreshRateRatio = currentResolution.refreshRateRatio;

        Application.targetFrameRate = Mathf.Max(60, (int)refreshRateRatio.value);
    }
}
