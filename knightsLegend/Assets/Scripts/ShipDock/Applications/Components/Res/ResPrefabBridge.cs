using UnityEngine;

namespace ShipDock.Applications
{
    public class ResPrefabBridge : ResBridge, IResPrefabBridge
    {
        protected override void Awake()
        {
            base.Awake();

            if(m_IsCreateInAwake)
            {
                CreateAsset();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Prefab = default;
        }

        public void CreateAsset()
        {
            if ((m_Asset != default) && (Prefab == default))
            {
                GameObject source = Assets.Get(m_Asset.GetABName(), m_Asset.GetAssetName());
                Prefab = source;
            }
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_Asset.SetSubgroup(ab, asset);
        }

        public GameObject Prefab { get; private set; }
    }

}