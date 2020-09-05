
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    [ExecuteInEditMode]
    public class CustomAssetComponent : MonoBehaviour
    {
        [SerializeField]
        private bool m_Valid;
        [SerializeField]
        private string m_BundleName;
        [SerializeField]
        private List<CustomAsset> m_Assets;

        private void Awake()
        {
            //if(!m_Valid)
            //{
            //    DestroyImmediate(this);
            //    return;
            //}
#if UNITY_EDITOR
            Update();
#endif
        }

        public string GetBundleName()
        {
            return m_BundleName;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (m_Valid)
            {
                int max = m_Assets.Count;
                for (int i = 0; i < max; i++)
                {
                    if (m_Assets[i] != default && m_Assets[i].refresh != default)
                    {
                        m_Assets[i].refresh = false;
                        name = m_Assets[i].assetName;
                        break;
                    }
                }
            }
        }
#endif

        public List<CustomAsset> Assets
        {
            get
            {
                return m_Assets;
            }
        }
    }

}