using ShipDock.Loader;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class ResBridge : MonoBehaviour, IResBridge
    {
        [SerializeField]
        protected bool m_IsCreateInAwake = true;
        [SerializeField]
        protected AssetSubgroup m_Asset;

        protected virtual void Awake()
        {
            Assets = ShipDockApp.Instance.ABs;
        }

        protected virtual void OnDestroy()
        {
            Assets = default;
        }

        protected IAssetBundles Assets { get; set; }
    }

}
