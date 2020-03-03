using ShipDock.Datas;
using ShipDock.ECS;
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

        public static void CallLater(Action<int> method)
        {
            Instance.TicksUpdater?.CallLater(method);
        }

        public static void Close()
        {
            Instance.Clean();
        }

        private int mFrameSign;
        private Action mAppStarted;

        public void Start(int ticks)
        {
            Tester.Instance.Log(TesterBaseApp.LOG, IsStarted, "warning: ShipDockApplication has started");

            if (IsStarted)
            {
                return;
            }

            Notificater = new Notifications<int>();
            ABs = new AssetBundles();
            Servers = new Servers();
            Servers.OnInit += OnCreateComponentManager;
            Datas = new DataWarehouse();
            AssetsPooling = new AssetsPooling();

            if (ticks > 0)
            {
                TicksUpdater = new TicksUpdater(ticks);
            }

            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Dispatch();

            IsStarted = true;
            mAppStarted?.Invoke();
            mAppStarted = null;
        }

        private void OnCreateComponentManager()
        {
            Components = new ShipDockComponentManager();

            MethodUpdater updater = new MethodUpdater
            {
                Update = ComponentUpdateByTicks
            };
            UpdaterNotice notice = new UpdaterNotice();
            notice.ParamValue = updater;
            ShipDockConsts.NOTICE_ADD_UPDATE.Dispatch(notice);
            notice.Dispose();
        }

        private void ComponentUpdateByTicks(int time)
        {
            Components.UpdateComponentUnit(ComponentUnitUpdate);
            if (mFrameSign > 0)
            {
                Components.FreeComponentUnit(ComponentUnitUpdate);
            }
            mFrameSign++;
            if (mFrameSign > 1)
            {
                mFrameSign = 0;
            }
        }

        private void ComponentUnitUpdate(Action<int> method)
        {
            TicksUpdater.CallLater(method);
        }

        public void Clean()
        {
            IsStarted = false;

            Utils.Reclaim(Notificater);
            TicksUpdater?.Dispose();

            Notificater = null;
            TicksUpdater = null;
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
    }
}
