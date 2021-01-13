using ShipDock.Commons;
using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Ticks
{
    public class UpdatesCacher : IDispose
    {
        public const int UPDATE_CACHER_TIME_SCALE = 10000;

        private int mAddItemNoticeName;
        private int mRemoveItemNoticeName;
        private int mCallLateNoticeName;
        private IUpdate mItem;
        private List<IUpdate> mCacher;
        private List<IUpdate> mDeleted;
        private TicksLater mTicksLater;

        public UpdatesCacher(int addNoticeName, int removeNoticeName, int callLateNoticeName)
        {
            mCacher = new List<IUpdate>();
            mDeleted = new List<IUpdate>();
            mTicksLater = new TicksLater();

            if (addNoticeName != int.MaxValue)
            {
                mAddItemNoticeName = addNoticeName;
                mAddItemNoticeName.Add(OnAddItem);
            }
            if (removeNoticeName != int.MaxValue)
            {
                mRemoveItemNoticeName = removeNoticeName;
                mRemoveItemNoticeName.Add(OnRemoveItem);
            }
            if (callLateNoticeName != int.MaxValue)
            {
                mCallLateNoticeName = callLateNoticeName;
                mCallLateNoticeName.Add(OnAddCallLate);
            }
        }

        public void Dispose()
        {
            IsDisposed = true;

            mAddItemNoticeName.Remove(OnAddItem);
            mRemoveItemNoticeName.Remove(OnRemoveItem);

            Utils.Reclaim(ref mCacher);
            Utils.Reclaim(ref mDeleted);
            Utils.Reclaim(mTicksLater);

            mTicksLater = default;
            mItem = default;
        }

        private void OnRemoveItem(INoticeBase<int> param)
        {
            if(IsDisposed)
            {
                return;
            }

            UpdaterNotice notice = param as UpdaterNotice;
            IUpdate target = notice != default ? notice.ParamValue : default;

            if ((target != default) && !mDeleted.Contains(target))
            {
                mDeleted.Add(target);
            }
        }

        private void OnAddItem(INoticeBase<int> param)
        {
            if (IsDisposed)
            {
                return;
            }

            UpdaterNotice notice = param as UpdaterNotice;
            IUpdate target = notice.ParamValue;
            if (mDeleted.Contains(target))
            {
                mDeleted.Remove(target);
            }
            if (!mCacher.Contains(target))
            {
                mCacher.Add(target);
            }
        }

        private void OnAddCallLate(INoticeBase<int> param)
        {
            if (IsDisposed)
            {
                return;
            }

            ParamNotice<Action<int>> notice = param as ParamNotice<Action<int>>;
            mTicksLater.CallLater(notice.ParamValue);
        }

        public void Update(int time)
        {
            if (IsDisposed)
            {
                return;
            }

            int max = mCacher.Count;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if (mItem.IsUpdate)
                {
                    mItem.OnUpdate(time);
                }
            }
            mItem = default;
        }

        public void FixedUpdate(int time)
        {
            if (IsDisposed)
            {
                return;
            }

            int max = mCacher.Count;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if (mItem != default && mItem.IsFixedUpdate)
                {
                    mItem.OnFixedUpdate(time);
                }
            }
            mItem = default;
        }

        public void LateUpdate()
        {
            if (IsDisposed)
            {
                return;
            }

            int max = mCacher.Count;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if ((mItem != default) && mItem.IsLateUpdate)
                {
                    mItem.OnLateUpdate();
                }
            }
            mItem = default;

            int time = (int)(Time.fixedDeltaTime * UPDATE_CACHER_TIME_SCALE);
            mTicksLater.Update(time);
        }

        public void CheckDeleted()
        {
            if (IsDisposed)
            {
                return;
            }

            int max = mDeleted.Count;
            int cacherCount = mCacher.Count;
            for (int i = 0; i < max; i++)
            {
                mItem = mDeleted[i];
                if (mCacher.Contains(mItem))
                {
                    mCacher.Remove(mItem);
                }
            }
            mItem = default;
            mDeleted.Clear();

            if (max > (cacherCount * 0.5f))
            {
                mDeleted.TrimExcess();
            }
        }

        public bool IsDisposed { get; private set; }
    }
}
