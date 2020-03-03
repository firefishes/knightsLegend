
using ShipDock.Interfaces;

namespace ShipDock.Tools
{
    /// <summary>
    /// 
    /// 自增id工具类
    /// 
    /// id 最大值为 int.MaxValue，达到最大之后自动扩容
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class IntegerID<T> : IDispose
    {
        private IntegerID<T> mNext;
        private KeyValueList<T, int> mMap;
        
        public IntegerID()
        {
            mMap = new KeyValueList<T, int>();
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private IntegerID<T> AddNext()
        {
            mNext = new IntegerID<T>();
            return mNext;
        }

        /// <summary>
        /// 从子节点中查找id
        /// </summary>
        private int GetIDInNext(ref T target)
        {
            int result = 0;
            IntegerID<T> next = !HasNext ? AddNext() : mNext;
            while (next != null)
            {
                result = next.GetID(ref target);
                if (result < int.MaxValue)
                {
                    return result;
                }
                else
                {
                    next = next.Next;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取id，如未找到id则通过自增的方式生成新id
        /// </summary>
        public int GetID(ref T target)
        {
            int result = int.MaxValue;
            if(mMap.IsContainsKey(target))
            {
                result = mMap[target];
            }
            else
            {
                if(MaxID == (int.MaxValue - 1))
                {
                    result = GetIDInNext(ref target);
                    return result;
                }
                else
                {
                    result = MaxID++;
                    mMap[target] = result;
                }
            }
            return result;
        }

        /// <summary>
        /// 销毁本体节点
        /// </summary>
        public void Dispose()
        {
            Purge(ref mNext);
        }

        /// <summary>
        /// 销毁子节点
        /// </summary>
        private void Purge(ref IntegerID<T> next, bool isDispose = true)
        {
            MaxID = 0;
            if (next.HasNext)
            {
                Utils.Reclaim(next);
            }
            next = null;
            if(isDispose)
            {
                Utils.Reclaim(ref mMap);
            }
            else
            {
                mMap.Clear();
            }
        }
        
        public void Clear()
        {
            Purge(ref mNext, false);
        }
        
        public bool HasNext
        {
            get
            {
                return mNext != null;
            }
        }
        
        public IntegerID<T> Next
        {
            get
            {
                return mNext;
            }
        }

        public int MaxID { get; private set; } = 0;
    }

}