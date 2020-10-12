#define G_LOG

using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Applications
{

    [Serializable]
    internal class GetDataProxyEvent : UnityEvent<IParamNotice<IDataProxy[]>> { }

    [Serializable]
    internal class GetLocalsConfigItemNotice : UnityEvent<Dictionary<int, string>, IConfigNotice> { }

    [Serializable]
    internal class GetServerConfigsEvent : UnityEvent<IParamNotice<IResolvableConfig[]>> { }

    [Serializable]
    internal class InitProfileEvent : UnityEvent<IParamNotice<int[]>> { }

    [Serializable]
    internal class GetGameServersEvent : UnityEvent<IParamNotice<IServer[]>> { }

    [Serializable]
    internal class InitProfileDataEvent : UnityEvent<IConfigNotice> { }
    
    [RequireComponent(typeof(UpdatesComponent))]
    public class ShipDockGame : MonoBehaviour
    {
        [SerializeField]
        private int m_FrameRate = 40;
        [SerializeField]
        private string m_Locals = "CN";
        [SerializeField]
        private DevelopSubgroup m_DevelopSubgroup;
        [SerializeField]
        private GameApplicationEvents m_GameAppEvents;

        private MethodUpdater mServerInitedChecker;

        public void UIRootAwaked(IUIRoot root)
        {
            ShipDockApp.Instance.InitUIRoot(root);
        }

        private void Awake()
        {
            CreateGame();
        }

        private void OnDestroy()
        {
            ShipDockApp.Close();
        }

        public void CreateGame()
        {
            ShipDockAppComponent component = GetComponent<ShipDockAppComponent>();
            if (component != default)
            {
                m_GameAppEvents.createTestersEvent.AddListener(component.CreateTestersHandler);
                m_GameAppEvents.enterGameEvent.AddListener(component.EnterGameHandler);
                m_GameAppEvents.getDataProxyEvent.AddListener(component.GetDataProxyHandler);
                m_GameAppEvents.getGameServersEvent.AddListener(component.GetGameServersHandler);
                m_GameAppEvents.initProfileDataEvent.AddListener(component.InitProfileDataHandler);
                m_GameAppEvents.getLocalsConfigItemEvent.AddListener(component.GetLocalsConfigItemHandler);
                m_GameAppEvents.getServerConfigsEvent.AddListener(component.GetServerConfigsHandler);
                m_GameAppEvents.initProfileEvent.AddListener(component.InitProfileHandler);
                m_GameAppEvents.serversFinishedEvent.AddListener(component.ServerFinishedHandler);

                "debug".Log("Game Component created..");
            }
            GameObject target = GameObject.Find("UIRoot");
            if (target != default)
            {
                UIRoot ui = target.GetComponent<UIRoot>();
                if (ui != default)
                {
                    "debug".Log("UI Root created..");
                    ui.AddAwakedHandler(UIRootAwaked);
                }
            }
        }

        void Start()
        {
#if RELEASE && !G_LOG
            Debug.unityLogger.logEnabled = false;
#endif
            Application.targetFrameRate = m_FrameRate;
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
        }

        private void OnShipDockStart()
        {
            CreateTesters();

            "debug".AssertLog("game", "Game start.");

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
            ShipDockApp.Instance.AddStart(OnAppStarted);

            InitDataProxy();
            InitServerContainers();
        }

        private void InitServerContainers()
        {
            ShipDockApp app = ShipDockApp.Instance;
            Servers servers = app.Servers;
            servers.OnInit += OnServersInit;
            IServer[] list = GetGameServers();
            int max = list.Length;
            for (int i = 0; i < max; i++)
            {
                servers.Add(list[i]);
            }
            servers.AddOnServerFinished(OnFinished);
        }


        private void InitDataProxy()
        {
            IDataProxy[] list = DataProxyWillInit();
            int max = list != default ? list.Length : 0;
            int[] proxyNames = new int[max];
            for (int i = 0; i < max; i++)
            {
                list[i].AddToWarehouse();
                proxyNames[i] = list[i].DataName;
            }

            InitProfile(ref proxyNames);
        }

        protected virtual void OnAppStarted()
        {
            "debug".AssertLog("game", "Game start callback");
        }

        private void OnServersInit()
        {
            ShipDockApp app = ShipDockApp.Instance;
            app.Servers.AddResolvableConfig(GetServerConfigs());
            "log".AssertLog("game", "ServerInit");
        }

        private void OnSceneUpdateReady(INoticeBase<int> obj)
        {
            mServerInitedChecker = new MethodUpdater();
            mServerInitedChecker.Update += CheckServerInited;
            UpdaterNotice.AddSceneUpdater(mServerInitedChecker);
        }

        private void CheckServerInited(int obj)
        {
            if (ShipDockApp.Instance.Servers.IsServersReady)
            {
                "log".AssertLog("game", "ServerFinished");
                UpdaterNotice.RemoveSceneUpdater(mServerInitedChecker);

                m_DevelopSubgroup.configInitedNoticeName.Add(OnConfigLoaded);

                ServerContainerSubgroup server = m_DevelopSubgroup.loadConfig;
                server.serverName.Delive<IConfigNotice>(server.deliverName, server.alias, OnGetConfigNotice);//IConfigNotice

            }
        }

        private void OnGetConfigNotice(ref IConfigNotice target)
        {
            target.SetNoticeName(m_DevelopSubgroup.configInitedNoticeName);
            target.ParamValue = m_DevelopSubgroup.configNames;
        }

        private void OnConfigLoaded(INoticeBase<int> obj)
        {
            m_DevelopSubgroup.configInitedNoticeName.Remove(OnConfigLoaded);

            IConfigNotice notice = obj as IConfigNotice;

            InitConfigs(ref notice);
            InitProfileData(ref notice);

            notice.IsClearHolderList = true;
            notice.ToPool();

            AssetsLoader assetsLoader = new AssetsLoader();
            assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
            assetsLoader.Add(AppPaths.StreamingResDataRoot.Append(AppPaths.resData), m_DevelopSubgroup.assetNameResData);
            if (m_DevelopSubgroup.assetNamePreload != default)
            {
                int max = m_DevelopSubgroup.assetNamePreload.Length;
                for (int i = 0; i < max; i++)
                {
                    assetsLoader.Add(m_DevelopSubgroup.assetNamePreload[i]);
                }
            }
            assetsLoader.Load(out _);
        }

        protected virtual void InitConfigs(ref IConfigNotice notice)
        {
            Locals locals = ShipDockApp.Instance.Locals;
            locals.SetLocalName(m_Locals);

            Dictionary<int, string> raw = new Dictionary<int, string>();
            m_GameAppEvents.getLocalsConfigItemEvent.Invoke(raw, notice);
            locals.SetLocal(raw);

            "log".AssertLog("game", "LocalsInited");
        }

        private void InitProfileData(ref IConfigNotice notice)
        {
            m_GameAppEvents.initProfileDataEvent?.Invoke(notice);

            "log".AssertLog("game", "ProfileDataInited");
        }

        private void OnPreloadComplete(bool successed, AssetsLoader target)
        {
            target.Dispose();

            EnterGame();
        }

        private void CreateTesters()
        {
            m_GameAppEvents.createTestersEvent?.Invoke();
        }

        private T CommonEventInovker<T>(UnityEvent<IParamNotice<T>> @event, bool applyPooling = false, T param = default)
        {
            T result = default;
            IParamNotice<T> notice = applyPooling ? Pooling<ParamNotice<T>>.From() : new ParamNotice<T>();
            if (param != default)
            {
                notice.ParamValue = param;
            }
            @event?.Invoke(notice);
            result = notice.ParamValue;
            if (applyPooling)
            {
                notice.ToPool();
            }
            else
            {
                notice.Dispose();
            }
            return result;
        }

        private IServer[] GetGameServers()
        {
            IServer[] servers = CommonEventInovker(m_GameAppEvents.getGameServersEvent);
            "debug".Log(servers != default, servers.Length.ToString());
            return servers;
        }

        private void OnFinished()
        {
            m_GameAppEvents.serversFinishedEvent?.Invoke();
        }

        private void InitProfile(ref int[] proxyNames)
        {
            CommonEventInovker(m_GameAppEvents.initProfileEvent, false, proxyNames);
        }

        private IResolvableConfig[] GetServerConfigs()
        {
            "log".AssertLog("game", "ServerInit");
            IResolvableConfig[] serverConfigs = CommonEventInovker(m_GameAppEvents.getServerConfigsEvent);
            return serverConfigs;
        }

        private void EnterGame()
        {
            m_GameAppEvents.enterGameEvent?.Invoke();
        }

        private IDataProxy[] DataProxyWillInit()
        {
            IDataProxy[] result = CommonEventInovker(m_GameAppEvents.getDataProxyEvent);
            return result;
        }
    }

    [Serializable]
    public class DevelopSubgroup
    {
        public int configInitedNoticeName;
        public string assetNameResData = "res_data/res_data";
        public string localsNameKey = "Locals_";
        public string[] configNames;
        public string[] assetNamePreload;
        public ServerContainerSubgroup loadConfig;
    }

    [Serializable]
    public class GameApplicationEvents
    {
        [SerializeField]
        internal UnityEvent createTestersEvent = new UnityEvent();
        [SerializeField]
        internal GetGameServersEvent getGameServersEvent = new GetGameServersEvent();
        [SerializeField]
        internal UnityEvent serversFinishedEvent = new UnityEvent();
        [SerializeField]
        internal InitProfileEvent initProfileEvent = new InitProfileEvent();
        [SerializeField]
        internal InitProfileDataEvent initProfileDataEvent = new InitProfileDataEvent();
        [SerializeField]
        internal GetServerConfigsEvent getServerConfigsEvent = new GetServerConfigsEvent();
        [SerializeField]
        internal GetLocalsConfigItemNotice getLocalsConfigItemEvent = new GetLocalsConfigItemNotice();
        [SerializeField]
        internal UnityEvent enterGameEvent = new UnityEvent();
        [SerializeField]
        internal GetDataProxyEvent getDataProxyEvent = new GetDataProxyEvent();
    }

    [Serializable]
    public class ServerContainerSubgroup
    {
        public string serverName;
        public string deliverName;
        public string alias;
    }

}