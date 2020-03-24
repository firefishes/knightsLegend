﻿using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{

    public class TimingTasker
    {
        public float dTime;
        public TimeGapper timeGapper;
        public Action completion;

        public TimingTasker(int name, ref KeyValueList<int, TimingTasker> mapper)
        {
            timeGapper = new TimeGapper();
            mapper[name] = this;
            completion += UpdateRunCounts;
            Name = name;
        }

        public void ResetRunCounts()
        {
            RunCounts = 0;
        }

        public void DirectCompletion()
        {
            completion?.Invoke();
        }

        private void UpdateRunCounts()
        {
            RunCounts++;
        }

        public bool IsStart
        {
            get
            {
                return timeGapper.isStart;
            }
        }

        public int RunCounts { get; private set; }
        public int Name { get; private set; }
    }

}