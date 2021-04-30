using ShipDock.Datas;
using System;

namespace ShipDock.HotFix
{
    public class HotFixData : IDataExtracter
    {
        public Action<IDataProxy, int> OnData { get; set; }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            OnData?.Invoke(data, DCName);
        }
    }
}
