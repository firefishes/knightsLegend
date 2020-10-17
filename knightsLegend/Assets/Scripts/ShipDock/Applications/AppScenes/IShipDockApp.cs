using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public class ShipDockAppComponent : MonoBehaviour, IShipDockApp
    {
        public virtual void CreateTestersHandler()
        {
        }

        public virtual void EnterGameHandler()
        {
        }

        public virtual void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
        {
        }

        public virtual void GetGameServersHandler(IParamNotice<IServer[]> param)
        {
        }

        public virtual void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param)
        {
        }

        public virtual void InitProfileDataHandler(IConfigNotice param)
        {
        }

        public virtual void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param)
        {
        }

        public virtual void InitProfileHandler(IParamNotice<int[]> param)
        {
        }

        public virtual void ServerFinishedHandler()
        {
        }

        public virtual void ApplicationCloseHandler()
        {
        }
    }
}