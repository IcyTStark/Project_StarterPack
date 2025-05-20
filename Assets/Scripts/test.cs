using System;
using UnityEngine;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine.UI;

using TMS.Feedback;
using EnhancedSignals;

public class test : MonoBehaviour
{
    private IFeedbackManager feedbackManager;

    [SerializeField] private AudioType audioType;

    private void Start()
    {
        feedbackManager = ServiceLocator.GetService<IFeedbackManager>();

        if (feedbackManager != null)
        {
            feedbackManager.PlayMusic(audioType);
        }
    }
}