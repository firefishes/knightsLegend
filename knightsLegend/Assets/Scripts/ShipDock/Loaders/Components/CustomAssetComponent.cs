
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
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
            if(!m_Valid)
            {
                DestroyImmediate(this);
                return;
            }
        }

        public string GetBundleName()
        {
            return m_BundleName;
        }

        public List<CustomAsset> Assets
        {
            get
            {
                return m_Assets;
            }
        }
    }

}