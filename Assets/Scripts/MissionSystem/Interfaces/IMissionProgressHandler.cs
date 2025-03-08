using System;

namespace Nukebox.Games.MissionSystem.Interfaces
{
    public interface IMissionProgressHandler
    {
        T LoadMissionProgress<T>(string missionId) where T : IComparable<T>;
        void SaveMissionProgress<T>(string missionId, T progress) where T : IComparable<T>;
        void SaveMissionClaimStatus(string missionId, bool isClaimed);
    }

}
