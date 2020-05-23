using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class TimingTasker : IDispose
    {
        public float dTime;
        public TimeGapper timeGapper;
        public Action completion;

        private int mState;

        public TimingTasker(int name, ref KeyValueList<int, TimingTasker> mapper)
        {
            mState = 0;
            timeGapper = new TimeGapper();
            mapper[name] = this;
            Name = name;
        }

        public void Dispose()
        {
            completion = default;
            timeGapper.totalTime = 0f;
        }

        public void Stop(bool onlyChangeState = false)
        {
            mState = 2;
            if (!onlyChangeState)
            {
                timeGapper.Stop();
            }
        }

        public void Start(float totalTime)
        {
            if (!ShouldRun())
            {
                return;
            }
            if (TotalCount > 0)
            {
                RunCounts++;
            }
            mState = 1;
            timeGapper.Start(totalTime);
        }
        
        public void Restart()
        {
            mState = 1;
            timeGapper.Start();
        }

        public void Reset()
        {
            RunCounts = 0;
            mState = mState != 0 ? 1 : mState;
            timeGapper.Stop();
        }

        public void ResetRunCounts()
        {
            RunCounts = 0;
        }

        public void DirectCompletion()
        {
            timeGapper.Stop();
            completion?.Invoke();
        }

        public bool ShouldRun()
        {
            return (TotalCount > 0) ? RunCounts < TotalCount : true;
        }

        public bool IsStart
        {
            get
            {
                return timeGapper.isStart;
            }
        }

        public bool IsFinish
        {
            get
            {
                return !timeGapper.isStart && mState > 1;
            }
        }

        public int TotalCount { get; set; } = 0;
        public int RunCounts { get; private set; }
        public int Name { get; private set; }
    }

}