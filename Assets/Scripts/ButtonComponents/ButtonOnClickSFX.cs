using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ButtonOnClickSFX : MonoBehaviour
{
    [Inject] private IFeedbackManager feedbackManager;
    [SerializeField] private AudioType audioType;
    private Button button;

    private void Awake()
    {
        TryGetComponent<Button>(out button);
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        feedbackManager?.PlayUISound(audioType);
    }
}