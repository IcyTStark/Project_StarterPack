using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : PersistantSingleton<GameManager>
{
    [SerializeField] private AudioClip _backgroundMusic;

    private void Start()
    {
        
    }

    [Button]
    private void TestSound()
    {
        var feedbackManager = ServiceLocator.GetService<IFeedbackManager>();
        feedbackManager.PlaySFXAtPosition(_backgroundMusic, this.transform.position, 1f);
    }
}