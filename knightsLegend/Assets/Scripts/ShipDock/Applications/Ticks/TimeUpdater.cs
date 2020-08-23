using System;

namespace ShipDock.Applications
{

    public class TimeUpdater : MethodUpdater
    {
        public static TimeUpdater GetTimUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            return new TimeUpdater(totalTime, method, cancelCondition, repeats);
        }

        private float mTime;
        private int mRepeats;
        private Func<bool> mCancelCondition;

        public TimeUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            Recreate(totalTime, method, cancelCondition, repeats);
        }

        public override void Dispose()
        {
            base.Dispose();

            Stop();

            Completion = default;
            mCancelCondition = default;
        }

        public void Recreate(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            TotalTime = totalTime;
            Completion = method;
            mRepeats = repeats;
            Repeatable = mRepeats > 0;
            mCancelCondition = cancelCondition;
        }

        public void Start()
        {
            IsStarted = true;
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

        private bool Repeat()
        {
            mRepeats--;
            bool result = mRepeats > 0;
            if (!result)
            {
                Stop();
                if (IsAutoDispose)
                {
                    Dispose();
                }
            }
            return result;
        }

        private bool CheckCancelCondition()
        {
            bool result = false;
            if (mCancelCondition != default)
            {
                if (mCancelCondition.Invoke())
                {
                    result = true;
                    Stop();
                    if (IsAutoDispose)
                    {
                        Dispose();
                    }
                }
            }
            return result;
        }

        private void TimeCountting(int dTime)
        {
            float t = (float)dTime / UpdatesCacher.UPDATE_CACHER_TIME_SCALE;
            mTime += t;
            if (mTime >= TotalTime)
            {
                IsTimeCounting = false;
                if (Repeatable && !Repeat())
                {
                    Completion?.Invoke();
                }
                else
                {
                    if(CheckCancelCondition())
                    {
                        return;
                    }
                    else
                    {
                        IsTimeCounting = true;
                        Completion?.Invoke();
                        mTime -= TotalTime;
                    }
                }
            }
        }

        public void SetComplete(Action method)
        {
            Completion += method;
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
        public bool IsAutoDispose { get; set; }
        public bool IsStarted { get; private set; }
    }
}