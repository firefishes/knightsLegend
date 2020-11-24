#define _G_LOG

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

    [Serializable]
    internal class ShipDockCloseEvent : UnityEvent { }
    
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
            m_GameAppEvents.frameworkCloseEvent.Invoke();

            m_GameAppEvents.frameworkCloseEvent.RemoveAllListeners();
            m_GameAppEvents.createTestersEvent.RemoveAllListeners();
            m_GameAppEvents.enterGameEvent.RemoveAllListeners();
            m_GameAppEvents.getDataProxyEvent.RemoveAllListeners();
            m_GameAppEvents.getGameServersEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileDataEvent.RemoveAllListeners();
            m_GameAppEvents.getLocalsConfigItemEvent.RemoveAllListeners();
            m_GameAppEvents.getServerConfigsEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileEvent.RemoveAllListeners();
            m_GameAppEvents.serversFinishedEvent.RemoveAllListeners();

            ShipDockApp.Close();

            "debug".Log("ShipDock close.");
        }

        private void CreateGame()
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
                m_GameAppEvents.frameworkCloseEvent.AddListener(component.ApplicationCloseHandler);

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

        private void Start()
        {
#if RELEASE
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
            int max = list != default ? list.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    servers.Add(list[i]);
                }
            }
            else
            {
                servers.Add(new MainServer("ServerShipDock"));
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

            IResolvableConfig[] resolvableConfs = GetServerConfigs();
            resolvableConfs = resolvableConfs != default ? resolvableConfs : MainServer.ServerConfigs.ToArray();

            app.Servers.AddResolvableConfig(resolvableConfs);

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

                int configInitedNoticeName = m_DevelopSubgroup.configInitedNoticeName;
                if (configInitedNoticeName != int.MaxValue)
                {
                    configInitedNoticeName.Add(OnConfigLoaded);//订阅一个配置初始化完成的消息
                    ServerContainerSubgroup server = m_DevelopSubgroup.loadConfig;
                    server.Delive<IConfigNotice>(OnGetConfigNotice);//加载配置
                	//调用配置服务容器方法 Sample: "ServerConfig".Delive<IConfigNotice>("LoadConfig", "ConfigNotice", OnGetConfigNotice);
                }
                else
                {
                    OnConfigLoaded(default);
                }
            }
        }

        /// <summary>
        /// 获取配置的外派方法的装饰器方法
        /// </summary>
        /// <param name="target"></param>
        private void OnGetConfigNotice(ref IConfigNotice target)
        {
            target.SetNoticeName(m_DevelopSubgroup.configInitedNoticeName);
            target.ParamValue = m_DevelopSubgroup.configNames;
        }

        /// <summary>
        /// 配置加载完成消息处理函数
        /// </summary>
        /// <param name="param"></param>
        private void OnConfigLoaded(INoticeBase<int> param)
        {
            m_DevelopSubgroup.configInitedNoticeName.Remove(OnConfigLoaded);

            IConfigNotice notice = param as IConfigNotice;

            //从消息参数中获取配置数据的 Sample:
            //Dictionary<int, ConfigClass> mapper = notice.GetConfigRaw<ConfigClass>("ConfigName");
            //var a = mapper[1].a;
            //var b = mapper[12].b;

            InitConfigs(ref notice);
            InitProfileData(ref notice);

            if (notice != default)
            {
                notice.IsClearHolderList = true;
                notice.ToPool();
            }

            AssetsLoader assetsLoader = new AssetsLoader();
            int max = m_DevelopSubgroup.assetNamePreload.Length;
            if (max > 0)
            {
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader.Add(AppPaths.StreamingResDataRoot.Append(AppPaths.resData), m_DevelopSubgroup.assetNameResData);
                for (int i = 0; i < max; i++)
                {
                    assetsLoader.Add(m_DevelopSubgroup.assetNamePreload[i]);
                }
                assetsLoader.Load(out _);
            }
            else
            {
                OnPreloadComplete(true, assetsLoader);
            }
        }

        protected virtual void InitConfigs(ref IConfigNotice notice)
        {
            Locals locals = ShipDockApp.Instance.Locals;
            locals.SetLocalName(m_Locals);

            if (notice != default)
            {
                Dictionary<int, string> raw = new Dictionary<int, string>();
                m_GameAppEvents.getLocalsConfigItemEvent.Invoke(raw, notice);
                locals.SetLocal(raw);
            }

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

        private T CommonEventInovker<T>(UnityEvent<IParamNotice<T>> commonEvent, bool applyPooling = false, T param = default)
        {
            IParamNotice<T> notice = applyPooling ? Pooling<ParamNotice<T>>.From() : new ParamNotice<T>();
            if (!param.Equals(default))
            {
                notice.ParamValue = param;
            }
            commonEvent?.Invoke(notice);

            T result = notice.ParamValue;
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
            "log".Log(servers != default, servers != default ? "Servers count is ".Append(servers.Length.ToString()) : "Servers is empty..");
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
        public int configInitedNoticeName = int.MaxValue;
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
        [SerializeField]
        internal ShipDockCloseEvent frameworkCloseEvent = new ShipDockCloseEvent();
    }

    [Serializable]
    public class ServerContainerSubgroup
    {
        /// <summary>服务容器名</summary>
        public string serverName;
        /// <summary>外派方法名</summary>
        public string deliverName;
        /// <summary>参数对象别名</summary>
        public string alias;

        public void Delive<T>(ResolveDelegate<T> customResolver = default)
        {
            serverName.Delive<T>(deliverName, alias, customResolver);//调用容器方法
        }
    }

}