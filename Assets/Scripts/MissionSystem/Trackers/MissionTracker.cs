using System;
using Nukebox.Games.MissionSystem.Interfaces;

namespace Nukebox.Games.MissionSystem.Trackers
{
    public class MissionTracker<T> : IMissionTracker<T> where T : IComparable<T>
    {
        #region Variables
        private IMission<T> _mission;
        #endregion

        #region Constructor
        public MissionTracker(IMission<T> mission)
        {
            _mission = mission;
        }
        #endregion

        #region Methods
        public void TrackProgress(T amount)
        {
            _mission.AddProgress(amount);
        }
        #endregion
    }
}