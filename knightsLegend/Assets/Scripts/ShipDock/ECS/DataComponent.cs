using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
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

        protected abstract T CreateData();

        public virtual void FillEntitasData<E>(ref E target, T data) where E : IShipDockEntitas
        {
            mDatas[target] = data;
        }

        public T GetEntitasData<E>(ref E target) where E : IShipDockEntitas
        {
            return mDatas[target];
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
                DrapData(ref data);
            }
            return base.DropEntitas(target, entitasID);
        }

        protected virtual void DrapData(ref T target)
        {
        }

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

        public bool IsDataValid<E>(ref E target) where E : IShipDockEntitas
        {
            mDataKeys = mDatas.Keys;
            int index = mDataKeys.IndexOf(target);
            return (index != -1) && (mInvalidDatasIndex.IndexOf(index) == -1);
        }
    }
}