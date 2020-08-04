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

        public void FillEntitasData(ref IShipDockEntitas target, T data)
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
            int statu = base.DropEntitas(target, entitasID);
            if (statu == 0)
            {
                mDataKeys.Remove(target);
                int index = mDataKeys.IndexOf(target);
                if (index != -1)
                {
                    mInvalidDatasIndex.Remove(index);
                }
            }
            return statu;
        }

        public void SetDataValidable<E>(bool value, ref E target) where E : IShipDockEntitas
        {
            mDataKeys = mDatas.Keys;
            int index = mDataKeys.IndexOf(target);
            if (value)
            {
                if (index >= 0 && mInvalidDatasIndex.Contains(index))
                {
                    mInvalidDatasIndex.Remove(index);
                }
            }
            else
            {
                if (index >= 0 && !mInvalidDatasIndex.Contains(index))
                {
                    mInvalidDatasIndex.Add(index);
                }
            }
        }

        public bool IsDataValid(ref IShipDockEntitas target)
        {
            mDataKeys = mDatas.Keys;
            int index = mDataKeys.IndexOf(target);
            return (index != -1) && (mInvalidDatasIndex.IndexOf(index) == -1);
        }
    }
}