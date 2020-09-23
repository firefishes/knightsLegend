#define G_LOG

using ShipDock.Testers;
using System;
using System.Collections.Generic;

namespace ShipDock.Pooling
{

    /// <summary>
    /// 
    /// 泛型对象池类
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

        private static void CheckPoolNull(Pooling<T> customPool, Func<T> func = null)
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

        public static T From(Pooling<T> customPool = null)
        {
            CheckPoolNull(customPool);
            return instance.FromPool();
        }

        public static void To(T target, Pooling<T> customPool = null)
        {
            "pool type error".Log((typeof(T).FullName != target.GetType().FullName), target.GetType().FullName);
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
            CheckPoolNull(null);
            return instance.CheckUsed(target);
        }
        #endregion

        private string mPoolTypeName;
        private bool mIsAddResetCallback = true;
        private Stack<T> mPool;
        private Func<T> mCreater;

        /// <summary>对象池构造函数</summary>
        public Pooling(Func<T> customCreater = null, Stack<T> pool = null)
        {
            if (instance == null)
            {
                instance = this;
            }

            if (pool != null)
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
        }

        /// <summary>销毁对象池</summary>
        public virtual void Dispose()
        {
#if UNITY_EDITOR
            AllPools.used.Clear();
#endif
            ClearPool();
            mCreater = null;
            mPool = null;
        }

        /// <summary>获取一个对象</summary>
        public virtual T FromPool(Func<T> creater = null)
        {
            T result = default;
            if (mPool.Count > 0)
            {
                result = mPool.Count > 0 ? mPool.Pop() : default;
                if (result == default)
                {
                    return FromPool(creater);
                }
            }
            else
            {
                if (creater != null)
                {
                    result = creater();
                }
                else
                {
                    if (mCreater != null)
                    {
                        result = mCreater();
                    }
                    else
                    {
                        result = new T();
                    }
                }
            }
            UsedCount++;

#if UNITY_EDITOR
            //if (!AllPools.used.Contains(result))
            //{
            //    AllPools.used.Add(result);
            //}
#endif
            return result;
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
            if (target == null)
            {
                return false;
            }

            bool result = true;
            if (mPool != null)
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
            if (mPool != null)
            {
                int max = mPool.Count;
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
                "pool clear".Log(mPoolTypeName);
            }
            UsedCount = 0;
        }

        public IPoolable GetInstance()
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