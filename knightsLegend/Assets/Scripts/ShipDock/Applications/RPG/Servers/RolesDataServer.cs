using ShipDock.Datas;
using ShipDock.Notices;

namespace ShipDock.Applications
{
    public abstract class RolesDataServer : Server.Server, IDataServer
    {
        protected ServerRelater mRelater;

        public RolesDataServer()
        {
            ServerName = DataServerName;
            mRelater = new ServerRelater
            {
                DataNames = RelatedDataNames,
                ComponentNames = RelatedComponentNames
            };

        }

        public override void InitServer()
        {
            base.InitServer();

            ShipDockApp app = ShipDockApp.Instance;
            var datas = app.Datas;
            IData[] dataList = DataList;
            int max = dataList.Length;
            for (int i = 0; i < max; i++)
            {
                datas.AddData(dataList[i]);
            }
        }

        public override void ServerReady()
        {
            base.ServerReady();

            mRelater.CommitRelate();
        }

        public abstract string DataServerName { get; }
        public abstract int[] RelatedDataNames { get; }
        public abstract int[] RelatedComponentNames { get; }
        public abstract IData[] DataList { get; }
    }


}