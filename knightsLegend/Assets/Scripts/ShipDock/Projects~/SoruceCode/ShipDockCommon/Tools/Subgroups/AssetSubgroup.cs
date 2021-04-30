
using System;
using UnityEngine;

namespace ShipDock.Tools
{
    [Serializable]
    public class AssetSubgroup
    {
        [SerializeField]
        private string m_ABName;
        [SerializeField]
        private string m_AssetName;

        public AssetSubgroup(string ab, string asset)
        {
            SetSubgroup(ab, asset);
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_ABName = ab;
            m_AssetName = asset;
        }

        public string GetABName()
        {
            return m_ABName;
        }

        public string GetAssetName()
        {
            return m_AssetName;
        }
    }
}
