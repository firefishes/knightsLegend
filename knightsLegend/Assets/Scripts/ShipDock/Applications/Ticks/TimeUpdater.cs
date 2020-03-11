using System;

namespace ShipDock.Applications
{

    public class TimeUpdater : MethodUpdater
    {
        public TimeUpdater GetTimUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            return new TimeUpdater(totalTime, method, cancelCondition, repeats);
        }

        private float mTime;
        private int mRepeats;
        private Func<bool> mCancelCondition;

        public TimeUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            TotalTime = totalTime;
            Completion = method;
            mRepeats = repeats;
            Repeatable = mRepeats > 0;
            mCancelCondition = cancelCondition;
        }

        public void Start()
        {
            HasStart = true;
            IsTimeCounting = true;
            UpdaterNotice.AddSceneUpdater(this);
        }

        public void Puase()
        {
            HasStart = false;
            IsTimeCounting = false;
        }

        public void Stop()
        {
            Puase();

            mTime = 0;
            UpdaterNotice.RemoveSceneUpdater(this);
        }

        public override void OnUpdate(int dTime)
        {
            base.OnUpdate(dTime);

            if (HasStart)
            {
                TimeCountting(dTime);
            }
        }

        private void CheckRepeat()
        {
            mRepeats--;
            if (mRepeats <= 0)
            {
                Stop();
            }
        }

        private void CheckOnlyOnce()
        {
            if (mCancelCondition != default)
            {
                if (mCancelCondition.Invoke())
                {
                    Stop();
                }
            }
        }

        private void TimeCountting(int dTime)
        {
            float t = (float)dTime / UpdatesCacher.UPDATE_CACHER_TIME_SCALE;
            mTime += t;
            if (mTime >= TotalTime)
            {
                IsTimeCounting = false;
                if (Repeatable)
                {
                    CheckRepeat();
                }
                else
                {
                    CheckOnlyOnce();
                }
                Completion?.Invoke();
                mTime -= TotalTime;
                IsTimeCounting = true;
            }
        }

        public bool IsCompleteCycle
        {
            get
            {
                return HasStart && !IsTimeCounting;
            }
        }

        public float TotalTime { get; private set; }
        public Action Completion { get; private set; }
        public bool Repeatable { get; private set; }
        public bool HasStart { get; private set; }
        public bool IsTimeCounting { get; private set; }
    }
}