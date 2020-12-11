#define _G_LOG

using ShipDock.Datas;
using ShipDock.ECS;
using ShipDock.FSM;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Testers;
using ShipDock.Tools;
using ShipDock.UI;
using System;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// ShipDock 框架单例，门面
    /// 
    /// </summary>
    public class ShipDockApp : Singletons<ShipDockApp>, IAppILRuntime
    {
        public static void StartUp(int ticks, Action onStartUp = default)
        {
            if (onStartUp != default)
            {
                Instance.AddStart(onStartUp);
            }
            Instance.Start(ticks);
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
            Instance.Clean();
        }

        private int mFrameSign;
        private int mFrameSignInScene;
        private Action mAppStarted;
        private KeyValueList<IStateMachine, IUpdate> mFSMUpdaters;
        private KeyValueList<IState, IUpdate> mStateUpdaters;

        private IHotFixConfig HotFixConfig { get; set; }

        public bool IsStarted { get; private set; }
        public UIManager UIs { get; private set; }
        public TicksUpdater TicksUpdater { get; private set; }
        public Notifications<int> Notificater { get; private set; }
        public ShipDockComponentContext Components { get; private set; }
        public Servers Servers { get; private set; }
        public DataWarehouse Datas { get; private set; }
        public AssetBundles ABs { get; private set; }
        public AssetsPooling AssetsPooling { get; private set; }
        public StateMachines StateMachines { get; private set; }
        public Effects Effects { get; private set; }
        public Locals Locals { get; private set; }
        public Tester Tester { get; private set; }
        public PerspectiveInputer PerspectivesInputer { get; private set; }
        public ILRuntimeHotFix ILRuntimeHotFix { get; private set; }

        public void Start(int ticks)
        {
            if (IsStarted)
            {
                "error".Log("ShipDockApplication has started");
                return;
            }
            Tester = Tester.Instance;
            Tester.Init(new TesterBaseApp());
            "log".AssertLog("framework start", "Welcom..");

            Notificater = new Notifications<int>();//新建消息中心
            ABs = new AssetBundles();//新建资源包管理器
            Servers = new Servers(OnServersInit);//新建服务容器管理器
            Datas = new DataWarehouse();//新建数据管理器
            AssetsPooling = new AssetsPooling();//新建场景资源对象池
            StateMachines = new StateMachines//新建有限状态机管理器
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater
            };
            Effects = new Effects();//新建特效管理器
            Locals = new Locals();//新建本地化管理器
            PerspectivesInputer = new PerspectiveInputer();//新建透视物体交互器

            mFSMUpdaters = new KeyValueList<IStateMachine, IUpdate>();
            mStateUpdaters = new KeyValueList<IState, IUpdate>();

            "log".AssertLog("framework start", "Managers Ready");

            if (ticks > 0)
            {
                TicksUpdater = new TicksUpdater(ticks);//新建客户端心跳帧更新器
                "log".AssertLog("framework start", "Ticks Ready");
            }

            IsStarted = true;
            mAppStarted?.Invoke();
            mAppStarted = null;

            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Broadcast();//框架启动完成
            "log".AssertLog("framework start", "Framework Started");
        }

        private void OnStateFrameUpdater(IState state, bool isAdd)
        {
            if (isAdd)
            {
                if(!mStateUpdaters.IsContainsKey(state))
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
                IUpdate updater = mStateUpdaters[state];
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        private void OnFSMFrameUpdater(IStateMachine fsm, bool isAdd)
        {
            if (mFSMUpdaters == default)
            {
                return;
            }
            if(isAdd)
            {
                MethodUpdater updater = new MethodUpdater
                {
                    Update = fsm.UpdateState
                };
                mFSMUpdaters[fsm] = updater;
                UpdaterNotice.AddSceneUpdater(updater);
            }
            else
            {
                IUpdate updater = mFSMUpdaters[fsm];
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        /// <summary>
        /// 所有服务容器初始化完成
        /// </summary>
        private void OnServersInit()
        {
            Components = new ShipDockComponentContext
            {
                FrameTimeInScene = (int)(UnityEngine.Time.deltaTime * 1000)
            };

            MethodUpdater updater = ShipDockECSSetting.isMergeUpdateMode ?
                new MethodUpdater
                {
                    Update = MergeUpdateMode
                }: 
                new MethodUpdater
                {
                    Update = AlternateFrameUpdateMode//框架默认为此模式
                };
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = updater;
            ShipDockConsts.NOTICE_ADD_UPDATE.Broadcast(notice);
            notice.ToPool();

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
        }

        /// <summary>
        /// 交替更新帧模式
        /// </summary>
        private void AlternateFrameUpdateMode(int time)
        {
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                Components.UpdateComponentUnit(time, ComponentUnitUpdate);
                if (mFrameSign > 0)
                {
                    Components.FreeComponentUnit(time, ComponentUnitUpdate);//奇数帧检测是否有需要释放的实体
                    Components.RemoveSingedComponents();
                }
            }
            else
            {
                Components.UpdateComponentUnit(time);//框架默认为此模式
                if (mFrameSign > 0)
                {
                    Components.FreeComponentUnit(time);//奇数帧检测是否有需要释放的实体，框架默认为此模式
                    Components.RemoveSingedComponents();
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
                Components.UpdateComponentUnit(time, ComponentUnitUpdate);
                Components.FreeComponentUnit(time, ComponentUnitUpdate);
                Components.RemoveSingedComponents();
            }
            else
            {
                Components.UpdateComponentUnit(time);
                Components.FreeComponentUnit(time);
                Components.RemoveSingedComponents();
            }
        }

        /// <summary>
        /// 更新单个组件
        /// </summary>
        private void ComponentUnitUpdate(Action<int> method)
        {
            TicksUpdater.CallLater(method);
        }

        /// <summary>
        /// 主线程的场景更新已就绪，用于需要在主线程中更新的组件
        /// </summary>
        private void OnSceneUpdateReady(INoticeBase<int> obj)
        {
            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady);

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
                Components.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                if (mFrameSignInScene > 0)
                {
                    Components.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);//奇数帧检测是否有需要释放的实体
                    Components.RemoveSingedComponents();
                }
            }
            else
            {
                Components.UpdateComponentUnitInScene(time);
                if (mFrameSignInScene > 0)
                {
                    Components.FreeComponentUnitInScene(time);//奇数帧检测是否有需要释放的实体
                    Components.RemoveSingedComponents();
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
                Components.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                Components.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);
                Components.RemoveSingedComponents();
            }
            else
            {
                Components.UpdateComponentUnitInScene(time);
                Components.FreeComponentUnitInScene(time);
                Components.RemoveSingedComponents();
            }
        }

        private void ComponentUnitUpdateInScene(Action<int> target)
        {
            UpdaterNotice.SceneCallLater(target);
        }

        public void Clean()
        {
            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Broadcast();

            ILRuntimeHotFix?.Clear();

            Utils.Reclaim(ref mFSMUpdaters);
            Utils.Reclaim(ref mStateUpdaters);

            Utils.Reclaim(Locals);
            Utils.Reclaim(Effects);
            Utils.Reclaim(Notificater);
            Utils.Reclaim(TicksUpdater);
            Utils.Reclaim(Components);
            Utils.Reclaim(Servers);
            Utils.Reclaim(StateMachines);
            Utils.Reclaim(Datas);
            Utils.Reclaim(AssetsPooling);
            Utils.Reclaim(ABs);
            Utils.Reclaim(PerspectivesInputer);

            Tester?.Dispose();

            AllPools.ResetAllPooling();

            Notificater = default;
            TicksUpdater = default;
            Components = default;
            Servers = default;
            StateMachines = default;
            Datas = default;
            AssetsPooling = default;
            ABs = default;
            Locals = default;
            Effects = default;
            Tester = default;
            PerspectivesInputer = default;
            ILRuntimeHotFix = default;

            GC.Collect();
        }

        public void AddStart(Action method)
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
            IDataProxy proxy;
            int name;
            int max = dataNames != default ? dataNames.Length : 0;
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
            IDataProxy proxy;
            int max = dataNames == default ? 0 : dataNames.Length;
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
    }
}
