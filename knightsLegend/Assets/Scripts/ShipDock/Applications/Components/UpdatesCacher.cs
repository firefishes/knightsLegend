using System.Collections.Generic;
using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class UpdatesCacher : IDispose
    {
        public const int UPDATE_CACHER_TIME_SCALE = 10000;

        private int mAddItemNoticeName;
        private int mRemoveItemNoticeName;
        private IUpdate mItem;
        private List<IUpdate> mCacher;
        private List<IUpdate> mDeleted;

        public UpdatesCacher(int addNoticeName, int removeNoticeName)
        {
            mCacher = new List<IUpdate>();
            mDeleted = new List<IUpdate>();

            if(addNoticeName != int.MaxValue)
            {
                mAddItemNoticeName = addNoticeName;
                mAddItemNoticeName.Add(OnAddItem);
            }
            if(removeNoticeName != int.MaxValue)
            {
                mRemoveItemNoticeName = removeNoticeName;
                mRemoveItemNoticeName.Add(OnRemoveItem);
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
            Utils.Reclaim(ref mCacher);
            Utils.Reclaim(ref mDeleted);

            mAddItemNoticeName.Remove(OnAddItem);
            mRemoveItemNoticeName.Remove(OnRemoveItem);
        }

        private void OnRemoveItem(INoticeBase<int> param)
        {
            if(IsDisposed)
            {
                return;
            }

            UpdaterNotice notice = param as UpdaterNotice;
            IUpdate target = notice.ParamValue;

            if (!mDeleted.Contains(target))
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
                if (mItem.IsFixedUpdate)
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
                if (mItem.IsLateUpdate)
                {
                    mItem.OnLateUpdate();
                }
            }
            mItem = default;
        }

        public void CheckDeleted()
        {
            if (IsDisposed)
            {
                return;
            }

            int max = mDeleted.Count;
            bool shoulddTrimExcess = mDeleted.Count > mCacher.Count / 2;
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
            if (shoulddTrimExcess)
            {
                mDeleted.TrimExcess();
            }
        }

        public bool IsDisposed { get; private set; }
    }
}
