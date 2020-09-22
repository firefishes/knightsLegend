#define G_LOG

using UnityEngine;
using System.Collections.Generic;
using ShipDock.Tools;
using ShipDock.Testers;
using ShipDock.Interfaces;
using ShipDock.Applications;

namespace ShipDock.Pooling
{

    /// <summary>
    /// 
    /// 物件池管理器单例
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetsPooling : IDispose
    {

        public delegate void OnGameObjectPoolItem(GameObject target);
        public delegate void OnComponentPoolItem<T>(T target);
        
        #region 私有属性
        /// <summary>以游戏物体预设为模板的对象池最大数量的集合</summary>
        private KeyValueList<int, int> mPoolElmMax;
        /// <summary>以游戏脚本为模板的对象池最大数量的集合</summary>
        private KeyValueList<int, int> mCompPoolElmMax;
        /// <summary>以游戏物体预设为模板的对象池集合</summary>
        private KeyValueList<int, Stack<GameObject>> mPool;
        /// <summary>以游戏脚本为模板的对象池集合</summary>
        private KeyValueList<int, Stack<Component>> mCompPool;
        /// <summary>是否已被销毁</summary>
        private bool mIsDispose;
        #endregion

        public AssetsPooling()
        {
            mPool = new KeyValueList<int, Stack<GameObject>>();
            mCompPool = new KeyValueList<int, Stack<Component>>();
            mPoolElmMax = new KeyValueList<int, int>();
            mCompPoolElmMax = new KeyValueList<int, int>();
        }

        #region 销毁相关
        /// <summary>
        /// 销毁对象池管理器
        /// </summary>
        public void Dispose()
        {
            mIsDispose = true;

            DestroyPool(ref mPool);
            DestroyPool(ref mCompPool);

            Utils.Reclaim(ref mPoolElmMax, false);
            Utils.Reclaim(ref mCompPoolElmMax, false);

            PoolContainer = null;

            mIsDispose = false;
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        private void DestroyPool<T>(ref KeyValueList<int, Stack<T>> pool)
        {
            Stack<T> list;
            int max = pool.Keys.Count;
            for (int i = 0; i < max; i++)
            {
                int key = pool.Keys[i];
                list = pool[key];
                Utils.Reclaim(ref list, true);
            }
            pool.Clear();
        }
        #endregion

        /// <summary>
        /// 清理指定对象池
        /// </summary>
        public void CleanPool(int poolName)
        {
            if (mPool.IsContainsKey(poolName))
            {
                GameObject item;
                Stack<GameObject> list = mPool.GetValue(poolName, true);
                while (list.Count > 0)
                {
                    item = list.Pop();
                    if (item != default)
                    {
                        Object.Destroy(item);
                    }
                }
                Utils.Reclaim(ref list, true);
            }
            if (mCompPool.IsContainsKey(poolName))
            {
                Component item;
                Stack<Component> list = mCompPool.GetValue(poolName, true);
                while (list.Count > 0)
                {
                    item = list.Pop();
                    if (item != default)
                    {
                        Object.Destroy(item.gameObject);
                    }
                }
                Utils.Reclaim(ref list, true);
            }
        }

        #region 获取对象
        /// <summary>
        /// 检测指定对象的池子是否为空
        /// </summary>
        private bool CheckPoolResult<T>(int poolName, ref KeyValueList<int, Stack<T>> pool, ref T result)
        {
            bool isContentable = false;
            if (pool.IsContainsKey(poolName))
            {
                if (pool[poolName].Count > 0)
                {
                    result = pool[poolName].Pop();
                    isContentable = true;
                }
            }
            else
            {
                Stack<T> stack = new Stack<T>();
                pool[poolName] = stack;
            }
            return isContentable;
        }

        /// <summary>
        /// 以脚本类型为模板，从对象池中获取闲置对象
        /// </summary>
        public T FromComponentPool<T>(int poolName, ref T template, OnComponentPoolItem<T> onInit = null, bool visible = true) where T : Component
        {
            "log".Log(template == null, "error: template is null, the type is ".Append(typeof(T).ToString()));

            Component tempResult = null;
            bool flag = CheckPoolResult(poolName, ref mCompPool, ref tempResult);

            if (tempResult == null)
            {
                tempResult = Object.Instantiate(template);
            }
            else
            {
                if (flag && (PoolContainer != null))
                {
                    PoolContainer.Get(tempResult.gameObject);
                }
            }

            T result = (T)tempResult;
            result.gameObject.SetActive(visible);
            onInit?.Invoke(result);

            return result;
        }

        /// <summary>
        /// 以游戏物体或预设为模板，从对象池中获取闲置对象
        /// </summary>
        public GameObject FromPool(int poolName, ref GameObject template, OnGameObjectPoolItem onInit = null, bool visible = true)
        {
            GameObject result = null;
            bool flag = CheckPoolResult(poolName, ref mPool, ref result);

            if (result == null)
            {
                result = Object.Instantiate(template);
            }
            else
            {
                if (flag && (PoolContainer != null))
                {
                    PoolContainer.Get(result);
                }
            }

            result.SetActive(visible);
            onInit?.Invoke(result);

            return result;
        }
        #endregion

        #region 归还对象
        public void ToPool<T>(int poolName, T target, OnComponentPoolItem<T> onRevert = null, bool visible = false) where T : Component
        {
            if (target == null || mIsDispose)
            {
                return;
            }

            onRevert?.Invoke(target);

            if (mCompPool.IsContainsKey(poolName))
            {
                int elmMax = mCompPoolElmMax.IsContainsKey(poolName) ? mCompPoolElmMax[poolName] : int.MaxValue;
                Stack<Component> pool = mCompPool[poolName];
                if (pool.Count < elmMax)
                {
                    pool.Push(target);

                    if (PoolContainer != null)
                    {
                        PoolContainer.Collect(target.gameObject, visible);
                    }
                    else
                    {
                        target.gameObject.SetActive(visible);
                    }
                }
                else
                {
                    Object.DestroyImmediate(target.gameObject);
                }
            }
            else
            {
                Stack<Component> pool = new Stack<Component>();
                mCompPool[poolName] = pool;

                ToPool(poolName, target, onRevert);
            }
        }

        public void ToPool(int poolName, GameObject target, OnGameObjectPoolItem onRevert = null, bool visible = false)
        {
            if (target == null || mIsDispose)
            {
                return;
            }

            onRevert?.Invoke(target);

            if (mPool.IsContainsKey(poolName))
            {
                int elmMax = (mPoolElmMax.IsContainsKey(poolName)) ? mPoolElmMax[poolName] : int.MaxValue;
                Stack<GameObject> pool = mPool[poolName];
                if (pool.Count < elmMax)
                {
                    pool.Push(target);

                    if (PoolContainer != default)
                    {
                        PoolContainer.Collect(target, visible);
                    }
                    else
                    {
                        target.SetActive(visible);
                    }
                }
                else
                {
                    Object.DestroyImmediate(target.gameObject);
                }
            }
            else
            {
                Stack<GameObject> pool = new Stack<GameObject>();
                mPool[poolName] = pool;

                ToPool(poolName, target, onRevert);
            }
        }
        #endregion

        /// <summary>
        /// 设置对象池的最大个数
        /// </summary>
        public void SetPoolMax(int poolName, int max = 0, bool isCompPool = false)
        {
            if ((mPool == null) || (mCompPool == null))
            {
                return;
            }
            if (isCompPool)
            {
                mCompPoolElmMax[poolName] = max;
            }
            else
            {
                mPoolElmMax[poolName] = max;
            }
        }

        public void SetAssetsPoolComp(AssetsPoolingComponent value)
        {
            PoolContainer = value;
        }

        public AssetsPoolingComponent PoolContainer { get; private set; }
    }
}