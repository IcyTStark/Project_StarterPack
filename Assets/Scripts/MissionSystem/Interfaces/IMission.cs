using System;

namespace Nukebox.Games.MissionSystem.Interfaces
{
    public interface IMission<T> where T : IComparable<T>
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        T TargetValue { get; }
        T CurrentProgress { get; }
        bool IsCompleted { get; }
        bool IsClaimed { get; } 

        void AddProgress(T amount);
        void Complete();
        void ClaimReward();

        event Action<IMission<T>> OnMissionCompleted;
        event Action<IMission<T>> OnMissionProgressed;
        event Action<IMission<T>> OnMissionClaimed; 
    }
}
