using ShipDock.Tools;

namespace ShipDock.Datas
{
    /// <summary>
    /// 
    /// 数据代理管理器
    /// 
    /// </summary>
    public class DataWarehouse
    {
        private KeyValueList<int, IDataProxy> mDataMapper;

        public DataWarehouse()
        {
            mDataMapper = new KeyValueList<int, IDataProxy>();
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mDataMapper, true, true);
        }

        public void AddData(IDataProxy target)
        {
            int name = target.DataName;
            if(mDataMapper.ContainsKey(name))
            {
                return;
            }

            mDataMapper[name] = target;
        }

        public void RemoveData(IDataProxy target)
        {
            int name = target.DataName;
            if (!mDataMapper.ContainsKey(name))
            {
                return;
            }

            mDataMapper.Remove(name);
        }

        public T GetData<T>(int dataName) where T : IDataProxy
        {
            return ((mDataMapper != default) && mDataMapper.IsContainsKey(dataName)) ? (T)mDataMapper[dataName] : default;
        }
    }

}