using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.UI;
using ShipDock.Versioning;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class ShipDockAppComponent : MonoBehaviour, IShipDockApp
    {
        protected ClientResVersion mClientVersions;
        protected IStartingUpdatePopup mStartingUpdatePopup;

        protected ShipDockGame GameComponent { get; private set; }

        protected virtual void OnDestroy()
        {
            GameComponent = default;
            mClientVersions = default;
            mStartingUpdatePopup = default;
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

        public virtual void UpdateRemoteAssetHandler()
        {
            HotFixSubgroup hotFixSubgroup = GameComponent.HotFixSubgroup;
            if (!string.IsNullOrEmpty(hotFixSubgroup.initerNameInResource))
            {
                IAppILRuntime app = ShipDockApp.Instance;
                app.SetHotFixSetting(new ILRuntimeHotFix(app), new AppHotFixConfig());

                GameObject mainBridge = Resources.Load<GameObject>(hotFixSubgroup.initerNameInResource);
                mainBridge = Instantiate(mainBridge);

                ILRuntimeIniter.ApplySingleHotFixMode = false;
                HotFixerComponent hotfixer = mainBridge.GetComponent<HotFixerComponent>();
                ILRuntimeUtils.InvokeMethodILR(hotfixer.ShellBridge, hotFixSubgroup.initerClassName, hotFixSubgroup.initerGameCompSetter, 1, GameComponent);
            }
            else
            {
                mClientVersions = GameComponent.DevelopSetting.remoteAssetVersions;
                if (mClientVersions != default)
                {
                    UIManager uis = ShipDockApp.Instance.UIs;
                    mStartingUpdatePopup = (IStartingUpdatePopup)uis.OpenResourceUI<MonoBehaviour>(GameComponent.DevelopSetting.resUpdatePopupPath);
                    mClientVersions.LoadRemoteVersion(OnLoadComplete, OnVersionInvalid, out _);
                }
                else
                {
                    AfterStartingLoadComplete();
                }
            }
        }

        private bool OnVersionInvalid()
        {
            string version = mClientVersions.RemoteAppVersion;
            string[] splits = version.Split('.');
            int v1 = int.Parse(splits[0]);
            int v2 = int.Parse(splits[1]);

            version = Application.version;
            splits = version.Split('.');

            bool result = v1 > int.Parse(splits[0]) || v2 > int.Parse(splits[1]);
            if (result)
            {
                AfterVersionInvalid();
            }
            else { }
            return result;
        }

        protected virtual void AfterVersionInvalid() { }

        private void OnLoadComplete(bool isComplete, float progress)
        {
            if (isComplete)
            {
                AfterStartingLoadComplete();
            }
            else
            {
                StartingResLoadProgress();
            }
        }

        protected virtual void StartingResLoadProgress()
        {
            mStartingUpdatePopup.Loaded = mClientVersions.UpdatingLoaded;
            mStartingUpdatePopup.LoadingCount = mClientVersions.UpdatingMax;
            mStartingUpdatePopup.LoadingUpdate();
        }

        protected virtual void AfterStartingLoadComplete()
        {
            if (mStartingUpdatePopup != default)
            {
                mStartingUpdatePopup.Close();
                mStartingUpdatePopup = default;
            }
            else { }

            GameComponent.PreloadAsset();

        }

        public void SetShipDockGame(ShipDockGame comp)
        {
            GameComponent = comp;
        }
    }
}