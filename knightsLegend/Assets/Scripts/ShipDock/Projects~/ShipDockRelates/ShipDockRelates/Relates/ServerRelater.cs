using ShipDock.Datas;
using ShipDock.ECS;
using ShipDock.Interfaces;
using ShipDock.Server;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class ServerRelater : IDispose
    {
        private KeyValueList<int, IDataProxy> mDataCached;
        private KeyValueList<int, IShipDockComponent> mCompCached;
        private KeyValueList<string, IServer> mServerCached;
        
        public void Dispose()
        {
            Utils.Reclaim(ref mDataCached);
            Utils.Reclaim(ref mCompCached);
            Utils.Reclaim(ref mServerCached);
        }

        public void CommitRelate()
        {
            int name;
            int max = (ComponentNames != default) ? ComponentNames.Length : 0;
            if(max > 0)
            {
                if (mCompCached == default)
                {
                    mCompCached = new KeyValueList<int, IShipDockComponent>();
                }
                var components = ShipDockECS.Instance.Context;
                for (int i = 0; i < max; i++)
                {
                    name = ComponentNames[i];
                    mCompCached[name] = components.RefComponentByName(name);
                }
            }
            max = (DataNames != default) ? DataNames.Length : 0;
            if (max > 0)
            {
                if (mDataCached == default)
                {
                    mDataCached = new KeyValueList<int, IDataProxy>();
                }
                DataWarehouse datas = Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
                for (int i = 0; i < max; i++)
                {
                    name = DataNames[i];
                    mDataCached[name] = datas.GetData<IDataProxy>(name);
                }
            }
            max = (ServerNames != default) ? ServerNames.Length : 0;
            if (max > 0)
            {
                if (mServerCached == default)
                {
                    mServerCached = new KeyValueList<string, IServer>();
                }
                string serverName;
                Servers servers = Framework.Instance.GetUnit<Servers>(Framework.UNIT_IOC);
                for (int i = 0; i < max; i++)
                {
                    serverName = ServerNames[i];
                    mServerCached[serverName] = servers.GetServer<IServer>(serverName);
                }
            }
        }

        public T ComponentRef<T>(int componentName) where T : IShipDockComponent
        {
            return mCompCached != default ? (T)mCompCached[componentName] : default;
        }

        public T DataRef<T>(int dataName) where T : IDataProxy
        {
            return mDataCached != default ? (T)mDataCached[dataName] : default;
        }

        public T ServerRef<T>(string serverName) where T : IServer
        {
            return mServerCached != default ? (T)mServerCached[serverName] : default;
        }

        public int[] DataNames { get; set; }
        public int[] ComponentNames { get; set; }
        public string[] ServerNames { get; set; }
    }
}