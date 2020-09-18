using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class DataProxy : IDataProxy, IDispose
    {
        private List<IDataExtracter> mDataHandlers;
        private Action<IDataProxy, int> mOnDataProxyNotify;

        public DataProxy(int dataName)
        {
            DataName = dataName;
            mDataHandlers = new List<IDataExtracter>();
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mDataHandlers);
            mOnDataProxyNotify = default;
        }

        public void DataNotify(params int[] keys)
        {
            int max = keys.Length;
            if (max > 0)
            {
                int keyName;
                for (int i = 0; i < max; i++)
                {
                    keyName = keys[i];
                    mOnDataProxyNotify?.Invoke(this, keyName);
                }
            }
            else
            {
                mOnDataProxyNotify?.Invoke(this, int.MaxValue);
            }
        }

        public void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Add(dataHandler);
            mOnDataProxyNotify += dataHandler.OnDataProxyNotify;
        }

        public void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers == default || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Remove(dataHandler);
            mOnDataProxyNotify -= dataHandler.OnDataProxyNotify;
        }

        public int DataName { get; private set; }
    }
}
