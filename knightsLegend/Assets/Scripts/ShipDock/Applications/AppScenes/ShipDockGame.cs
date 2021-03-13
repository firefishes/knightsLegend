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
    [RequireComponent(typeof(UpdatesComponent))]
    public class ShipDockGame : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("运行帧率")]
        private int m_FrameRate = 40;
        [SerializeField]
        [Tooltip("多语言本地化标识")]
        private string m_Locals = "CN";
        [SerializeField]
        [Tooltip("开发设置子组")]
        private DevelopSubgroup m_DevelopSubgroup;
        [SerializeField]
        [Tooltip("ILRuntime热更子组")]
        private HotFixSubgroup m_HotFixSubgroup;
        [SerializeField]
        [Tooltip("游戏应用启动系列事件")]
        private GameApplicationEvents m_GameAppEvents;

        public DevelopSubgroup DevelopSetting
        {
            get
            {
                return m_DevelopSubgroup;
            }
        }

        public HotFixSubgroup HotFixSubgroup
        {
            get
            {
                return m_HotFixSubgroup;
            }
        }

        public void UIRootAwaked(IUIRoot root)
        {
#if RELEASE
            Debug.unityLogger.logEnabled = false;
#endif
            ShipDockApp.Instance.InitUIRoot(root);
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
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
                component.SetShipDockGame(this);
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
                m_GameAppEvents.updateRemoteAssetEvent.AddListener(component.UpdateRemoteAssetHandler);

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

        protected virtual void OnApplicationQuit()
        {
            BackgroundOperation(true);
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                BackgroundOperation(false);
            }
        }

        protected virtual void BackgroundOperation(bool isCleanVersionCache)
        {
#if !UNITY_EDITOR
            if (m_DevelopSubgroup.ApplyRemoteAssets)
            {
                m_DevelopSubgroup.remoteAssetVersions.CacheResVersion(isCleanVersionCache);
            }
#endif
        }

        private void OnShipDockStart()
        {
            CreateTesters();
            InitDataProxy();
            InitServerContainers();
        }

        private void InitServerContainers()
        {
            ShipDockApp app = ShipDockApp.Instance;
            bool flag = m_DevelopSubgroup.startUpIOC;
            IServer[] servers = flag ? GetGameServers() : default;
            Action[] onInited = flag ? new Action[] { AddResolvableConfigs } : default;
            Action[] onFinished = flag ? new Action[] { OnServersFinished } : default;
            app.StartIOC(servers, MainThreadServerReady, onInited, onFinished);
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

        private void AddResolvableConfigs()
        {
            ShipDockApp app = ShipDockApp.Instance;

            IResolvableConfig[] resolvableConfs = GetServerConfigs();
            app.Servers.AddResolvableConfig(resolvableConfs);

            "log".AssertLog("game", "ServerInit");
        }

        private void MainThreadServerReady()
        {
            "log".AssertLog("game", "ServerFinished");

            int configInitedNoticeName = m_DevelopSubgroup.configInitedNoticeName;
            if (m_DevelopSubgroup.hasLocalsConfig && (configInitedNoticeName != int.MaxValue))
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

            if (m_DevelopSubgroup.ApplyRemoteAssets)
            {
                m_GameAppEvents.updateRemoteAssetEvent.Invoke();
            }
            else
            {
                PreloadAsset();
            }
        }

        public void PreloadAsset()
        {
            AssetsLoader assetsLoader = new AssetsLoader();
            int max = m_DevelopSubgroup.assetNamePreload.Length;
            if (max > 0)
            {
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader.AddManifest(m_DevelopSubgroup.assetNameResData, m_DevelopSubgroup.applyManifestAutoPath);
                for (int i = 0; i < max; i++)
                {
                    assetsLoader.Add(m_DevelopSubgroup.assetNamePreload[i], true, m_DevelopSubgroup.applyManifestAutoPath);
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

            "debug".AssertLog("game", "Game start.");
        }

        private T CommonEventInovker<T>(UnityEvent<IParamNotice<T>> commonEvent, bool applyPooling = false, T param = default)
        {
            IParamNotice<T> notice = applyPooling ? Pooling<ParamNotice<T>>.From() : new ParamNotice<T>();
            if (param != null)
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

        private void OnServersFinished()
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
}