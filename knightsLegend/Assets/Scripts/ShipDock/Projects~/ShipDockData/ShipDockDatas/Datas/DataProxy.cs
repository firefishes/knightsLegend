using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class DataProxy : IDataProxy
    {
        private List<IDataExtracter> mDataHandlers;
        private Action<IDataProxy, int> mOnDataProxyNotify;

        public DataProxy()
        {
            mDataHandlers = new List<IDataExtracter>();
        }

        public DataProxy(int dataName) : this()
        {
            DataName = dataName;
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

        public virtual void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Add(dataHandler);
            mOnDataProxyNotify += dataHandler.OnDataProxyNotify;
        }

        public virtual void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers == default || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Remove(dataHandler);
            mOnDataProxyNotify -= dataHandler.OnDataProxyNotify;
        }

        public void AddDataProxyNotify(Action<IDataProxy, int> notifyHandler)
        {
            mOnDataProxyNotify += notifyHandler;
        }

        public void RemoveDataProxyNotify(Action<IDataProxy, int> notifyHandler)
        {
            mOnDataProxyNotify -= notifyHandler;
        }

        public virtual int DataName { get; private set; }
    }
}
