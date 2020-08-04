﻿using ShipDock.Datas;
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
    /// 门面模式创建的框架单例
    /// </summary>
    public class ShipDockApp : Singletons<ShipDockApp>
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

        public void Start(int ticks)
        {
            Tester.Instance.Log(TesterBaseApp.LOG, IsStarted, "warning: ShipDockApplication has started");

            if (IsStarted)
            {
                return;
            }

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
            mFSMUpdaters = new KeyValueList<IStateMachine, IUpdate>();
            mStateUpdaters = new KeyValueList<IState, IUpdate>();
            Effects = new EffectManager();

            if (ticks > 0)
            {
                TicksUpdater = new TicksUpdater(ticks);//新建客户端心跳帧更新器
            }

            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Broadcast();//框架启动完成

            IsStarted = true;
            mAppStarted?.Invoke();
            mAppStarted = null;
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
            Components = new ShipDockComponentManager
            {
                FrameTimeInScene = (int)(UnityEngine.Time.deltaTime * 1000)
            };

            MethodUpdater updater = ShipDockComponentManagerSetting.isMergeUpdateMode ?
                new MethodUpdater
                {
                    Update = MergeUpdateMode
                } : 
                new MethodUpdater
                {
                    Update = AlternateFrameUpdateMode//框架默认为此模式
                };
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = updater;
            ShipDockConsts.NOTICE_ADD_UPDATE.Broadcast(notice);
            Pooling<UpdaterNotice>.To(notice);

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
        }

        /// <summary>
        /// 交替更新帧模式
        /// </summary>
        private void AlternateFrameUpdateMode(int time)
        {
            if (ShipDockComponentManagerSetting.isUpdateByCallLate)
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
                Components.UpdateComponentUnit(time);
                if (mFrameSign > 0)
                {
                    Components.FreeComponentUnit(time);//奇数帧检测是否有需要释放的实体
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
            if (ShipDockComponentManagerSetting.isUpdateByCallLate)
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

            MethodUpdater updater = ShipDockComponentManagerSetting.isMergeUpdateMode ?
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
            Pooling<UpdaterNotice>.To(notice);
        }

        /// <summary>
        /// 交替更新帧模式（用于主线程的场景更新）
        /// </summary>
        private void AlternateFramUpdateModeInScene(int time)
        {
            if (ShipDockComponentManagerSetting.isUpdateByCallLate)
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
            if (ShipDockComponentManagerSetting.isUpdateByCallLate)
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
            Utils.Reclaim(ref mFSMUpdaters);
            Utils.Reclaim(ref mStateUpdaters);
            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Broadcast();

            Utils.Reclaim(Effects);
            Utils.Reclaim(Notificater);
            Utils.Reclaim(TicksUpdater);
            Utils.Reclaim(Components);
            Utils.Reclaim(Servers);
            Utils.Reclaim(StateMachines);
            Utils.Reclaim(Datas);
            Utils.Reclaim(AssetsPooling);
            Utils.Reclaim(ABs);

            AllPools.ResetAllPooling();

            Notificater = default;
            TicksUpdater = default;
            Components = default;
            Servers = default;
            StateMachines = default;
            Datas = default;
            AssetsPooling = default;
            ABs = default;
            
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
            }
        }

        public bool IsStarted { get; private set; }
        public UIManager UIs { get; private set; }
        public TicksUpdater TicksUpdater { get; private set; }
        public Notifications<int> Notificater { get; private set; }
        public ShipDockComponentManager Components { get; private set; }
        public Servers Servers { get; private set; }
        public DataWarehouse Datas { get; private set; }
        public AssetBundles ABs { get; private set; }
        public AssetsPooling AssetsPooling { get; private set; }
        public StateMachines StateMachines { get; private set; }
        public EffectManager Effects { get; private set; }
    }
}
