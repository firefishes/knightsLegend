using ShipDock.Tools;
using System;
using DataLoader = ShipDock.Loader.Loader;

namespace ShipDock.Network
{
    public class NetDistributer
    {
        public Action<bool, byte[]> Prerecived { get; set; }
        public Action<int, byte[]> Successed { get; set; }
        public Action<int, byte[]> Failed { get; set; }
        public Func<string, byte[]> FalseDataCreater { get; set; }

        private KeyValueList<string, int> mDataResultNotices;

        public void Init()
        {
            mDataResultNotices = new KeyValueList<string, int>();
        }

        public void AddRequestInfo(int noticeName, string api)
        {
            mDataResultNotices[api] = noticeName;
        }

        public void Send(string url, bool isFalseData = false)
        {
            if (isFalseData && (FalseDataCreater != default))
            {
                byte[] data = FalseDataCreater?.Invoke(url);
                RecivedData(true, ref url, ref data);
            }
            else
            {
                DataLoader loader = new DataLoader(DataLoader.LOADER_DEFAULT);
                loader.CompleteEvent.AddListener(OnNetCompleted);
                loader.Load(url);
            }
        }

        private void OnNetCompleted(bool successed, DataLoader loader)
        {
            string url = loader.Url;
            byte[] data = loader.ResultData;

            RecivedData(successed, ref url, ref data);

            loader.Dispose();
        }

        private void RecivedData(bool successed, ref string api, ref byte[] data)
        {
            Prerecived?.Invoke(successed, data);

            int noticeName = mDataResultNotices[api];
            if (successed)
            {
                Successed?.Invoke(noticeName, data);
            }
            else
            {
                Failed?.Invoke(noticeName, data);
            }
        }
    }
}
