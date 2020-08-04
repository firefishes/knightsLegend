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
        [SerializeField]
        protected int m_PoolID;

        protected virtual void Awake()
        {
            Assets = ShipDockApp.Instance.ABs;
        }

        protected virtual void OnDestroy()
        {
            Assets = default;
            m_PoolID = int.MaxValue;
        }

        public void SetPoolID(int id)
        {
            if (m_PoolID == int.MaxValue)
            {
                m_PoolID = id;
            }
        }

        protected IAssetBundles Assets { get; set; }

        public int PoolID
        {
            get
            {
                return m_PoolID;
            }
        }
    }

}
