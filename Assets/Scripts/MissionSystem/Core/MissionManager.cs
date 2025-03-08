using Nukebox.Games.MissionSystem.Interfaces;
using Nukebox.Games.MissionSystem.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nukebox.Games.MissionSystem.Core
{
    public class MissionManager<T> where T : IComparable<T>
    {
        #region Variables
        private static readonly List<IMission<T>> _allMissions = new();
        private static readonly List<IMission<T>> _activeMissions = new();
        private static readonly Dictionary<string, IMissionTracker<T>> _trackers = new();
        private static List<IMission<T>> activeMissionPool;

        public List<IMission<T>> ActiveMissions => _activeMissions;
        public event Action<List<IMission<T>>> OnActiveMissionsChanged;
        public event Action<IMission<T>> OnMissionRewardClaimed;
        #endregion

        #region Contructor
        public MissionManager()
        {

        }
        #endregion

        #region Methods
        public void Initialize(List<IMission<T>> missions)
        {
            _allMissions.AddRange(missions);
        }

        public void SetActivePool(List<string> missionPool)
        {
            activeMissionPool = _allMissions.Where(m => missionPool.Contains(m.Id)).ToList();
        }

        public void AssignActiveMissions(int missionAssignCount = 3)
        {
            var newMissions = activeMissionPool.Where(m => !m.IsClaimed && !_activeMissions.Any(am => am.Id == m.Id)).Take(missionAssignCount).ToList();

            if (newMissions.Count != 0)
                _activeMissions.AddRange(newMissions);

            foreach (var mission in _activeMissions)
            {
                if (!_trackers.ContainsKey(mission.Id))
                    _trackers[mission.Id] = new MissionTracker<T>(mission);
            }

            OnActiveMissionsChanged?.Invoke(_activeMissions);
        }

        public void UpdateProgress(string missionId, T amount)
        {
            if (string.IsNullOrEmpty(missionId))
            {
                return;
            }

            var mission = _activeMissions.FirstOrDefault(m => m.Id == missionId);
            if (mission == null)
            {
                return;
            }

            if (!_trackers.TryGetValue(missionId, out var tracker))
            {
                tracker = new MissionTracker<T>(mission);
                _trackers[missionId] = tracker;
            }

            tracker.TrackProgress(amount);

            if (mission.IsCompleted)
            {
                _activeMissions.Remove(mission);
            }
        }

        public void ClaimReward(string missionId)
        {
            var mission = _allMissions.FirstOrDefault(m => m.Id == missionId);
            mission.ClaimReward();
            OnMissionRewardClaimed?.Invoke(mission);
            AssignActiveMissions(1);
        }

        public int GetTotalClaimedMissionsInActivePool()
        {
            return activeMissionPool.Where(m => m.IsClaimed).Count();
        }
        #endregion
    }
}
