#define _G_LOG

using System;
using System.Collections.Generic;

namespace ShipDock.Pooling
{

    /// <summary>
    /// 
    /// 泛型对象池
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Pooling<T> : IPool<T> where T : class, IPoolable, new()
    {
        #region 静态成员
        private static Pooling<T> instance;

        public static Pooling<T> Instance
        {
            get
            {
                CheckPoolNull(null);
                return instance;
            }
        }

        public static Pooling<T> InitByCustom(Pooling<T> customPool, Func<T> func)
        {
            CheckPoolNull(customPool, func);
            return instance;
        }

        private static void CheckPoolNull(Pooling<T> customPool, Func<T> func = default)
        {
            if (instance == null)
            {
                if (customPool != null)
                {
                    instance = customPool;
                }
                else
                {
                    instance = new Pooling<T>(func ?? (() => { return new T(); }));
                }
            }
        }

        public static T From(Pooling<T> customPool = default)
        {
            CheckPoolNull(customPool);
            return instance.FromPool();
        }

        public static void To(T target, Pooling<T> customPool = default)
        {
            "pool type error".Log((target != default) && (typeof(T).FullName != target.GetType().FullName), target.GetType().FullName);
            CheckPoolNull(customPool);
            instance.ToPool(target);
        }

        public static bool CheckAndRevert(T item)
        {
            bool result = IsUsed(item);
            if (result)
            {
                To(item);
            }
            return result;
        }

        public static bool IsUsed(T target)
        {
            CheckPoolNull(default);
            return instance.CheckUsed(target);
        }
        #endregion

        private string mPoolTypeName;

        private object mLock;
        private bool mIsAddResetCallback = true;
        private Stack<T> mPool;
        private Func<T> mCreater;

        /// <summary>对象池构造函数</summary>
        public Pooling(Func<T> customCreater = default, Stack<T> pool = default)
        {
            if (instance == default)
            {
                instance = this;
            }

            if (pool != default)
            {
                mPool = pool;
            }
            else
            {
                mPool = new Stack<T>();
            }

            mPoolTypeName = typeof(T).FullName;
            mCreater = customCreater;

            if (mIsAddResetCallback)
            {
                AllPools.AddReset(ClearPool);
            }
            mLock = new object();
        }

        /// <summary>销毁对象池</summary>
        public virtual void Dispose()
        {
#if UNITY_EDITOR
            AllPools.used.Clear();
#endif
            ClearPool();
            mCreater = default;
            mPool = default;
            mLock = default;
            instance = default;
        }

        private int mInstanceCount = 0;

        /// <summary>获取一个对象</summary>
        public virtual T FromPool(Func<T> creater = default)
        {
            lock (mLock)
            {
                T result = default;
                if (mInstanceCount > 2)
                {
                    mInstanceCount--;
                    result = mPool.Pop();
                }
                else
                {
                    if (creater != default)
                    {
                        result = creater();
                    }
                    else
                    {
                        result = (mCreater != default) ? mCreater() : new T();
                    }
                }
                UsedCount++;

#if UNITY_EDITOR
                //if (!AllPools.used.Contains(result))
                //{
                //    AllPools.used.Add(result);
                //}
#endif
                if (result == default)
                {
                    result = new T();
                }

                return result;
            }
        }

        /// <summary>重置并归还一个对象</summary>
        public virtual void ToPool(T target)
        {
            if (mPool == default)
            {
                return;
            }

            target.Revert();

            if (!mPool.Contains(target))
            {
                mPool.Push(target);
                mInstanceCount++;
                UsedCount--;
            }
#if UNITY_EDITOR
            //if (AllPools.used.Contains(target))
            //{
            //    AllPools.used.Remove(target);
            //}
#endif
        }

        /// <summary>检测一个对象是否正在使用</summary>
        public bool CheckUsed(T target)
        {
            if (target == default)
            {
                return false;
            }

            bool result = true;
            if (mPool != default)
            {
                if (mPool.Contains(target))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>重置池中所有对象</summary>
        public void ClearPool()
        {
            if (mPool != default)
            {
                int max = mPool.Count;
                "pool clear".Log(max > 0, mPoolTypeName.Append(", count is ", max.ToString()));
                for (int i = 0; i < max; i++)
                {
                    T item = mPool.Pop();
                    item.Revert();
                    if (item is IDisposable)
                    {
                        (item as IDisposable).Dispose();
                    }
                }
                mPool.Clear();
            }
            UsedCount = 0;
        }

        public IPoolable Create()
        {
            return FromPool() as IPoolable;
        }

        public void Reserve(ref IPoolable item)
        {
            ToPool((T)item);
        }

        /// <summary>获取当前对象池中对象的数量</summary>
        public int UsedCount { get; private set; }
    }
}