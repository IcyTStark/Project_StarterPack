using UnityEngine;

public class FeedbackManager : Singleton<FeedbackManager>
{
    [Header("Save Data: ")]
    [SerializeField] private FeedbackSaveData feedbackSaveData;

    [Header("Dependencies: ")]
    [SerializeField] private Transform audioHolder;

    [Header("Audio Settings: ")]
    [SerializeField] private AudioSettings audioSettings;
    private AudioManager audioManager;

    [Header("Haptic Settings: ")]
    private HapticsManager hapticsManager;

    protected override void Awake()
    {
        base.Awake();

        audioManager ??= new AudioManager(audioSettings);
        hapticsManager ??= new HapticsManager();

        //signalManager = ServiceLocator.GetService<ISignalManager>();
    }

    public void OnEnable()
    {
        //signalManager.AddListener<OnDataLoadedSignal>(OnDataLoaded);
    }

    public void OnDisable()
    {
        //signalManager.RemoveListener<OnDataLoadedSignal>(OnDataLoaded);
    }

    #region Audio Calls
    private bool IsMusicOn() => feedbackSaveData?.isMusicOn ?? true;

    private bool IsSFXOn() => feedbackSaveData?.isSFXOn ?? true;

    public void PlayMusic(AudioClip clip)
    {
        if (IsMusicOn())
        {
            audioManager.PlayMusic(clip);
        }
    }

    public void PlayAudioClip(string clipName)
    {
        if (IsSFXOn())
        {
            audioManager.PlayAudioClip(clipName);
        }
    }

    public void PlayAudioClipOneShot(string clipName)
    {
        if (IsSFXOn())
        {
            audioManager.PlayAudioClipAtOneShot(clipName);
        }
    }

    public void PlayUISound(AudioClip clip)
    {
        if (IsSFXOn())
        {
            audioManager.PlayUISound(clip);
        }
    }
    #endregion

    #region Toggle Feedbacks
    public void ToggleMusic() => ToggleSetting(ref feedbackSaveData.isMusicOn);
    public void ToggleSFX() => ToggleSetting(ref feedbackSaveData.isSFXOn);
    public void ToggleHaptics() => ToggleSetting(ref feedbackSaveData.isHapticOn, true);

    private void ToggleSetting(ref bool setting, bool isHaptics = false)
    {
        setting = !setting;

        if(isHaptics)
            hapticsManager.ToggleHaptic(feedbackSaveData.isHapticOn);

        Save();
    }
    #endregion

    #region Haptic Calls
    private bool IsHapticsOn() => feedbackSaveData?.isHapticOn ?? true;

    //public void PlayHapticsFromPreset(HapticTypes hapticTypes)
    //{
    //    if (IsHapticsOn())
    //    {
    //        hapticsManager.PlayPreset(hapticTypes);
    //    }
    //}
    #endregion

    #region Load and Save
    private void OnDataLoaded()
    {
        feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData ?? new FeedbackSaveData();
    }

    public async void Save()
    {
        SaveManager.Instance.SaveData.feedbackSaveData = feedbackSaveData;
        await SaveManager.Instance.SaveAsync();
    }
    #endregion
}