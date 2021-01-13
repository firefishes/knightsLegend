using ShipDock.Config;
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
        void SetShipDockGame(ShipDockGame comp);
    }

    public abstract class ShipDockAppComponent : MonoBehaviour, IShipDockApp
    {
        protected ShipDockGame GameComponent { get; private set; }

        protected virtual void OnDestroy()
        {
            GameComponent = default;
        }

        public virtual void CreateTestersHandler() { }

        public virtual void EnterGameHandler() { }

        public virtual void GetDataProxyHandler(IParamNotice<IDataProxy[]> param) { }

        public virtual void GetGameServersHandler(IParamNotice<IServer[]> param) { }

        public virtual void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param) { }

        protected void SetDataFromLocalsConfig<T>(ref string locals, ref string localsConfigName, ref IConfigNotice param, ref Dictionary<int, string> raw) where T : IConfig, new()
        {
            Dictionary<int, T> configs = param.GetConfigRaw<T>(localsConfigName);

            int id;
            T item;
            KeyValuePair<int, T> pair;
            Dictionary<int, T>.Enumerator localsEnumer = configs.GetEnumerator();
            int max = configs.Count;
            for (int i = 0; i < max; i++)
            {
                localsEnumer.MoveNext();
                pair = localsEnumer.Current;
                item = pair.Value;
                id = item.GetID();
                raw[id] = GetLocalsDescription(ref locals, ref item);
            }
        }

        protected abstract string GetLocalsDescription<T>(ref string locals, ref T item) where T : IConfig, new();

        public virtual void InitProfileDataHandler(IConfigNotice param) { }

        public virtual void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param) { }

        public virtual void InitProfileHandler(IParamNotice<int[]> param) { }

        public virtual void ServerFinishedHandler() { }

        public virtual void ApplicationCloseHandler() { }

        public virtual void UpdateRemoteAssetHandler() { }

        public void SetShipDockGame(ShipDockGame comp)
        {
            GameComponent = comp;
        }
    }
}