using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public struct Vector3Lerper
    {
        public Vector3 target;

        private TimeGapper mTime;

        public Vector3Lerper(Vector3 targetValue, Vector3 currentValue, float time)
        {
            mTime = new TimeGapper();
            mTime.totalTime = time;
            target = targetValue;
            Current = currentValue;
            IsCompoleted = false;
        }

        public Vector3 Update(float time)
        {
            mTime.TimeAdvanced(time);
            Current = Vector3.Lerp(Current, target, (mTime.totalTime - mTime.time) / mTime.totalTime);
            IsCompoleted = Vector3.Distance(Current, target) <= 0.1f;
            return Current;
        }

        public void Start(Vector3 value, float time = -1f)
        {
            if (time > 0f)
            {
                mTime.totalTime = time;
            }
            target = value;
            IsCompoleted = false;
        }

        public Vector3 Current { get; private set; }
        public bool IsCompoleted { get; private set; }
    }
}