using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public interface IShipDockApp
    {
        void CreateTestersHandler();
        void GetGameServersHandler(IParamNotice<IServer[]> param);
        void ServerFinishedHandler();
        void InitProfileHandler(IParamNotice<int[]> param);
        void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param);
        void InitProfileDataHandler(IConfigNotice param);
        void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param);
        void EnterGameHandler();
        void GetDataProxyHandler(IParamNotice<IDataProxy[]> param);
        void ApplicationCloseHandler();
        void SetShipDockGame(ShipDockGame comp);
    }
}