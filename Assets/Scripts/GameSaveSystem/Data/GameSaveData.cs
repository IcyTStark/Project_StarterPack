using System;

[Serializable]
public class  GameSaveData
{
    public FeedbackSaveData feedbackSaveData;

    public GameSaveData()
    {
        feedbackSaveData = new();
    }
}