using UnityEngine;

using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;

    [SerializeField] private FeedbackSaveData feedbackSaveData;
    [SerializeField] private IFeedbackManager feedbackManager;

    public void OpenPopUp()
    {
        Initialization();
        gameObject.SetActive(true);
    }

    private void Initialization()
    {
        feedbackManager = ServiceLocator.GetService<IFeedbackManager>();

        feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData;

        musicToggle.isOn = feedbackSaveData.isMusicOn;
        soundToggle.isOn = feedbackSaveData.isSFXOn;
        vibrationToggle.isOn = feedbackSaveData.isHapticOn;
    }

    public void ToggleMusic()
    {
        feedbackManager.ToggleMusic();
        feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData;
        musicToggle.isOn = feedbackSaveData.isMusicOn;
    }

    public void ToggleSound()
    {
        feedbackManager.ToggleSFX();
        feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData;
        soundToggle.isOn = feedbackSaveData.isSFXOn;
    }

    public void ToggleVibration()
    {
        feedbackManager.ToggleHaptics();
        feedbackSaveData = SaveManager.Instance.SaveData.feedbackSaveData;
        vibrationToggle.isOn = feedbackSaveData.isHapticOn;
    }

    private void OnDisable()
    {
        feedbackManager.Dispose();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
