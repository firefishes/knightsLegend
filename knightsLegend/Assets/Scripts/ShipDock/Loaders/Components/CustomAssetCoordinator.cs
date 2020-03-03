
using ShipDock.Applications;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    public class CustomAssetCoordinator : MonoBehaviour
    {
        [SerializeField]
        private List<CustomAssetComponent> m_Assets;

        private CustomAssetBundle mCustomAssetBundle;

        private void Awake()
        {
            mCustomAssetBundle = new CustomAssetBundle();

            ComponentBridge bridge = new ComponentBridge(OnAppReady);
            bridge.Start();
        }

        private void OnDestroy()
        {
            Utils.Reclaim(mCustomAssetBundle);
            Utils.Reclaim(ref m_Assets);
        }

        private void OnAppReady()
        {
            mCustomAssetBundle?.FillAssets(ref m_Assets);
            ShipDockApp.Instance.ABs.SetCustomBundles(ref mCustomAssetBundle);
        }
    }

}