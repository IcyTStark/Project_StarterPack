using System;
using UnityEngine;

using TMS.Feedback;

public class ServicesInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeServices()
    {
        InitializeFeedbackServices();
    }

    private static void InitializeFeedbackServices()
    {
        ServiceLocator.RegisterService<IFeedbackManager>(new FeedbackManager());
    }
}