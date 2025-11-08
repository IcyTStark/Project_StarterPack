using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;

    [Inject] private IFeedbackManager feedbackManager;

    public void OpenPopUp()
    {
        InitializeToggles();
        gameObject.SetActive(true);
    }

    private void InitializeToggles()
    {
        // Set toggle states WITHOUT triggering their events
        musicToggle.SetIsOnWithoutNotify(feedbackManager.IsMusicOn);
        soundToggle.SetIsOnWithoutNotify(feedbackManager.IsSFXOn);
        vibrationToggle.SetIsOnWithoutNotify(feedbackManager.IsHapticOn);
    }

    public void ToggleMusic()
    {
        feedbackManager.ToggleMusic();
        musicToggle.SetIsOnWithoutNotify(feedbackManager.IsMusicOn);
    }

    public void ToggleSound()
    {
        feedbackManager.ToggleSFX();
        soundToggle.SetIsOnWithoutNotify(feedbackManager.IsSFXOn);
    }

    public void ToggleVibration()
    {
        feedbackManager.ToggleHaptics();
        vibrationToggle.SetIsOnWithoutNotify(feedbackManager.IsHapticOn);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}