using System;

namespace ShipDock.Applications
{
    public class TimeUpdater : MethodUpdater
    {
        public static TimeUpdater GetTimUpdater(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            return new TimeUpdater(totalTime, method, cancelCondition, repeats);
        }

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

            TotalRepeats = 0;
            Completion = default;
            mCancelCondition = default;
        }

        public void Recreate(float totalTime, Action method, Func<bool> cancelCondition = default, int repeats = 0)
        {
            TotalRepeats = repeats;
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
            IsStarted = true;
            if (IsPause)
            {
                IsPause = false;
            }
            else
            {
                UpdaterNotice.AddSceneUpdater(this);
            }
        }

        public void Restart()
        {
            Recreate(TotalTime, Completion, mCancelCondition, TotalRepeats);
            Start();
        }

        public void Pause()
        {
            if (IsStarted)
            {
                HasStart = false;
                IsTimeCounting = false;
                IsPause = true;
            }
        }

        public void Stop()
        {
            Pause();

            Time = 0;
            IsStarted = false;
            IsPause = false;

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

        /// <summary>
        /// 是否还能重复计时
        /// </summary>
        private bool ShouldRepeat()
        {
            mRepeats--;
            bool result = mRepeats > 0;
            if (!result)
            {
                Stop();
            }
            return result;
        }

        /// <summary>
        /// 检测定时器退出条件
        /// </summary>
        private bool CheckCancelCondition()
        {
            bool result = false;
            if (mCancelCondition.Invoke())
            {
                result = true;
                Stop();
                if (IsAutoDispose)
                {
                    Dispose();
                }
            }
            return result;
        }

        private void TimeCountting(int dTime)
        {
            if (IsPause || !IsStarted)
            {
                return;
            }

            float t = (float)dTime / UpdatesCacher.UPDATE_CACHER_TIME_SCALE;
            Time += t;
            if (Time >= TotalTime)
            {
                IsTimeCounting = false;
                if (Repeatable && !ShouldRepeat())
                {
                    Completion?.Invoke();
                    if (IsAutoDispose)
                    {
                        Dispose();
                    }
                }
                else
                {
                    if (mCancelCondition != default)
                    {
                        CheckCancelCondition();//附带退出条件的定时器
                    }
                    else
                    {
                        //未完成指定的重复次数、永久重复的计时器开始新一轮定时器周期
                        IsTimeCounting = true;
                        Completion?.Invoke();
                        Time -= TotalTime;
                    }
                }
            }
        }

        public void SetComplete(Action method)
        {
            Completion += method;
        }

        public void SetTimeOffset(float time)
        {
            Time = time;
        }

        public bool IsCompleteCycle
        {
            get
            {
                return HasStart && !IsTimeCounting;
            }
        }

        public int TotalRepeats { get; private set; }
        public float Time { get; private set; }
        public float TotalTime { get; private set; }
        public Action Completion { get; private set; }
        public bool Repeatable { get; private set; }
        public bool HasStart { get; private set; }
        public bool IsTimeCounting { get; private set; }
        public bool IsAutoDispose { get; set; }
        public bool IsStarted { get; private set; }
        public bool IsPause { get; private set; }
    }
}