#define G_LOG

using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Testers;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KnightsLegend : MonoBehaviour
    {
        private MethodUpdater updater;

        public void UIRootAwaked(IUIRoot root)
        {
            ShipDockApp.Instance.InitUIRoot(root);
        }

        void Start()
        {
            ShipDockApp.StartUp(200, OnShipDockStart);
        }

        private void OnShipDockStart()
        {
            Tester.Instance.SetDefaultTester(KLTester.Instance);
            Tester.Instance.Init();
            Tester.Instance.Log(KLTester.LOG0, "ShipDock start up..");
            Tester.Instance.Log(KLTester.LOG0, "KL game start");

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);

            ShipDockApp app = ShipDockApp.Instance;
            app.AddStart(OnAppStarted);

            Servers servers = app.Servers;
            servers.OnInit += OnServersInit;
            servers.Add(new KLServer());
            servers.Add(new KLDataServer());
            servers.Add(new KLComponentServer());
            servers.Add(new KLCameraServer());
            servers.AddOnServerFinished(OnFinished);
        }

        private void OnAppStarted()
        {
        }

        private void OnSceneUpdateReady(INoticeBase<int> obj)
        {
            updater = new MethodUpdater();
            updater.Update += CheckServerInited;
            UpdaterNotice.AddSceneUpdater(updater);
        }

        private void CheckServerInited(int obj)
        {
            if (ShipDockApp.Instance.Servers.IsServersReady)
            {
                UpdaterNotice.RemoveSceneUpdater(updater);

                AssetsLoader assetsLoader = new AssetsLoader();
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader
                    .Add(AppPaths.StreamingResDataRoot.Append(AppPaths.resData), KLConsts.A_RES_DATA)
                    //.Add(KLConsts.ASSET_UI_MAIN)
                    //.Add(FWConsts.ASSET_UI_ROLE_CHOOSER)
                    .Add(KLConsts.A_RES_BRIGEDS)
                    .Add(KLConsts.A_MAIN_MALE_ROLE)
                    .Load(out _);
            }
        }

        private void OnPreloadComplete(bool successed, Loader target)
        {
            AssetBundles ABs = ShipDockApp.Instance.ABs;
            GameObject prefab = ABs.Get(KLConsts.A_RES_BRIGEDS, "MainMaleRoleRes");
            GameObject role = Instantiate(prefab);
            //UIManager uis = ShipDockApp.Instance.UIs;
            //uis.Open<RoleChooser>(FWConsts.UI_NAME_ROLE_CHOOSER);

        }

        private void OnServersInit()
        {
            ShipDockApp app = ShipDockApp.Instance;
            app.Servers.AddResolvableConfig(KLConsts.ServerConfigs);
        }

        private void OnFinished()
        {
            #region 测试服务容器
            //FWServer server = FWConsts.SERVER_FW.GetServer<FWServer>();
            //INotice notice = server.Resolve<INotice>("Notice") as INotice;
            //Debug.Log(notice.Name);
            //notice = server.Resolve<INotice>("GameNotice") as INotice;
            //Debug.Log(notice.Name);
            #endregion

        }

        private void OnDestroy()
        {
            ShipDockApp.Close();
        }
    }

}