using System;
using System.Collections.Generic;
using Nukebox.Games.MissionSystem.Interfaces;

namespace Nukebox.Games.MissionSystem.Trackers
{
    public abstract class BaseMission<T> : IMission<T> where T: IComparable<T>
    {
        #region Variables
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public T TargetValue { get; protected set; }
        public T CurrentProgress { get; protected set; }
        public bool IsCompleted => CompareProgress(CurrentProgress, TargetValue);

        public bool IsClaimed { get; private set; }

        public event Action<IMission<T>> OnMissionCompleted;
        public event Action<IMission<T>> OnMissionProgressed;
        public event Action<IMission<T>> OnMissionClaimed;

        private readonly IMissionProgressHandler _progressHandler;
        #endregion

        #region Contructor
        protected BaseMission(string id, string name, string description, T target, IMissionProgressHandler progressHandler)
        {
            Id = id;
            Name = name;
            Description = description;
            TargetValue = target;
            _progressHandler = progressHandler;

            CurrentProgress = _progressHandler.LoadMissionProgress<T>(Id);
            IsClaimed = _progressHandler.LoadMissionProgress<bool>($"{Id}_claimed");
        }
        #endregion

        #region Methods
        private bool CompareProgress(T current, T target)
        {
            if (typeof(T) == typeof(int))
            {
                return Comparer<T>.Default.Compare(current, target) >= 0;
            }
            else if (typeof(T) == typeof(float))
            {
                return Comparer<T>.Default.Compare(current, target) >= 0;
            }
            else if (typeof(T) == typeof(string))
            {
                return current.Equals(target);
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                return Comparer<T>.Default.Compare(current, target) >= 0;
            }
            else
            {
                throw new InvalidOperationException("Unsupported comparison for the type.");
            }
        }

        public virtual void AddProgress(T amount)  // Generic AddProgress
        {
            if (IsCompleted) return;

            CurrentProgress = AddCurrentProgress(CurrentProgress, amount);

            // Save progress using injected handler
            _progressHandler.SaveMissionProgress(Id, CurrentProgress);

            OnMissionProgressed?.Invoke(this);
            if (CompareProgress(CurrentProgress, TargetValue))
            {
                OnMissionCompleted?.Invoke(this);
            }
        }

        private T AddCurrentProgress(T current, T amount)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)((int)(object)current + (int)(object)amount);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)((float)(object)current + (float)(object)amount);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)((string)(object)current + (string)(object)amount); // Concatenate strings
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                return (T)(object)((TimeSpan)(object)current + (TimeSpan)(object)amount);
            }
            else
            {
                throw new InvalidOperationException("Unsupported addition for the type.");
            }
        }

        public virtual void Complete()
        {
            OnMissionCompleted?.Invoke(this);
        }

        public void ClaimReward()
        {
            if (!IsCompleted || IsClaimed) return;

            IsClaimed = true;

            _progressHandler.SaveMissionClaimStatus(Id, IsClaimed);
            OnMissionClaimed?.Invoke(this);
        }
        #endregion
    }
}
