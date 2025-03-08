using System;
using System.Collections.Generic;

namespace Nukebox.Games.MissionSystem.Core
{
    public partial class JsonMissionProgressHandler
    {
        [Serializable]
        private class ProgressDataWrapper
        {
            public Dictionary<string, object> progress = new();
        }
    }
}

