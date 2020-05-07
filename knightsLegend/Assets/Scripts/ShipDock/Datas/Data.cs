using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class Data : IData, IDispose
    {
        private List<IDataExtracter> mDataHandlers;
        private Action<IData, int> mOnDataChanged;

        public Data(int dataName)
        {
            DataName = dataName;
            mDataHandlers = new List<IDataExtracter>();
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mDataHandlers);
            mOnDataChanged = default;
        }

        public void DataChanged(params int[] keys)
        {
            int keyName;
            int max = keys.Length;
            for (int i = 0; i < max; i++)
            {
                keyName = keys[i];
                mOnDataChanged?.Invoke(this, keyName);
            }
        }

        public void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Add(dataHandler);
            mOnDataChanged += dataHandler.OnDataChanged;
        }

        public void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Remove(dataHandler);
            mOnDataChanged -= dataHandler.OnDataChanged;
        }

        public int DataName { get; private set; }
    }

    public class DataStorager : Data
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

    public class DataUnit
    {

    }

    public interface IDataUnit
    {

    }
}
