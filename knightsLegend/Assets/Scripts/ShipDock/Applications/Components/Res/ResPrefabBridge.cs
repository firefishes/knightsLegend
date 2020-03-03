using UnityEngine;

namespace ShipDock.Applications
{
    public class ResPrefabBridge : ResBridge, IResPrefabBridge
    {
        protected override void Awake()
        {
            base.Awake();

            if(m_Asset != default)
            {
                GameObject source = Assets.Get(m_Asset.GetABName(), m_Asset.GetAssetName());
                Prefab = Instantiate(source);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Prefab = default;
        }

        public GameObject Prefab { get; private set; }
    }

}