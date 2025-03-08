namespace Nukebox.Games.MissionSystem.Interfaces
{
    public interface IMissionTracker<T>
    {
        void TrackProgress(T amount);
    }
}
