using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    /// <summary>
    /// ECS数据组件
    /// 
    /// 组件与实体为一对多的关系，组件中的数据与实体的关系则是一一对应的关系
    /// 
    /// </summary>
    /// <typeparam name="T">组件数据泛型</typeparam>
    public abstract class DataComponent<T> : ShipDockComponent, IDataComponent<T>
    {
        protected KeyValueList<IShipDockEntitas, T> mDatas;

        private List<int> mInvalidDatasIndex;
        private List<IShipDockEntitas> mDataKeys;

        public DataComponent()
        {
            IsSystem = false;
            mInvalidDatasIndex = new List<int>();
            mDatas = new KeyValueList<IShipDockEntitas, T>();
        }

        public override void Dispose()
        {
            base.Dispose();

            Utils.Reclaim(ref mInvalidDatasIndex);
            Utils.Reclaim(ref mDatas);

            mDataKeys = default;
        }

        /// <summary>
        /// 实现此方法，创建组件数据对象
        /// </summary>
        protected abstract T CreateData();

        /// <summary>
        /// 覆盖此方法，填充组件数据
        /// </summary>
        public virtual void FillEntitasData<E>(ref E target, T data) where E : IShipDockEntitas
        {
            mDatas[target] = data;
        }

        /// <summary>
        /// 获取已添加此组件的实体相对应的数据
        /// </summary>
        public T GetEntitasData<E>(ref E target) where E : IShipDockEntitas
        {
            return mDatas[target];
        }

        /// <summary>
        /// 判定指定实体是否包含有组件数据
        /// </summary>
        public bool HasEntitasData<E>(ref E target) where E : IShipDockEntitas
        {
            return mDatas.ContainsKey(target);
        }

        public override int SetEntitas(IShipDockEntitas target)
        {
            int id = base.SetEntitas(target);

            FillEntitasData(ref target, CreateData());

            return id;
        }

        public override int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            if (mDatas.ContainsKey(target))
            {
                mDataKeys.Remove(target);
                int index = mDataKeys.IndexOf(target);
                if (index != -1)
                {
                    mInvalidDatasIndex.Remove(index);
                }
                T data = mDatas.Remove(target);
                DropData(ref data);
            }
            return base.DropEntitas(target, entitasID);
        }

        /// <summary>
        /// 废弃已添加此组件对应的数据
        /// </summary>
        protected virtual void DropData(ref T target)
        {
        }

        /// <summary>
        /// 设置指定实体对应的数据为是否有效
        /// </summary>
        public void SetDataValidable<E>(bool value, ref E target) where E : IShipDockEntitas
        {
            mDataKeys = mDatas.Keys;
            int index = mDataKeys.IndexOf(target);
            if (index >= 0)
            {
                if (value)
                {
                    if (mInvalidDatasIndex.Contains(index))
                    {
                        mInvalidDatasIndex.Remove(index);
                    }
                }
                else
                {
                    if (!mInvalidDatasIndex.Contains(index))
                    {
                        mInvalidDatasIndex.Add(index);
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定实体对应的数据是否有效
        /// </summary>
        public bool IsDataValid<E>(ref E target) where E : IShipDockEntitas
        {
            mDataKeys = mDatas.Keys;
            int index = mDataKeys.IndexOf(target);
            return (index != -1) && (mInvalidDatasIndex.IndexOf(index) == -1);
        }
    }
}