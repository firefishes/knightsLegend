using ShipDock.Interfaces;
using System;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    public class DoubleBuffers<T> : IDispose
    {
        private bool mIsDisposed;
        private bool mIsFront;
        private Queue<T> mCachesTemp;
        private Queue<T> mCacheFront;
        private Queue<T> mCacheBack;
        private Queue<T> mCache;

        public DoubleBuffers()
        {
            mCacheFront = new Queue<T>();
            mCacheBack = new Queue<T>();
            mCache = mCacheFront;
            mIsFront = true;
        }

        public virtual void Dispose()
        {
            mIsDisposed = true;
            Current = default;
            OnDequeue = default;
            Utils.Reclaim(ref mCachesTemp);
            Utils.Reclaim(ref mCacheFront);
            Utils.Reclaim(ref mCacheBack);
            Utils.Reclaim(ref mCache);
        }

        public void Step(int dTime)
        {
            if (!mIsDisposed && mCache != default)
            {
                if (mCache != default)//执行处理中的队列
                {
                    int max = mCache.Count;
                    if (max > 0)
                    {
                        Current = mCache.Dequeue();
                        OnDequeue?.Invoke(dTime, Current);
                    }
                }
            }
            Current = default;
        }

        public void Update(int dTime)
        {
            mCachesTemp = mCache;//设置处理中的的队列
            if (!mIsDisposed && mCachesTemp != default)
            {
                mCache = mIsFront ? mCacheBack : mCacheFront;//切换队列
                if (mCachesTemp != default)//执行处理中的队列
                {
                    int max = mCachesTemp.Count;
                    if (max > 0)
                    {
                        while ((mCachesTemp != default) && (mCachesTemp.Count > 0))
                        {
                            Current = mCachesTemp.Dequeue();
                            OnDequeue?.Invoke(dTime, Current);
                        }
                    }
                }
                mIsFront = !mIsFront;
            }
            Current = default;
            mCachesTemp = default;
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void Enqueue(T target, bool isCheckContains = true)
        {
            if (!mIsDisposed && (mCache != default))
            {
                if (isCheckContains)
                {
                    if (!mCache.Contains(target))
                    {
                        mCache.Enqueue(target);
                    }
                }
                else
                {
                    mCache.Enqueue(target);
                }
            }
        }

        public T Current { get; private set; }
        public Action<int, T> OnDequeue { get; set; }
    }
}
