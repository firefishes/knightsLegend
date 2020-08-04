using UnityEngine;

namespace ShipDock.Applications
{
    public class ResPrefabBridge : ResBridge, IResPrefabBridge
    {
        private GameObject mPrefab;

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

            mPrefab = default;
        }

        public void CreateRaw()
        {
            if ((m_Asset != default) && (Prefab == default))
            {
                GameObject source = Assets.Get(m_Asset.GetABName(), m_Asset.GetAssetName());
                mPrefab = source;
            }
        }

        public GameObject CreateAsset(bool isCreateFromPool = false)
        {
            if (isCreateFromPool)
            {
                return ShipDockApp.Instance.AssetsPooling.FromPool(m_PoolID, ref mPrefab);
            }
            else
            {
                return Instantiate(Prefab);
            }
        }

        public void CollectAsset(GameObject target)
        {
            if (m_PoolID == int.MaxValue)
            {
                Destroy(target);
            }
            else
            {
                ShipDockApp.Instance.AssetsPooling.ToPool(m_PoolID, target);
            }
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_Asset.SetSubgroup(ab, asset);
        }

        public GameObject Prefab
        {
            get
            {
                return mPrefab;
            }
        }
    }

}