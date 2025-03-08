using Nukebox.Games.MissionSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nukebox.Games.MissionSystem.Core
{
    public class MissionProvider<T> where T : IComparable<T>
    {
        #region Variables
        private List<IMission<T>> _missionPool;
        #endregion

        #region Contructor
        public MissionProvider(List<IMission<T>> missions)
        {
            _missionPool = missions;
        }
        #endregion

        #region Methods
        public List<IMission<T>> GetNewActiveMissions()
        {
            return _missionPool.Where(m => !m.IsCompleted).Take(3).ToList();
        }
        #endregion
    }
}
