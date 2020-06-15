using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class TicksUpdater : IDispose, INotificationSender
    {
        public const int TICKS_FIXED_UPDATE = 0;
        public const int TICKS_UPDATE = 1;
        public const int TICKS_LATE_UPDATE = 2;

        private int mSize;
        private int mIndex;
        private int mFixedUpdateDeltaTime;
        private int mFixedUpdateCountTime;
        private bool mEnable;
        private bool mIsDisposed;
        private IUpdate mItem;
        private IUpdate mItemAdded;
        private IUpdate mItemRemoved;
        private List<IUpdate> mTicksList;
        private List<IUpdate> mListDeleted;
        private TicksLater mTicksLater; 
        private ThreadTicks mThreadTicks;
        private UpdaterNotice mNoticeAdded;
        private UpdaterNotice mNoticeRemoved;

        public TicksUpdater(int tickTime, float fixedUpdateTime = 0.3f)
        {
            FixedUpdateTime = fixedUpdateTime;
            mFixedUpdateDeltaTime = (int)(fixedUpdateTime * 1000);
            mFixedUpdateCountTime = mFixedUpdateDeltaTime;
            if (mThreadTicks == null)
            {
                mThreadTicks = new ThreadTicks(tickTime);
                mThreadTicks.Add(Updating);
                mThreadTicks.Start();
            }
            mTicksLater = new TicksLater();
            mTicksList = new List<IUpdate>();
            mListDeleted = new List<IUpdate>();

            Enabled();
        }

        public void Dispose()
        {
            mIsDisposed = true;
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Remove(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Remove(AddUpdate);
            
            Utils.Reclaim(ref mTicksList);
            Utils.Reclaim(ref mListDeleted);
            Utils.Reclaim(mTicksLater);
            Utils.Reclaim(mThreadTicks);

            mItem = null;
            mItemAdded = null;
            mItemRemoved = null;
            mNoticeAdded = null;
            mNoticeRemoved = null;
            mTicksLater = null;
            mThreadTicks = null;
        }

        private void Enabled()
        {
            if (mIsDisposed)
            {
                return;
            }
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Add(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Add(AddUpdate);
            ShipDockConsts.NOTICE_FRAME_UPDATER_COMP_READY.Broadcast();
        }

        private void Disabled()
        {
            if (mIsDisposed)
            {
                return;
            }
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Remove(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Remove(AddUpdate);
        }
        
        private void AddUpdaterItem(float time)
        {
            if (mIsDisposed || mItemAdded == null)
            {
                return;
            }
            if ((mTicksList != null) && (mTicksList.IndexOf(mItemAdded) == -1))
            {
                mTicksList.Add(mItemAdded);
                mItemAdded.AddUpdate();
            }
            if ((mListDeleted != null) && mListDeleted.Contains(mItemAdded))
            {
                mListDeleted.Remove(mItemAdded);//清除之前添加过的移除刷帧标记，避免最新的队列标记不生效
            }
            mItemAdded = null;
        }

        private void RemoveUpdaterItem(float time)
        {
            if (mIsDisposed)
            {
                return;
            }
            if ((mItemRemoved != null) && (mListDeleted != null) && (mListDeleted.IndexOf(mItemRemoved) == -1))
            {
                mListDeleted.Add(mItemRemoved);//加入删除列表，下一次帧周期中统一移除
            }
        }

        /// <summary>添加一个需要刷帧的对象</summary>
        protected virtual void AddUpdate(INoticeBase<int> param)
        {
            mNoticeAdded = param as UpdaterNotice;
            if ((mNoticeAdded == null) || 
                (mNoticeAdded.ParamValue == null) || 
                (mNoticeAdded.NotifcationSender != null && !mNoticeAdded.CheckSender(this)))
            {
                return;
            }

            mItemAdded = mNoticeAdded.ParamValue;
            AddUpdaterItem(0);
            mNoticeAdded = null;
        }

        /// <summary>移除一个需要刷帧的对象</summary>
        protected virtual void RemoveUpdate(INoticeBase<int> param)
        {
            mNoticeRemoved = param as UpdaterNotice;
            if (mNoticeRemoved != null || 
                mNoticeRemoved.ParamValue == null ||
                (mNoticeAdded.NotifcationSender != null && !mNoticeAdded.CheckSender(this)))
            {
                return;
            }
            mItemRemoved = mNoticeRemoved.ParamValue;
            RemoveUpdaterItem(0);
            mNoticeRemoved = null;
        }

        /// <summary>检测一个刷帧对象是否有效</summary>
        private bool IsValidUpdate(IUpdate target)
        {
            return (target != default) && (mListDeleted != null) && !mListDeleted.Contains(target);
        }
        
        private void Updating(int time)
        {
            if (mIsDisposed)
            {
                return;
            }
            RunTime += time * 0.001f;

            CheckRemoveUpdate();
            WalkUpdateItems(time, TICKS_FIXED_UPDATE);
            WalkUpdateItems(time, TICKS_UPDATE);
            WalkUpdateItems(time, TICKS_LATE_UPDATE);
            mTicksLater.Update(time);

            LastRunTime = RunTime;
        }

        private void WalkUpdateItems(int time, int methodType)
        {
            mIndex = 0;
            mSize = (mTicksList != default) ? mTicksList.Count : 0;
            for (mIndex = 0; mIndex < mSize; mIndex++)
            {
                if(mTicksList == null)
                {
                    break;
                }
                mItem = mTicksList[mIndex];
                if (IsValidUpdate(mItem))
                {
                    CallUpdateMethodByType(time, methodType);
                }
            }
        }

        private void CallUpdateMethodByType(int time, int methodType)
        {
            switch(methodType)
            {
                case TICKS_FIXED_UPDATE:
                    if(mItem.IsFixedUpdate)
                    {
                        mFixedUpdateCountTime -= time;
                        if (mFixedUpdateCountTime <= 0)
                        {
                            mItem.OnFixedUpdate(mFixedUpdateDeltaTime);
                            mFixedUpdateCountTime += mFixedUpdateDeltaTime;
                        }
                    }
                    break;
                case TICKS_UPDATE:
                    if(mItem.IsUpdate)
                    {
                        mTicksLater?.Update(time);
                        mItem?.OnUpdate(time);
                    }
                    break;
                case TICKS_LATE_UPDATE:
                    if(mItem.IsLateUpdate)
                    {
                        mItem.OnLateUpdate();
                    }
                    break;
            }
        }

        public void CallLater(Action<int> method)
        {
            mTicksLater.CallLater(method);
        }

        /// <summary>检测已被标记为移除的刷帧对象</summary>
        protected void CheckRemoveUpdate()
        {
            mSize = mListDeleted.Count;
            if (mSize > 0)
            {
                for (mIndex = 0; mIndex < mSize; mIndex++)
                {
                    mItem = mListDeleted[mIndex];
                    mTicksList.Remove(mItem);
                    mItem.RemoveUpdate();
                }
                mListDeleted.Clear();
            }
        }

        public bool Enable
        {
            set
            {
                mEnable = value;
                if(mEnable)
                {
                    Enabled();
                }
                else
                {
                    Disabled();
                }
            }
            get
            {
                return mEnable;
            }
        }

        public float DeltaTime
        {
            get
            {
                return LastRunTime - RunTime;
            }
        }

        public float FixedUpdateTime { get; private set; }
        public float RunTime { get; private set; }
        public float LastRunTime { get; private set; }
    }
}
