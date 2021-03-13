#define _G_LOG
#define _SHIPDOCK_MODULARS

using ShipDock.Commons;
using ShipDock.Datas;
using ShipDock.ECS;
using ShipDock.FSM;
using ShipDock.Loader;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Testers;
using ShipDock.Ticks;
using ShipDock.Tools;
using ShipDock.UI;
using System;
using UnityEngine;

namespace ShipDock.Applications 
{

    /// <summary>
    /// 
    /// ShipDock 框架单例，门面
    /// 
    /// </summary>
    public class ShipDockApp : Singletons<ShipDockApp>, IAppILRuntime, ICustomFramework
    {
        public static void StartUp(int ticks, Action onStartUp = default)
        {
            ICustomFramework app = Instance;
            Framework.Instance.InitCustomFramework(app, ticks, onStartUp);
        }

        /// <summary>
        /// 副线程中延迟一帧后调用
        /// </summary>
        public static void CallLater(Action<int> method)
        {
            Instance.TicksUpdater?.CallLater(method);
        }

        public static void Close()
        {
            Framework.Instance.Clean();
        }

        private int mFrameSign;
        private int mFrameSignInScene;
        private Action mAppStarted;
        private KeyValueList<IStateMachine, IUpdate> mFSMUpdaters;
        private KeyValueList<IState, IUpdate> mStateUpdaters;
        private MethodUpdater mMainThreadReadyChecker;
        private IServer[] mServersWillAdd;

        private IHotFixConfig HotFixConfig { get; set; }

        public bool IsStarted { get; private set; }
        public UIManager UIs { get; private set; }
        public TicksUpdater TicksUpdater { get; private set; }
        public Notifications<int> Notificater { get; private set; }
        public ShipDockComponentContext ECSContext { get; private set; }
        public Servers Servers { get; private set; }
        public AssetBundles ABs { get; private set; }
        public AssetsPooling AssetsPooling { get; private set; }
        public StateMachines StateMachines { get; private set; }
        public Effects Effects { get; private set; }
        public Locals Locals { get; private set; }
        public Tester Tester { get; private set; }
        public PerspectiveInputer PerspectivesInputer { get; private set; }
        public ILRuntimeHotFix ILRuntimeHotFix { get; private set; }
        public DecorativeModulars AppModulars { get; private set; }
        public ConfigHelper Configs { get; private set; }
        public IFrameworkUnit[] FrameworkUnits { get; private set; }
        public Action MergeCallOnMainThread { get; set; }
        public Action SceneUpdaterReady { get; set; }
        public bool IsSceneUpdateReady { get; private set; }
        public IUpdatesComponent UpdatesComponent { get; private set; }

        public DataWarehouse Datas
        {
            get
            {
                return Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
            }
        }

        public void Start(int ticks)
        {
            Application.targetFrameRate = ticks <= 0 ? 10 : ticks;
            if (IsStarted)
            {
                "error".Log("ShipDockApplication has started");
                return;
            }
            Tester = Tester.Instance;
            Tester.Init(new TesterBaseApp());
            "log".AssertLog("framework start", "Welcom..");

            Notificater = NotificatonsInt.Instance.Notificater;//new Notifications<int>();//新建消息中心
            ABs = new AssetBundles();//新建资源包管理器
            Servers = new Servers();//新建服务容器管理器
            DataWarehouse datas = new DataWarehouse();//新建数据管理器
            AssetsPooling = new AssetsPooling();//新建场景资源对象池
            ECSContext = new ShipDockComponentContext//新建 ECS 组件上下文
            {
                FrameTimeInScene = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE)
            };
            StateMachines = new StateMachines//新建有限状态机管理器
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater
            };
            Effects = new Effects();//新建特效管理器
            Locals = new Locals();//新建本地化管理器
            PerspectivesInputer = new PerspectiveInputer();//新建透视物体交互器
            AppModulars = new DecorativeModulars();//新建装饰模块管理器
            Configs = new ConfigHelper();//新建配置管理器

