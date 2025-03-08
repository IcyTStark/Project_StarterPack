using System;
using Nukebox.Games.MissionSystem.Enum;

namespace Nukebox.Games.MissionSystem.Data
{
    [Serializable]
    public class MissionData
    {
        #region Variables
        public string Id;
        public string Name;
        public string Description;
        public MissionDataType Type;
        public string TargetValue; // Store as string, convert later
        #endregion

        #region Methods
        #endregion
    }
}
