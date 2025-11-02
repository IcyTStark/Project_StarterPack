using System;
using UnityEngine;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine.UI;

using TMS.Feedback;
using deVoid.Utils;

using FIO.AddressablesSystem;

public class test : MonoBehaviour
{
    private IFeedbackManager feedbackManager;

    [SerializeField] private AudioType audioType;

    private void OnEnable()
    {
        Signals.Get<OnAddressableInitialized>().AddListener(AddressableTest);
    }

    private async void AddressableTest()
    {
        await AddressablesManager.Instance.LoadAssetAsync<GameObject>("Cube");
    }

    private void SoundTest()
    {
        feedbackManager = ServiceLocator.GetService<IFeedbackManager>();

        if (feedbackManager != null)
        {
            feedbackManager.PlayMusic(audioType);
        }
    }
}