            "debug".Log("UI root ready 2222");
            Framework framework = Framework.Instance;
            FrameworkUnits = new IFrameworkUnit[]
            {
                framework.CreateUnitByBridge(Framework.UNIT_DATA, datas),
                framework.CreateUnitByBridge(Framework.UNIT_AB, ABs),
                framework.CreateUnitByBridge(Framework.UNIT_MODULARS, AppModulars),
                framework.CreateUnitByBridge(Framework.UNIT_ECS, ECSContext),
                framework.CreateUnitByBridge(Framework.UNIT_IOC, Servers),
                framework.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
                framework.CreateUnitByBridge(Framework.UNIT_UI, UIs),
                framework.CreateUnitByBridge(Framework.UNIT_FSM, StateMachines),
            };
            framework.LoadUnit(FrameworkUnits);

            mFSMUpdaters = new KeyValueList<IStateMachine, IUpdate>();
            mStateUpdaters = new KeyValueList<IState, IUpdate>();
            TicksUpdater = new TicksUpdater(Application.targetFrameRate);//新建客户端心跳帧更新器
            "log".AssertLog("framework start", "Ticks Ready");
            "log".AssertLog("framework start", "Managers Ready");
            IsStarted = true;
            mAppStarted?.Invoke();
            mAppStarted = default;

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
            UpdatesComponent?.Init();

            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Broadcast();//框架启动完成
            "log".AssertLog("framework start", "Framework Started");
        }

        private void OnSceneUpdateReady(INoticeBase<int> time)
        {
            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady);

            IsSceneUpdateReady = true;
            SceneUpdaterReady?.Invoke();
        }

        private void OnStateFrameUpdater(IState state, bool isAdd)
        {
            if (isAdd)
            {
                if (!mStateUpdaters.IsContainsKey(state))
                {
                    MethodUpdater updater = new MethodUpdater
                    {
                        Update = state.UpdateState
                    };
                    mStateUpdaters[state] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
            }
            else
            {
                IUpdate updater = mStateUpdaters.GetValue(state, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        private void OnFSMFrameUpdater(IStateMachine fsm, bool isAdd)
        {
            if (isAdd)
            {
                if (!mFSMUpdaters.ContainsKey(fsm))
                {
                    MethodUpdater updater = new MethodUpdater
                    {
                        Update = fsm.UpdateState
                    };
                    mFSMUpdaters[fsm] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
            }
            else
            {
                IUpdate updater = mFSMUpdaters.GetValue(fsm, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        /// <summary>
        /// 启动 IOC 功能
        /// </summary>
        /// <param name="servers">需要添加的服务容器</param>
        /// <param name="mainThreadServersReady">当服务容器初始化完成后在主线程上的回调</param>
        /// <param name="onInitedCallbacks">服务容器初始化完成后在子线程上的一组回调函数</param>
        /// <param name="onFinishedCallbacks">服务容器初始化完成后在子线程上的一组回调函数</param>
        public void StartIOC(IServer[] servers, Action mainThreadServersReady, Action[] onInitedCallbacks = default, Action[] onFinishedCallbacks = default)
        {
            if (mainThreadServersReady != default)
            {
                MergeCallOnMainThread += mainThreadServersReady;
            }
            SetServersCallback(ref onInitedCallbacks, ref onFinishedCallbacks);

            MergeToMainThread(out bool flag);
            if (!flag)
            {
                SceneUpdaterReady += () =>
                {
                    StartIOC(default, default);
                };
            }
            if (mServersWillAdd == default)
            {
                mServersWillAdd = servers;
            }
        }

        private void MergeToMainThread(out bool isSceneUpdateReady)
        {
            isSceneUpdateReady = IsSceneUpdateReady;
            if (IsSceneUpdateReady)
            {
                mMainThreadReadyChecker = new MethodUpdater();
                mMainThreadReadyChecker.Update += OnCheckMainThreadReady;
                UpdaterNotice.AddSceneUpdater(mMainThreadReadyChecker);//将调用并入主线程调用
            }
        }

        private void SetServersCallback(ref Action[] onInitedCallbacks, ref Action[] onFinishedCallbacks)
        {
            int max = onInitedCallbacks != default ? onInitedCallbacks.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.AddOnServerInited(onInitedCallbacks[i]);
                }
            }
            max = onFinishedCallbacks != default ? onFinishedCallbacks.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.AddOnServerFinished(onFinishedCallbacks[i]);
                }
            }
        }

        private void OnCheckMainThreadReady(int time)
        {
            if (Servers.IsServersReady)
            {
                UpdaterNotice.RemoveSceneUpdater(mMainThreadReadyChecker);

                "log".AssertLog("game", "ServerFinished");

                AddServers();
                MergeCallOnMainThread?.Invoke();
                MergeCallOnMainThread = default;
            }
            else
            {
                bool hasPreset = mServersWillAdd != default;
                if (hasPreset)
                {
                    AddServers();
                }
                else
                {
                    Servers.ServersInited();
                }
            }
        }

        private void AddServers()
        {
            int max = mServersWillAdd != default ? mServersWillAdd.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.Add(mServersWillAdd[i]);
                }
            }
            Utils.Reclaim(ref mServersWillAdd);
        }

        /// <summary>
        /// 启动 ECS 功能
        /// </summary>
        public void StartECS()
        {
            if (IsSceneUpdateReady)
            {
                InitECSUpdateModes();
            }
            else
            {
                SceneUpdaterReady += StartECS;
            }
        }

        private void InitECSUpdateModes()
        {
            MethodUpdater updater = ShipDockECSSetting.isMergeUpdateMode ?
                new MethodUpdater
                {
                    Update = MergeUpdateMode
                } :
                new MethodUpdater
                {
                    Update = AlternateFrameUpdateMode//框架默认为此模式
                };
            UpdaterNotice.AddUpdater(updater);

            updater = ShipDockECSSetting.isMergeUpdateMode ?
                new MethodUpdater
                {
                    Update = MergeUpdateModeInScene
                } :
                new MethodUpdater
                {
                    Update = AlternateFramUpdateModeInScene
                };
            UpdaterNotice.AddSceneUpdater(updater);
        }

        /// <summary>
        /// 交替更新帧模式
        /// </summary>
        private void AlternateFrameUpdateMode(int time)
        {
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                ECSContext.UpdateComponentUnit(time, ComponentUnitUpdate);
                if (mFrameSign > 0)
                {
                    ECSContext.FreeComponentUnit(time, ComponentUnitUpdate);//奇数帧检测是否有需要释放的实体
                    ECSContext.RemoveSingedComponents();
                }
            }
            else
            {
                ECSContext.UpdateComponentUnit(time);//框架默认为此模式
                if (mFrameSign > 0)
                {
                    ECSContext.FreeComponentUnit(time);//奇数帧检测是否有需要释放的实体，框架默认为此模式
                    ECSContext.RemoveSingedComponents();
                }
            }
            mFrameSign++;
            mFrameSign = mFrameSign > 1 ? 0 : mFrameSign;
        }

        /// <summary>
        /// 合并更新帧模式
        /// </summary>
        private void MergeUpdateMode(int time)
        {
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                ECSContext.UpdateComponentUnit(time, ComponentUnitUpdate);
                ECSContext.FreeComponentUnit(time, ComponentUnitUpdate);
                ECSContext.RemoveSingedComponents();
            }
            else
            {
                ECSContext.UpdateComponentUnit(time);
                ECSContext.FreeComponentUnit(time);
                ECSContext.RemoveSingedComponents();
            }
        }

        /// <summary>
        /// 更新单个组件
        /// </summary>
        private void ComponentUnitUpdate(Action<int> method)
        {
            TicksUpdater?.CallLater(method);
        }

        /// <summary>
        /// 主线程的场景更新已就绪，用于需要在主线程中更新的组件
        /// </summary>
        private void OnSceneUpdateReady2(INoticeBase<int> obj)
        {
            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady2);

            MethodUpdater updater = ShipDockECSSetting.isMergeUpdateMode ?
                new MethodUpdater
                {
                    Update = MergeUpdateModeInScene
                } :
                new MethodUpdater
                {
                    Update = AlternateFramUpdateModeInScene
                };

            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = updater;
            ShipDockConsts.NOTICE_ADD_SCENE_UPDATE.Broadcast(notice);
            notice.ToPool();
        }

        /// <summary>
        /// 交替更新帧模式（用于主线程的场景更新）
        /// </summary>
        private void AlternateFramUpdateModeInScene(int time)
        {
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                ECSContext.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                if (mFrameSignInScene > 0)
                {
                    ECSContext.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);//奇数帧检测是否有需要释放的实体
                    ECSContext.RemoveSingedComponents();
                }
            }
            else
            {
                ECSContext.UpdateComponentUnitInScene(time);
                if (mFrameSignInScene > 0)
                {
                    ECSContext.FreeComponentUnitInScene(time);//奇数帧检测是否有需要释放的实体
                    ECSContext.RemoveSingedComponents();
                }
            }
            mFrameSignInScene++;
            mFrameSignInScene = mFrameSignInScene > 1 ? 0 : mFrameSignInScene;
        }

        /// <summary>
        /// 合并更新帧模式（用于主线程的场景更新）
        /// </summary>
        private void MergeUpdateModeInScene(int time)
        {
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                ECSContext.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                ECSContext.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);
                ECSContext.RemoveSingedComponents();
            }
            else
            {
                ECSContext.UpdateComponentUnitInScene(time);
                ECSContext.FreeComponentUnitInScene(time);
                ECSContext.RemoveSingedComponents();
            }
        }

        private void ComponentUnitUpdateInScene(Action<int> target)
        {
            UpdaterNotice.SceneCallLater(target);
        }

        public void Clean()
        {
            Framework.Instance.IsStarted = false;

            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Broadcast();

            ILRuntimeHotFix?.Clear();

            Utils.Reclaim(ref mFSMUpdaters);
            Utils.Reclaim(ref mStateUpdaters);

            Locals?.Dispose();
            Effects?.Dispose();
            Notificater?.Dispose();
            TicksUpdater?.Dispose();
            ECSContext?.Dispose();
            Servers?.Dispose();
            StateMachines?.Dispose();
            Datas?.Dispose();
            AssetsPooling?.Dispose();
            ABs?.Dispose();
            PerspectivesInputer?.Dispose();
            AppModulars?.Dispose();

            Tester?.Dispose();

            AllPools.ResetAllPooling();

            Notificater = default;
            TicksUpdater = default;
            ECSContext = default;
            Servers = default;
            StateMachines = default;
            AssetsPooling = default;
            ABs = default;
            Locals = default;
            Effects = default;
            Tester = default;
            PerspectivesInputer = default;
            ILRuntimeHotFix = default;
            AppModulars = default;

            GC.Collect();
        }

        public void AddStart(Action method)
        {
            if (method != default)
            {
                if (IsStarted)
                {
                    method();
                }
                else
                {
                    mAppStarted += method;
                }
            }
        }

        public void RemoveStart(Action method)
        {
            mAppStarted -= method;
        }

        public void InitUIRoot(IUIRoot root)
        {
            if (UIs == default)
            {
                UIs = new UIManager();
                UIs.SetRoot(root);
                "debug".Log("UI root ready");
            }
        }

        public KeyValueList<int, IDataProxy> DataProxyLink(IDataExtracter target, params int[] dataNames)
        {
            int name, max = dataNames == default ? 0 : dataNames.Length;
            IDataProxy proxy;
            KeyValueList<int, IDataProxy> result = new KeyValueList<int, IDataProxy>();
            for (int i = 0; i < max; i++)
            {
                name = dataNames[i];
                proxy = Datas.GetData<IDataProxy>(name);
                proxy.Register(target);
                result[name] = proxy;
            }
            return result;
        }

        public void DataProxyDelink(IDataExtracter target, params int[] dataNames)
        {
            int max = dataNames == default ? 0 : dataNames.Length;
            IDataProxy proxy;
            for (int i = 0; i < max; i++)
            {
                proxy = Datas.GetData<IDataProxy>(dataNames[i]);
                proxy.Unregister(target);
            }
        }

        public void SetHotFixSetting(ILRuntimeHotFix value, IHotFixConfig config)
        {
            if (value != default)
            {
                ILRuntimeHotFix = value;//新建IL热更方案的管理器
                if (ILRuntimeHotFix.GetAppILRuntime() == default)
                {
                    ILRuntimeHotFix.InitFromApp(this);
                }
            }
            HotFixConfig = config;
        }

        public IHotFixConfig GetHotFixConfig()
        {
            return HotFixConfig;
        }

        public void SetStarted(bool value)
        {
            IsStarted = value;
        }

        public void SetUpdatesComp(IUpdatesComponent component)
        {
            UpdatesComponent = component;
        }
    }
}
