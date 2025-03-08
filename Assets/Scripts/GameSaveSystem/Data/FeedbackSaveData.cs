using System;

[Serializable]
public class FeedbackSaveData
{
    public bool isMusicOn;
    public bool isSFXOn;
    public bool isHapticOn;

    public FeedbackSaveData()
    {
        isMusicOn = true;
        isSFXOn = true;
        isHapticOn = true;
    }
}