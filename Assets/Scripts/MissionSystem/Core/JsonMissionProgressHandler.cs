using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Nukebox.Games.MissionSystem.Interfaces;
using UnityEngine;

namespace Nukebox.Games.MissionSystem.Core
{
    public partial class JsonMissionProgressHandler : IMissionProgressHandler
    {
        #region Variables
        private const string FileName = "MissionProgress.json";
        private Dictionary<string, object> _progressData;
        #endregion

        #region Contructor
        public JsonMissionProgressHandler()
        {
            LoadAllProgress();
        }
        #endregion

        #region Methods
        public T LoadMissionProgress<T>(string missionId) where T : IComparable<T>
        {
            if (_progressData.TryGetValue(missionId, out object progress))
            {
                return (T)Convert.ChangeType(progress, typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        public void SaveMissionProgress<T>(string missionId, T progress) where T : IComparable<T>
        {
            _progressData[missionId] = progress;
            SaveAllProgress();
        }

        public void SaveMissionClaimStatus(string missionId, bool isClaimed)
        {
            _progressData[$"{missionId}_claimed"] = isClaimed;
            SaveAllProgress();
        }

        private void LoadAllProgress()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                _progressData = JsonConvert.DeserializeObject<ProgressDataWrapper>(json)?.progress ?? new Dictionary<string, object>();
            }
            else
            {
                _progressData = new Dictionary<string, object>();
            }
        }

        private void SaveAllProgress()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            ProgressDataWrapper wrapper = new ProgressDataWrapper { progress = _progressData };
            string json = JsonConvert.SerializeObject(wrapper, Formatting.None);
            File.WriteAllText(path, json);
        }
        #endregion
    }
}

