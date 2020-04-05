using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class TicksLater : IDispose
    {
        private bool mIsDisposed;
        private Action<int> mAction;
        private Queue<Action<int>> mCachesTemp;
        private Queue<Action<int>> mCacheFront;
        private Queue<Action<int>> mCacheBack;
        private Queue<Action<int>> mCallLateCaches;

        public TicksLater()
        {
            mCacheFront = new Queue<Action<int>>();
            mCacheBack = new Queue<Action<int>>();
            mCallLateCaches = mCacheFront;
        }

        public void Dispose()
        {
            mIsDisposed = true;
            Utils.Reclaim(ref mCachesTemp);
            Utils.Reclaim(ref mCacheFront);
            Utils.Reclaim(ref mCacheBack);
            Utils.Reclaim(ref mCallLateCaches);
        }

        public void Update(int dTime)
        {
            mCachesTemp = mCallLateCaches;//设置需要调用的单帧函数队列
            if (!mIsDisposed && mCachesTemp != null)
            {
                mCallLateCaches = (mCallLateCaches == mCacheFront) ? mCacheBack : mCacheFront;//切换单帧函数队列
                if (mCachesTemp != null)//执行单帧函数队列
                {
                    int max = mCachesTemp.Count;
                    if (max > 0)
                    {
                        while (mCachesTemp != default && mCachesTemp.Count > 0)
                        {
                            mAction = mCachesTemp.Dequeue();
                            mAction?.Invoke(dTime);
                        }
                    }
                }
            }
            mCachesTemp = null;
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void CallLater(Action<int> method)
        {
            if (!mIsDisposed && (mCallLateCaches != null) && !mCallLateCaches.Contains(method))
            {
                mCallLateCaches.Enqueue(method);
            }
        }
    }

}
