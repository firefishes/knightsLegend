using ShipDock.Interfaces;
using System;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    public class DoubleBuffers<T> : IDispose
    {
        private bool mIsDisposed;
        private bool mIsFront;
        private Queue<T> mCacheFront;
        private Queue<T> mCacheBack;
        private Queue<T> mCache;
        private Queue<T> mEnqueueCache;

        public DoubleBuffers()
        {
            mCacheFront = new Queue<T>();
            mCacheBack = new Queue<T>();
            mCache = mCacheFront;
            mEnqueueCache = mCacheBack;
            mIsFront = true;
        }

        public virtual void Dispose()
        {
            mIsDisposed = true;
            Current = default;
            OnDequeue = default;
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
            if (!mIsDisposed)
            {
                mCache = mIsFront ? mCacheBack : mCacheFront;//切换到需要处理的队列
                mEnqueueCache = mIsFront ? mCacheFront : mCacheBack;
                if (mCache != default)//执行处理中的队列
                {
                    int max = mCache.Count;
                    if (max > 0)
                    {
                        while (mCache.Count > 0)
                        {
                            Current = mCache.Dequeue();
                            OnDequeue?.Invoke(dTime, Current);
                        }
                    }
                }
                mIsFront = !mIsFront;
            }
            Current = default;
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void Enqueue(T target, bool isCheckContains = true)
        {
            if (!mIsDisposed)
            {
                if (mEnqueueCache != default)
                {
                    if (isCheckContains)
                    {
                        if (!mEnqueueCache.Contains(target))
                        {
                            mEnqueueCache.Enqueue(target);
                        }
                    }
                    else
                    {
                        mEnqueueCache.Enqueue(target);
                    }
                }
            }
        }

        public T Current { get; private set; }
        public Action<int, T> OnDequeue { get; set; }
    }
}
