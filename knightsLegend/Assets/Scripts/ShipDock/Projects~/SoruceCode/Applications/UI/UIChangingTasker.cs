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
            mStartedChangeTask?.Clear();
            mFinishTaskIndex?.Clear();
            mChangeTimes = default;

            int max = mChangeHnadlers.Keys.Count;
            string key;
            for (int i = 0; i < max; i++)
            {
                key = mChangeHnadlers.Keys[i];
                mChangeHnadlers[key] = default;
            }

            Owner = default;
        }

        public bool HasTask(string taskName)
        {
            return mStartedChangeTask.Contains(taskName);
        }

        public bool IsTaskStoped(string taskName)
        {
            bool result = false;
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                if (index != -1)
                {
                    if (!mFinishTaskIndex.Contains(index))
                    {
                        mFinishTaskIndex.Add(index);
                        TimeGapper timeGapper = mChangeTimes[index];
                        result = timeGapper.IsFinised;
                    }
                    else { }
                }
                else { }
            }
            else { }
            return result;
        }

        public void StopChangeTask(string taskName)
        {
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                if (index != -1)
                {
                    if (!mFinishTaskIndex.Contains(index))
                    {
                        mFinishTaskIndex.Add(index);
                        TimeGapper timeGapper = mChangeTimes[index];
                        timeGapper.Stop();

                        mChangeTimes[index] = timeGapper;
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        public void AddChangeTask(string taskName, float duringTime, Action<TimeGapper> handler)
        {
            TimeGapper timeGapper;
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                mFinishTaskIndex.Remove(index);

                timeGapper = mChangeTimes[index];
                timeGapper.Start(duringTime);
                mChangeTimes[index] = timeGapper;
            }
            else
            {
                timeGapper = new TimeGapper();
                timeGapper.Start(duringTime);
                mChangeTimes.Add(timeGapper);

                mStartedChangeTask.Add(taskName);
                mChangeHnadlers[taskName] = handler;
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
                    else { }
                }

                Owner.UIChanged = !isChangeFinish;
            }
            else { }
        }

        private UI.UI Owner { get; set; }

    }
}