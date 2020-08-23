using ShipDock.Tools;

namespace ShipDock.Datas
{
    public class DataStorager : DataProxy
    {
        private KeyValueList<int, IDataUnit> mStorager;

        public DataStorager(int dataName) : base(dataName)
        {
            mStorager = new KeyValueList<int, IDataUnit>();
        }

        public bool HasDataUnit(int key)
        {
            return mStorager.ContainsKey(key);
        }

        public void SetDataUnit(int key, IDataUnit data)
        {
            if (data == default)
            {
                mStorager.Remove(key);
            }
            else
            {
                mStorager.Put(key, data);
            }
        }

        public IDataUnit GetDataUnit<T>(int key) where T : IDataUnit
        {
            IDataUnit result = mStorager[key];
            return result == default ? default : (T)result;
        }
    }
}