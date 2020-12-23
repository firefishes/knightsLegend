#define _G_LOG

using UnityEngine;

namespace ShipDock.Applications
{
    public class ResPrefabBridge : ResBridge, IResPrefabBridge
    {
        public bool IsCreateInAwake
        {
            get
            {
                return m_IsCreateInAwake;
            }
            set
            {
                m_IsCreateInAwake = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if(m_IsCreateInAwake)
            {
                CreateRaw();
                Instantiate(Prefab);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Prefab = default;
        }

        public void CreateRaw()
        {
            if ((m_Asset != default) && (Prefab == default))
            {
                if (Assets == default)
                {
                    Assets = ShipDockApp.Instance.ABs;
                }
                GameObject source = Assets.Get(m_Asset.GetABName(), m_Asset.GetAssetName());
                Prefab = source;
            }
        }

        public GameObject CreateAsset(bool isCreateFromPool = false)
        {
            "error: Res prefab bridge raw is null, ABName = {0}, AssetName = {1}".Log(Prefab == default, m_Asset.GetABName(), m_Asset.GetAssetName());

            GameObject result = Prefab.Create(isCreateFromPool ? m_PoolID : int.MaxValue);

            return result;
        }

        public void CollectAsset(GameObject target)
        {
            target.Terminate(m_PoolID);
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_Asset.SetSubgroup(ab, asset);
        }

        public void FillRaw(GameObject raw)
        {
            if (Prefab == default)
            {
                Prefab = raw;
            }
        }

        public GameObject Prefab { get; private set; }
    }

}