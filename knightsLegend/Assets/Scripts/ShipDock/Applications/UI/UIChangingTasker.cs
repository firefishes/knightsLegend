using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class UIChangingTasker
    {
        private string mCurrentTaskName;
        private List<string> mStartedChangeTask = new List<string>();
        private List<int> mFinishTaskIndex = new List<int>();
        private List<TimeGapper> mChangeTimes = new List<TimeGapper>();
        private KeyValueList<string, Action<TimeGapper>> mChangeHnadlers = new KeyValueList<string, Action<TimeGapper>>();

        public UIChangingTasker(UI.UI target)
        {
            Owner = target;
        }

        public void Clean()
        {
            Utils.Reclaim(ref mStartedChangeTask);
            Utils.Reclaim(ref mFinishTaskIndex);
            Utils.Reclaim(ref mChangeTimes);
            Utils.Reclaim(ref mChangeHnadlers);
            Owner = default;
        }

        public void AddChangeTask(string taskName, float duringTime, Action<TimeGapper> handler)
        {
            TimeGapper timeGapper;
            if (!mStartedChangeTask.Contains(taskName))
            {
                timeGapper = new TimeGapper();
                timeGapper.Start(duringTime);
                mChangeTimes.Add(timeGapper);

                mStartedChangeTask.Add(taskName);
                mChangeHnadlers[taskName] = handler;
            }
            else
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                mFinishTaskIndex.Remove(index);

                timeGapper = mChangeTimes[index];
                timeGapper.Start(duringTime);
                mChangeTimes[index] = timeGapper;
            }
            Owner.UIChanged = true;
        }

        public void UpdateUITasks()
        {
            if (Owner != default)
            {
                bool isChangeFinish = false;
                int max = mStartedChangeTask.Count;

                TimeGapper gapper;
                for (int i = 0; i < max; i++)
                {
                    if (mFinishTaskIndex.IndexOf(i) == -1)
                    {
                        gapper = mChangeTimes[i];
                        isChangeFinish = gapper.TimeAdvanced(Time.deltaTime);
                        mChangeTimes[i] = gapper;
                        if (isChangeFinish)
                        {
                            mFinishTaskIndex.Add(i);
                        }
                        else
                        {
                            mCurrentTaskName = mStartedChangeTask[i];
                            mChangeHnadlers[mCurrentTaskName].Invoke(gapper);
                        }
                    }
                }

                Owner.UIChanged = !isChangeFinish;
            }
        }

        private UI.UI Owner { get; set; }

    }
}